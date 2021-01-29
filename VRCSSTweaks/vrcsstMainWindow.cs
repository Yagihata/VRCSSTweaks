using MetroFramework;
using MetroFramework.Controls;
using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using ZXing;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Security.Cryptography;
using Ionic.Zip;
using Ionic.Zlib;
using System.Reflection;
using CoreTweet;
using System.Net;

namespace VRCSSTweaks
{
    public partial class vrcsstMainWindow : MetroForm
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0x000B;

        public static void BeginControlUpdate(Control control)
        {
            SendMessage(new HandleRef(control, control.Handle), WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
        }

        public static void EndControlUpdate(Control control)
        {
            SendMessage(new HandleRef(control, control.Handle), WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
            control.Invalidate();
        }

        private FileSystemWatcher fileSystemWatcher = null;
        private FileSystemWatcher newSSMoverWatcher = null;
        private Dictionary<string, FilePathTagList> containTagList = new Dictionary<string, FilePathTagList>();
        private SortableBindingList<TagItem> tagItems = new SortableBindingList<TagItem>();
        private SortableBindingList<TagItem> hashTagItems = new SortableBindingList<TagItem>();
        private BindingSource fileListBindingSource = new BindingSource();
        private BindingSource tagListBindingSource = new BindingSource();
        private BindingSource tagSelectorBindingSource = new BindingSource();
        private bool finishInit = false;
        private string lastFilePath = "";
        private byte lastCompressDate = 0;
        private string apiKey = "";
        private string apiSecret = "";
        private string accessToken = "";
        private string accessSecret = "";
        private Tokens tweeetToken = null;
        private bool minimizeWithCloseButton = false;
        private string msgURL = "";
        private DateTime msgLastGotTime = DateTime.MinValue;
        private string lastDetectSSPath = "";
        public vrcsstMainWindow()
        {
            InitializeComponent();
            Environment.CurrentDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            Directory.SetCurrentDirectory(Environment.CurrentDirectory);

            windowTabControl.SelectedIndex = 0;

            //Load XML
            LoadConfig();
            LoadTags();
            LoadKeys();

            if (!Directory.Exists(settingsFolderLinkSrc.FilePath))
                settingsFolderLinkSrc.FilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\VRChat";
            if (!Directory.Exists(settingsFolderLinkSrc.FilePath))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\VRChat");

            CheckSSFolderIsExist();

            fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Path = GetSSFolderPath();
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fileSystemWatcher.Filter = "*.png";
            fileSystemWatcher.SynchronizingObject = this;
            fileSystemWatcher.Created += new FileSystemEventHandler(fileSystemWatcher_Changed);
            fileSystemWatcher.Changed += new FileSystemEventHandler(fileSystemWatcher_Changed);
            //fileSystemWatcher.Deleted += new FileSystemEventHandler(fileSystemWatcher_Changed);
            fileSystemWatcher.EnableRaisingEvents = true;

            newSSMoverWatcher = new FileSystemWatcher();
            newSSMoverWatcher.Path = settingsFolderLinkSrc.FilePath;
            newSSMoverWatcher.NotifyFilter = NotifyFilters.LastWrite;
            newSSMoverWatcher.Filter = "*.png";
            newSSMoverWatcher.SynchronizingObject = this;
            newSSMoverWatcher.Created += new FileSystemEventHandler(newSSMoverWatcher_Changed);
            newSSMoverWatcher.Changed += new FileSystemEventHandler(newSSMoverWatcher_Changed);
            //newSSMoverWatcher.Deleted += new FileSystemEventHandler(newSSMoverWatcher_Changed);
            newSSMoverWatcher.EnableRaisingEvents = true;
            if (settingsUseDarkMode.Checked)
                metroStyleManager1.Theme = MetroThemeStyle.Dark;
            else
                metroStyleManager1.Theme = MetroThemeStyle.Light;

            this.components.SetDefaultStyle(this, metroStyleManager1.Style);
            this.components.SetDefaultTheme(this, metroStyleManager1.Theme);
            this.StyleManager = metroStyleManager1;
            metroCheckBox1.Enabled = false;
            Task.Factory.StartNew(() => 
            {

                var shaList = Directory.GetFiles(GetSSFolderPath(), "*.png", SearchOption.AllDirectories).ToDictionary(n => GetSHA256(n));
                var count = containTagList.Count();
                var keys = containTagList.Keys.ToList();
                for(int i = 0; i < count; ++i)
                {
                    if (shaList.ContainsKey(keys[i]))
                    {
                        containTagList[keys[i]].FilePath = shaList[keys[i]];
                    }
                    else
                    {
                        containTagList.Remove(keys[i]);
                        keys.RemoveAt(i);
                        --count;
                        --i;
                    }
                }
                this.Invoke((MethodInvoker)(() =>
                {
                    metroCheckBox1.Enabled = true;
                }));
            });

        }

        private void newSSMoverWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (IsSSFolderLinked())
            {
                CheckSSFolderIsExist();
                var path = e.FullPath;
                var directoryPath = GetSSFolderPath() + "\\";
                try
                {
                    var dest = directoryPath + Path.GetFileName(path);
                    if (!File.Exists(dest))
                        File.Move(path, dest);
                }
                catch
                {

                }
            }
        }
        private List<FileItem> GetFileItemsList()
        {
            if (metroCheckBox1.Checked && tempFileItems != null)
                return tempFileItems;
            else
                return rawFileItems;
        }
        private bool IsSSFolderLinked()
        {
            var srcPath = settingsFolderLinkSrc.FilePath;
            var destPath = settingsFolderLinkDest.FilePath;
            return (!string.IsNullOrEmpty(destPath) && destPath != srcPath);
        }
        private void CheckSSFolderIsExist()
        {
            if (!Directory.Exists(settingsFolderLinkSrc.FilePath))
            {
                MessageBox.Show("VRChatのスクリーンショットフォルダが見つかりません\nマイピクチャ内にVRChatフォルダが存在していることを確認してください");
                Application.Exit();
            }
            if (IsSSFolderLinked())
            {
                var destPath = settingsFolderLinkDest.FilePath;
                try
                {
                    if(!Directory.Exists(destPath))
                    {
                        Directory.CreateDirectory(destPath);
                    }
                }
                catch
                {
                    MessageBox.Show("リンクフォルダにアクセスできませんでした");
                    settingsFolderLinkDest.FilePath = "";
                }
            }
        }
        private string GetSSFolderPath()
        {
            CheckSSFolderIsExist();
            if (IsSSFolderLinked())
                return settingsFolderLinkDest.FilePath;
            
            return settingsFolderLinkSrc.FilePath;
        }
        IEnumerable<int> waitCounter = Enumerable.Range(0, 50);
        private void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            var path = e.FullPath;
            if (path == lastDetectSSPath)
                return;
            lastDetectSSPath = path;
            foreach(var v in waitCounter)
            {
                if (File.Exists(path))
                    break;
                Task.Delay(100).Wait();
            }
            if (!settingsRunWithVRC.Checked || CheckIsVRCRunning())
            {
                if (settingsSortSS.Checked)
                {
                    var info = new FileInfo(path);
                    var date = info.CreationTime - TimeSpan.FromHours(settingsBorderHour.Value);
                    var folderName = string.Format("{0}-{1:D2}-{2:D2}", date.Year, date.Month, date.Day);
                    var directoryPath = GetSSFolderPath() + "\\" + folderName;
                    var newPath = directoryPath + "\\" + Path.GetFileName(path);
                    try
                    {
                        if (!Directory.Exists(directoryPath))
                            Directory.CreateDirectory(directoryPath);
                        File.Move(path, newPath);
                        path = newPath;
                        lastFilePath = newPath;
                        LoadRecentlyImage(newPath);
                        if (windowTabControl.SelectedIndex == 0)
                            LoadTagsData(GetSHA256(lastFilePath));
                        if (GetSSFolderPath() == currentDirectory)
                        {
                            var dirInfo = new DirectoryInfo(directoryPath);
                            if(!GetFileItemsList().Any(n => n.Name == dirInfo.Name && n.Tags.Contains("Folder")))
                            {
                                var fileItem = new FileItem();
                                fileItem.Name = dirInfo.Name;
                                fileItem.Size = new FileSize(-1);
                                fileItem.Date = dirInfo.LastWriteTime.ToString();
                                var sha256 = GetSHA256(newPath);
                                fileItem.AddTag("Folder");
                                GetFileItemsList().Add(fileItem);
                                if (!metroCheckBox1.Checked)
                                    fileItems.Add(fileItem);
                                SortRows(metroGrid1.SortedColumn, false);
                            }
                        }
                        else if (Path.GetDirectoryName(newPath) == currentDirectory)
                        {
                            info = new FileInfo(newPath);
                            var fileItem = new FileItem();
                            fileItem.Name = info.Name;
                            fileItem.Size = new FileSize(info);
                            fileItem.Date = info.LastWriteTime.ToString();
                            var sha256 = GetSHA256(newPath);
                            if (containTagList.ContainsKey(sha256))
                            {
                                foreach (var v in containTagList[sha256].TagItems)
                                    fileItem.AddTag(v);
                            }
                            GetFileItemsList().Add(fileItem);
                            if (!metroCheckBox1.Checked)
                                fileItems.Add(fileItem);
                            SortRows(metroGrid1.SortedColumn, false);
                        }
                        if (settingsNewSSToQueue.Checked)
                        {
                            RegisterToImageQueue(newPath);
                            if (windowTabControl.SelectedIndex == 2)
                                ReloadQueueImages();
                        }
                        //UpdateFileList();
                        return;
                    }
                    catch
                    {
                    }
                }
                if (settingsNewSSToQueue.Checked)
                {
                    RegisterToImageQueue(path);
                    if (windowTabControl.SelectedIndex == 2)
                        ReloadQueueImages();
                }
                lastFilePath = path;
                LoadRecentlyImage(path);
                if (Path.GetDirectoryName(path) == currentDirectory)
                {
                    var info = new FileInfo(path);
                    var fileItem = new FileItem();
                    fileItem.Name = info.Name;
                    fileItem.Size = new FileSize(info);
                    fileItem.Date = info.LastWriteTime.ToString();
                    var sha256 = GetSHA256(path);
                    if (containTagList.ContainsKey(sha256))
                    {
                        foreach (var v in containTagList[sha256].TagItems)
                            fileItem.AddTag(v);
                    }
                    GetFileItemsList().Add(fileItem);
                }
            }
        }
        public Image CreateImage(string filename)
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
                return null;
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            Image img = Image.FromStream(fs);
            fs.Close();
            return img;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            if (!finishInit)
            {

                var authenticated = false;
                if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(accessSecret))
                {
                    tweeetToken = Tokens.Create(apiKey, apiSecret, accessToken, accessSecret);
                    var response = tweeetToken.Account.VerifyCredentials();
                    tweeetToken.UserId = response.Id.Value;
                    tweeetToken.ScreenName = response.ScreenName;
                    metroPanel8.Enabled = true;
                    labelTwitterUserID.Text = "@" + tweeetToken.ScreenName;
                    authenticated = true;
                    try
                    {
                    }
                    catch(Exception ex)
                    {
                        LogOutput(ex.ToString());
                    }
                }
                if (!authenticated)
                {
                    tweeetToken = null;
                    accessToken = "";
                    accessSecret = "";
                    metroPanel8.Enabled = false;
                    labelTwitterUserID.Text = "未認証";
                }

                //監視を開始する
                fileSystemWatcher.EnableRaisingEvents = settingsObserveSS.Checked;
                fileListBindingSource.DataSource = fileItems;
                metroGrid1.DataSource = fileListBindingSource;
                gridPRSSTagList.AutoGenerateColumns = false;
                tagListBindingSource.DataSource = tagItems;
                gridPRSSTagList.DataSource = tagListBindingSource;
                gridNSSTagList.AutoGenerateColumns = false;
                gridNSSTagList.DataSource = tagListBindingSource;
                tagSelectorBindingSource.DataSource = tagItems;
                metroComboBox2.DataSource = tagSelectorBindingSource;
                gridTweetHashtagList.DataSource = hashTagItems;
                currentDirectory = GetSSFolderPath();
                LoadRecentlyImage(lastFilePath, false);
                LoadPreviewImage(null);
                finishInit = true;

                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(100);
                    this.Invoke((MethodInvoker)(() =>
                    {
                        listViewTweetImage.BackColor = this.BackColor;
                        listViewTweetQueue.BackColor = this.BackColor;
                        CheckNewMessage();
                        if (IsSSFolderLinked())
                        {
                            var srcPath = settingsFolderLinkSrc.FilePath;
                            var destPath = settingsFolderLinkDest.FilePath;
                            LoadPreviewImage(null);
                            LoadRecentlyImage(null);
                            var files = Directory.GetFiles(srcPath, "*.png", SearchOption.AllDirectories);
                            var dialog = new ProgressWindow(metroStyleManager1);
                            dialog.Title = "スクリーンショット移動中...";
                            dialog.MaxValue = files.Length;
                            dialog.Owner = this;
                            dialog.Show();
                            Task.Factory.StartNew(() =>
                            {
                                this.Invoke((MethodInvoker)(() =>
                                {
                                    windowTabControl.Enabled = false;
                                }));
                                foreach (var path in files)
                                {
                                    try
                                    {
                                        var folderName = Path.GetDirectoryName(path).Replace(srcPath, "");
                                        var directoryPath = GetSSFolderPath() + folderName;
                                        if (!Directory.Exists(directoryPath))
                                            Directory.CreateDirectory(directoryPath);
                                        File.Move(path, directoryPath + "\\" + Path.GetFileName(path));
                                    }
                                    catch
                                    {
                                    }
                                    dialog.Invoke((MethodInvoker)(() => ++dialog.CurrentValue));
                                }
                                dialog.Invoke((MethodInvoker)(() => dialog.Close()));

                                this.Invoke((MethodInvoker)(() =>
                                {
                                    windowTabControl.Enabled = true;
                                    var lastImage = Directory.GetFiles(GetSSFolderPath(), "*.png", SearchOption.TopDirectoryOnly).OrderByDescending(n => File.GetLastWriteTime(n).Ticks).FirstOrDefault();
                                    if (settingsSortSS.Checked)
                                        SortScreenshot(true);
                                    else if (settingsUseCompress.Checked && lastCompressDate != DateTime.Now.Day)
                                        CompressScrenshot();
                                    else
                                        fileListRefresher.RunWorkerAsync();
                                    if (settingsStartWithMinimized.Checked)
                                        WindowState = FormWindowState.Minimized;
                                }));

                            });
                        }
                        else
                        {
                            if (settingsSortSS.Checked)
                                SortScreenshot(true);
                            else if (settingsUseCompress.Checked && lastCompressDate != DateTime.Now.Day)
                                CompressScrenshot();
                            else
                                fileListRefresher.RunWorkerAsync();
                            if (settingsStartWithMinimized.Checked)
                                WindowState = FormWindowState.Minimized;
                        }
                    }));
                });
            }
        }
        private void CheckNewMessage()
        {

            WebClient wc = new WebClient();
            try
            {
                var lines = new List<string>();
                using (var sr = new StringReader(wc.DownloadString(msgURL)))
                {
                    while (sr.Peek() > -1)
                    {
                        var line = sr.ReadLine();

                        if (!string.IsNullOrEmpty(line))
                            lines.Add(line);
                    }
                }
                DateTime dateTime;
                if (lines.Count >= 2 && DateTime.TryParse(lines[0], out dateTime))
                {
                    if (dateTime >= msgLastGotTime)
                    {
                        msgLastGotTime = DateTime.Now;
                        lines.RemoveAt(0);
                        var window = new NewMessageWindow(metroStyleManager1) { MessageText = string.Join("\n", lines), Owner = this };
                        window.ShowDialog();
                    }
                }

            }
            catch
            {
            }
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
                return;
            var index = minimizeWithCloseButton ? settingsCloseButtonMode.SelectedIndex : settingsMinButtonMode.SelectedIndex;
            if (index == 0)
            {
                Visible = false;
                notifyIcon1.Visible = true;
            }
            else if(index == 2)
            {
                Application.Exit();
            }
            minimizeWithCloseButton = false;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Visible = true;
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }
        private void vrcsstMainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (settingsCloseButtonMode.SelectedIndex != 2 && e.CloseReason == CloseReason.UserClosing)
            {
                minimizeWithCloseButton = true;
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
            }
            else
            {
                SaveConfig();
                SaveTags();
            }
        }
        private void LoadKeys()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("VRCSSTweaks.Resources.keys.xml");
            var streamReader = new StreamReader(stream);
            var xmlFile = XElement.Load(streamReader);
            apiKey = (string)GetConfigValue(xmlFile, "APIKey", apiKey, typeof(string));
            apiSecret = (string)GetConfigValue(xmlFile, "APISecret", apiSecret, typeof(string));
            msgURL = (string)GetConfigValue(xmlFile, "MsgURL", msgURL, typeof(string));
        }
        private void LoadConfig()
        {
            var path = Directory.GetCurrentDirectory() + @"\config.xml";
            if (!File.Exists(path))
                return;
            var xmlFile = XElement.Load(path);
            var vrcSSTSettings = xmlFile;

            settingsFolderLinkDest.FilePath = (string)GetConfigValue(vrcSSTSettings, "LinkedSSPath", settingsFolderLinkDest.FilePath, typeof(string));
            settingsFolderLinkSrc.FilePath = (string)GetConfigValue(vrcSSTSettings, "SourceSSPath", settingsFolderLinkSrc.FilePath, typeof(string));
            settingsObserveSS.Checked = (bool)GetConfigValue(vrcSSTSettings, "ObserveScreenShot", settingsObserveSS.Checked, typeof(bool));
            settingsRunWithVRC.Checked = (bool)GetConfigValue(vrcSSTSettings, "ObserveWithVRCRunning", settingsRunWithVRC.Checked, typeof(bool));
            settingsDetectBarcode.Checked = (bool)GetConfigValue(vrcSSTSettings, "DetectBarcode", settingsDetectBarcode.Checked, typeof(bool));
            settingsOpenBarcode.Checked = (bool)GetConfigValue(vrcSSTSettings, "AutoOpenBarcode", settingsOpenBarcode.Checked, typeof(bool));
            settingsSortSS.Checked = (bool)GetConfigValue(vrcSSTSettings, "SortScreenShot", settingsSortSS.Checked, typeof(bool));
            settingsUseStartup.Checked = (bool)GetConfigValue(vrcSSTSettings, "StartupApp", settingsUseStartup.Checked, typeof(bool));
            lastFilePath = (string)GetConfigValue(vrcSSTSettings, "LastImagePath", "", typeof(string));
            settingsUseCompress.Checked = (bool)GetConfigValue(vrcSSTSettings, "EnableCompress", settingsUseCompress.Checked, typeof(bool));
            settingsCompressMethod.SelectedIndex = (int)GetConfigValue(vrcSSTSettings, "CompressSeparateType", settingsCompressMethod.SelectedIndex, typeof(int));
            settingsCompressLevel.SelectedIndex = (int)GetConfigValue(vrcSSTSettings, "CompressLevel", settingsCompressLevel.SelectedIndex, typeof(int));
            settingsCompressDays.Value = (int)GetConfigValue(vrcSSTSettings, "CompressDayLimit", settingsCompressDays.Value, typeof(int));
            lastCompressDate = (byte)GetConfigValue(vrcSSTSettings, "LastCompressDate", lastCompressDate, typeof(byte));
            accessToken = (string)GetConfigValue(vrcSSTSettings, "AccessToken", accessToken, typeof(string));
            accessSecret = (string)GetConfigValue(vrcSSTSettings, "AccessTokenSecret", accessSecret, typeof(string));
            hashTagItems.Clear();
            foreach (var v in ((string)GetConfigValue(vrcSSTSettings, "HashTags", accessSecret, typeof(string))).Split(','))
            hashTagItems.Add(new TagItem() { Name = v });
            settingsNewSSToQueue.Checked = (bool)GetConfigValue(vrcSSTSettings, "NewSSToQueue", settingsNewSSToQueue.Checked, typeof(bool));
            msgLastGotTime = (DateTime)GetConfigValue(vrcSSTSettings, "MessageGotTime", msgLastGotTime, typeof(DateTime));
            settingsBorderHour.Value = (int)GetConfigValue(vrcSSTSettings, "DayBorder", settingsBorderHour.Value, typeof(int));
            settingsUseDarkMode.Checked = (bool)GetConfigValue(vrcSSTSettings, "DarkMode", settingsUseDarkMode.Checked, typeof(bool));
            settingsStartWithMinimized.Checked = (bool)GetConfigValue(vrcSSTSettings, "MinimizedStart", settingsStartWithMinimized.Checked, typeof(bool));
            settingsMinButtonMode.SelectedIndex = (int)GetConfigValue(vrcSSTSettings, "MinimizeMode", settingsMinButtonMode.SelectedIndex, typeof(int));
            settingsCloseButtonMode.SelectedIndex = (int)GetConfigValue(vrcSSTSettings, "CloseMode", settingsCloseButtonMode.SelectedIndex, typeof(int));
        }
        public static void LogOutput(string log)
        {
            var date = DateTime.Now;
            var now = string.Format("{0}-{1}-{2}_{3}-{4}-{5}", date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
            File.WriteAllText(Directory.GetCurrentDirectory() + "\\output_" + now + ".txt", log);
        }
        private void SaveConfig()
        {
            var path = Directory.GetCurrentDirectory() + @"\config.xml";
            var xmlFile = new XElement("VRCSSTSettings");
            xmlFile.Add(new XElement("LinkedSSPath", settingsFolderLinkDest.FilePath));
            xmlFile.Add(new XElement("SourceSSPath", settingsFolderLinkSrc.FilePath));
            xmlFile.Add(new XElement("ObserveScreenShot", settingsObserveSS.Checked));
            xmlFile.Add(new XElement("ObserveWithVRCRunning", settingsRunWithVRC.Checked));
            xmlFile.Add(new XElement("DetectBarcode", settingsDetectBarcode.Checked));
            xmlFile.Add(new XElement("AutoOpenBarcode", settingsOpenBarcode.Checked));
            xmlFile.Add(new XElement("SortScreenShot", settingsSortSS.Checked));
            xmlFile.Add(new XElement("StartupApp", settingsUseStartup.Checked));
            xmlFile.Add(new XElement("LastImagePath", lastFilePath));
            xmlFile.Add(new XElement("EnableCompress", settingsUseCompress.Checked));
            xmlFile.Add(new XElement("CompressSeparateType", settingsCompressMethod.SelectedIndex));
            xmlFile.Add(new XElement("CompressLevel", settingsCompressLevel.SelectedIndex));
            xmlFile.Add(new XElement("CompressDayLimit", settingsCompressDays.Value));
            xmlFile.Add(new XElement("LastCompressDate", lastCompressDate));
            xmlFile.Add(new XElement("AccessToken", accessToken));
            xmlFile.Add(new XElement("AccessTokenSecret", accessSecret));
            xmlFile.Add(new XElement("HashTags", string.Join(",", hashTagItems.Select(n => n.Name))));
            xmlFile.Add(new XElement("NewSSToQueue", settingsNewSSToQueue.Checked));
            xmlFile.Add(new XElement("MessageGotTime", msgLastGotTime));
            xmlFile.Add(new XElement("DayBorder", settingsBorderHour.Value));
            xmlFile.Add(new XElement("DarkMode", settingsUseDarkMode.Checked));
            xmlFile.Add(new XElement("MinimizedStart", settingsStartWithMinimized.Checked));
            xmlFile.Add(new XElement("MinimizeMode", settingsMinButtonMode.SelectedIndex));
            xmlFile.Add(new XElement("CloseMode", settingsCloseButtonMode.SelectedIndex));
            xmlFile.Save(path);
        }
        private void LoadTags()
        {
            containTagList.Clear();
            var path = Directory.GetCurrentDirectory() + @"\tags.xml";
            if (!File.Exists(path))
                return;
            var xmlFile = XElement.Load(path);
            foreach(var element in xmlFile.Elements())
            {
                var tags = element.Value.Split(',').ToList();
                foreach (var v in tags.Where(tagName => !tagItems.Any(tagItem => tagItem.Name == tagName)))
                    tagItems.Add(new TagItem() { Name = v });
                containTagList.Add(element.Name.LocalName.Replace("SHA256_", ""), new FilePathTagList() { TagItems = tags });
            }
        }
        private void SaveTags()
        {
            var path = Directory.GetCurrentDirectory() + @"\tags.xml";
            var xmlFile = new XElement("VRCSSTTags");
            foreach (var tagData in containTagList)
            {
                xmlFile.Add(new XElement("SHA256_" + tagData.Key, string.Join(",", tagData.Value.TagItems)));
            }
            xmlFile.Save(path);
        }
        private bool CheckIsVRCRunning()
        {
            return Process.GetProcessesByName("VRChat").Any();
        }
        private bool IsUrl(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            return Regex.IsMatch(input, @"^s?https?://[-_.!~*'()a-zA-Z0-9;/?:@&=+$,%#]+$"
            );
        }
        private object GetConfigValue(XElement element, string key, object defaultValue, Type type)
        {
            if (type == typeof(string))
            {
                var input = element.Element(key);
                if (input != null)
                    return input.Value;
                else
                    return defaultValue;
            }
            else
            {
                var converter = TypeDescriptor.GetConverter(type);
                if (converter != null)
                {
                    var input = element.Element(key);
                    if (input != null)
                        try
                        {
                            var returnVal = converter.ConvertFromString(input.Value);
                            if (returnVal != null)
                                return returnVal;
                            else
                                return defaultValue;
                        }
                        catch
                        {
                            return defaultValue;
                        }
                }
            }
            return defaultValue;
        }
        private string GetSHA256(string baseTxt)
        {
            byte[] input = Encoding.ASCII.GetBytes(baseTxt);
            SHA256 sha = new SHA256CryptoServiceProvider();
            byte[] hash_sha256 = sha.ComputeHash(input);

            string result = "";

            for (int i = 0; i < hash_sha256.Length; i++)
            {
                result = result + string.Format("{0:X2}", hash_sha256[i]);
            }
            return result;
        }

        private void SortScreenshot(bool compressAfter = false)
        {
            LoadPreviewImage(null);
            LoadRecentlyImage(null);
            var files = Directory.GetFiles(GetSSFolderPath(), "*.png");
            var dialog = new ProgressWindow(metroStyleManager1);
            dialog.Title = "スクリーンショット整理中...";
            dialog.MaxValue = files.Length;
            dialog.Owner = this;
            dialog.Show();
            Task.Factory.StartNew(() =>
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    settingsSortSS.Enabled = false;
                    windowTabControl.Enabled = false;
                }));
                foreach (var path in files)
                {
                    var info = new FileInfo(path);
                    var date = info.CreationTime;
                    var folderName = string.Format("{0}-{1:D2}-{2:D2}", date.Year, date.Month, date.Day);
                    var directoryPath = GetSSFolderPath() + "\\" + folderName;
                    try
                    {
                        if (!Directory.Exists(directoryPath))
                            Directory.CreateDirectory(directoryPath);
                        File.Move(path, directoryPath + "\\" + Path.GetFileName(path));
                    }
                    catch
                    {
                    }
                    dialog.Invoke((MethodInvoker)(() => ++dialog.CurrentValue));
                }
                dialog.Invoke((MethodInvoker)(() => dialog.Close()));

                this.Invoke((MethodInvoker)(() =>
                {
                    if (compressAfter && settingsUseCompress.Checked && lastCompressDate != DateTime.Now.Day)
                        CompressScrenshot();
                    else
                    {
                        settingsSortSS.Enabled = true;
                        windowTabControl.Enabled = true;
                        LoadRecentlyImage(lastFilePath);
                        fileListRefresher.RunWorkerAsync();
                    }
                }));
                
            });
        }
        private void metroButton6_Click(object sender, EventArgs e)
        {
            var files = Directory.GetFiles(GetSSFolderPath(), "*.png", SearchOption.AllDirectories);
            foreach (var path in files)
            {
                if (Path.GetDirectoryName(path) != GetSSFolderPath())
                    File.Move(path, GetSSFolderPath() + "\\" + Path.GetFileName(path));
            }
        }

        private void タグを削除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var rowIndex = -1;
            if (windowTabControl.SelectedIndex == 2)
            {
                rowIndex = gridTweetHashtagList.Rows.IndexOf(gridTweetHashtagList.SelectedRows[0]);
                var name = hashTagItems[rowIndex].Name;
                hashTagItems.RemoveAt(rowIndex);
                ((CurrencyManager)gridTweetHashtagList.BindingContext[hashTagItems]).Refresh();
            }
            else
            {
                if (windowTabControl.SelectedIndex == 0)
                    rowIndex = gridPRSSTagList.Rows.IndexOf(gridPRSSTagList.SelectedRows[0]);
                else if (windowTabControl.SelectedIndex == 1)
                    rowIndex = gridNSSTagList.Rows.IndexOf(gridNSSTagList.SelectedRows[0]);
                else
                    return;
                var name = tagItems[rowIndex].Name;
                tagItems.RemoveAt(rowIndex);
                foreach (var v in containTagList.Values)
                    v.TagItems.Remove(name);
                foreach (var v in rawFileItems)
                    v.RemoveTag(name);
                PoseFileList();
                UpdateFileList();
                ((CurrencyManager)gridPRSSTagList.BindingContext[tagItems]).Refresh();
                ((CurrencyManager)gridNSSTagList.BindingContext[tagItems]).Refresh();
                ((CurrencyManager)metroGrid1.BindingContext[fileItems]).Refresh();
            }
        }

        private void windowTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearTagsData();
            if (windowTabControl.SelectedIndex == 0)
                LoadTagsData(GetSHA256(lastFilePath));
            else if (windowTabControl.SelectedIndex == 1)
            {
                LoadTagsData(GetSHA256(currentPreviewImagePath));
                RefreshFileListTags();
            }
            else if(windowTabControl.SelectedIndex == 2)
            {
                ReloadQueueImages();
            }
        }
        private void CompressScrenshot()
        {
            lastCompressDate = (byte)DateTime.Now.Day;
            LoadPreviewImage(null);
            LoadRecentlyImage(null);
            var files = Directory.GetFiles(GetSSFolderPath(), "*.png", SearchOption.AllDirectories);
            var dialog = new ProgressWindow(metroStyleManager1);
            dialog.Title = "スクリーンショット圧縮中...";
            dialog.MaxValue = files.Length;
            dialog.Owner = this;
            dialog.Show();
            var bindmode = settingsCompressMethod.SelectedIndex;
            var levelIndex = settingsCompressLevel.SelectedIndex;
            var level = (CompressionLevel)levelIndex;
            Console.WriteLine(settingsCompressLevel.SelectedIndex);
            Console.WriteLine(level);
            Task.Factory.StartNew(() =>
            {
                this.Invoke((MethodInvoker)(() => 
                {
                    settingsUseCompress.Enabled = false;
                    windowTabControl.Enabled = false;
                }));
                ZipFile zip = null;
                var currentZipFile = "";
                var limit = settingsCompressDays.Value;
                long byteCount = 0;
                var deletePath = new List<string>();
                if (!Directory.Exists(GetSSFolderPath() + "\\Archives"))
                    Directory.CreateDirectory(GetSSFolderPath() + "\\Archives");
                foreach (var path in files)
                {
                    var info = new FileInfo(path);
                    var date = info.CreationTime;
                    if((DateTime.Now - date).TotalDays >= limit)
                    {
                        var zipName = "";
                        if (bindmode == 0)
                            zipName = GetSSFolderPath() + "\\Archives\\" + string.Format("{0}-{1:D2}-{2:D2}_L{3}.zip", date.Year, date.Month, date.Day, levelIndex);
                        else if (bindmode == 1)
                            zipName = GetSSFolderPath() + "\\Archives\\" + string.Format("{0}-{1:D1}_L{2}.zip", date.Year, date.Month, levelIndex);
                        else
                            zipName = GetSSFolderPath() + "\\Archives\\" + string.Format("CompressedSS_L{0}.zip", levelIndex);
                        if (zip == null || zipName != currentZipFile)
                        {
                            if (zip != null)
                            {
                                zip.Save();
                                zip.Dispose();
                                zip = null;
                            }
                            zip = new ZipFile(zipName, Encoding.GetEncoding("shift_jis"));
                            zip.CompressionLevel = level;
                            currentZipFile = zipName;
                        }
                        var entryName = Path.GetDirectoryName(path).Replace(GetSSFolderPath() + "\\", "");
                        try
                        {
                            zip.AddItem(path, entryName);
                            byteCount += info.Length;
                            if (byteCount > 104857600)
                            {
                                if (zip != null)
                                {
                                    zip.Save();
                                    zip.Dispose();
                                    zip = null;
                                }
                                byteCount = 0;
                            }
                            deletePath.Add(path);
                        }
                        catch
                        {
                        }
                    }
                    dialog.Invoke((MethodInvoker)(() => ++dialog.CurrentValue));
                }
                if (zip != null)
                {
                    zip.Save();
                    zip.Dispose();
                    zip = null;
                }
                foreach(var v in deletePath)
                {
                    File.Delete(v);
                    var dir = Path.GetDirectoryName(v);
                    if (!Directory.EnumerateFileSystemEntries(dir).Any())
                        Directory.Delete(dir);
                }
                dialog.Invoke((MethodInvoker)(() => dialog.Close()));
                this.Invoke((MethodInvoker)(() =>
                {
                    settingsUseCompress.Enabled = true;
                    windowTabControl.Enabled = true;
                    LoadRecentlyImage(lastFilePath); 
                    fileListRefresher.RunWorkerAsync();
                }));

            });
        }

        private void dayChangeDetector_Tick(object sender, EventArgs e)
        {
            if (settingsUseCompress.Enabled && lastCompressDate != DateTime.Now.Day)
            {
                CheckNewMessage();
                CompressScrenshot();
            }
        }

        private void 終了ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}

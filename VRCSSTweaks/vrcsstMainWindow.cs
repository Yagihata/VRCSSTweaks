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
        private Dictionary<string, List<string>> containTagList = new Dictionary<string, List<string>>();
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
        private string msgURL = "";
        private DateTime msgLastGotTime = DateTime.MinValue;
        public vrcsstMainWindow()
        {
            InitializeComponent();
            comboBoxCompressMethod.SelectedIndex = 1;
            comboBoxCompressLevel.SelectedIndex = 9;
            textBoxCompressDays.Text = "180";
            windowTabControl.SelectedIndex = 0;

            if (!Directory.Exists(ssFolderPath.Text))
                ssFolderPath.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\VRChat";
            if (!Directory.Exists(ssFolderPath.Text))
            {
                MessageBox.Show("VRChatのスクリーンショットフォルダが見つかりません\nスクリーンショットのフォルダを選択して下さい");
                using (var ofd = new CommonOpenFileDialog() { InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), DefaultFileName = "VRChat", IsFolderPicker = true })
                {
                    if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        ssFolderPath.Text = ofd.FileName;
                    }
                }
            }


            if (fileSystemWatcher != null) return;

            fileSystemWatcher = new FileSystemWatcher();
            //監視するディレクトリを指定
            fileSystemWatcher.Path = ssFolderPath.Text;
            //最終アクセス日時、最終更新日時、ファイル、フォルダ名の変更を監視する
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            //すべてのファイルを監視
            fileSystemWatcher.Filter = "*.png";
            //UIのスレッドにマーシャリングする
            //コンソールアプリケーションでの使用では必要ない
            fileSystemWatcher.SynchronizingObject = this;

            //イベントハンドラの追加
            fileSystemWatcher.Changed += new FileSystemEventHandler(fileSystemWatcher_Changed);
            fileSystemWatcher.Created += new FileSystemEventHandler(fileSystemWatcher_Changed);
            fileSystemWatcher.Deleted += new FileSystemEventHandler(fileSystemWatcher_Changed);

            //Load XML
            LoadConfig();
            LoadTags();
            LoadKeys();

            if (toggleUseDarkMode.Checked)
                metroStyleManager1.Theme = MetroThemeStyle.Dark;
            else
                metroStyleManager1.Theme = MetroThemeStyle.Light;

            this.components.SetDefaultStyle(this, metroStyleManager1.Style);
            this.components.SetDefaultTheme(this, metroStyleManager1.Theme);
            this.StyleManager = metroStyleManager1;
            listViewTweetImage.BackColor = this.BackColor;
            listViewTweetQueue.BackColor = this.BackColor;
        }
        private void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            var path = e.FullPath;
            if (!toggleObserveRunningVRC.Checked || CheckIsVRCRunning())
            {
                if (toggleSortSS.Checked)
                {
                    var info = new FileInfo(path);
                    var date = info.CreationTime - TimeSpan.FromHours(int.Parse(textBoxBorderHour.Text));
                    var folderName = string.Format("{0}-{1:D2}-{2:D2}", date.Year, date.Month, date.Day);
                    var directoryPath = ssFolderPath.Text + "\\" + folderName;
                    var newPath = directoryPath + "\\" + Path.GetFileName(path);
                    try
                    {
                        if (!Directory.Exists(directoryPath))
                            Directory.CreateDirectory(directoryPath);
                        File.Move(path, newPath);
                        lastFilePath = newPath;
                        LoadRecentlyImage(newPath);
                        if (windowTabControl.SelectedIndex == 0)
                            LoadTagsData(GetSHA256(lastFilePath));
                        if (ssFolderPath.Text == currentDirectory)
                        {
                            var dirInfo = new DirectoryInfo(directoryPath);
                            var fileItem = new FileItem();
                            fileItem.Name = dirInfo.Name;
                            fileItem.Size = new FileSize(-1);
                            fileItem.Date = dirInfo.LastWriteTime.ToString();
                            var sha256 = GetSHA256(newPath);
                            fileItem.AddTag("Folder");
                            rawFileItems.Add(fileItem);
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
                                foreach (var v in containTagList[sha256])
                                    fileItem.AddTag(v);
                            }
                            rawFileItems.Add(fileItem);
                        }
                        if (toggleNewSSToQueue.Checked)
                        {
                            RegisterToImageQueue(newPath);
                            if (windowTabControl.SelectedIndex == 2)
                                ReloadQueueImages();
                        }
                        UpdateFileList();
                        return;
                    }
                    catch
                    {
                    }
                }
                if (toggleNewSSToQueue.Checked)
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
                        foreach (var v in containTagList[sha256])
                            fileItem.AddTag(v);
                    }
                    rawFileItems.Add(fileItem);
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
                fileSystemWatcher.EnableRaisingEvents = toggleObserveSS.Checked;
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
                currentDirectory = ssFolderPath.Text;
                LoadRecentlyImage(lastFilePath, false);
                LoadPreviewImage(null);
                finishInit = true;

                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(500);
                    this.Invoke((MethodInvoker)(() =>
                    {
                        CheckNewMessage();
                        if (toggleSortSS.Checked)
                            SortScreenshot(true);
                        else if (toggleCompression.Checked && lastCompressDate != DateTime.Now.Day)
                            CompressScrenshot();
                        else
                            fileListRefresher.RunWorkerAsync();
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
            Visible = false;
            notifyIcon1.Visible = true;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Visible = true;
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }
        private void vrcsstMainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveConfig();
            SaveTags();
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

            ssFolderPath.Text = (string)GetConfigValue(vrcSSTSettings, "SSDirectoryPath", ssFolderPath.Text, typeof(string));

            toggleObserveSS.Checked = (bool)GetConfigValue(vrcSSTSettings, "ObserveScreenShot", toggleObserveSS.Checked, typeof(bool));
            toggleObserveRunningVRC.Checked = (bool)GetConfigValue(vrcSSTSettings, "ObserveWithVRCRunning", toggleObserveRunningVRC.Checked, typeof(bool));
            toggleDetectBarcode.Checked = (bool)GetConfigValue(vrcSSTSettings, "DetectBarcode", toggleDetectBarcode.Checked, typeof(bool));
            toggleOpenBarcode.Checked = (bool)GetConfigValue(vrcSSTSettings, "AutoOpenBarcode", toggleOpenBarcode.Checked, typeof(bool));
            toggleSortSS.Checked = (bool)GetConfigValue(vrcSSTSettings, "SortScreenShot", toggleSortSS.Checked, typeof(bool));
            toggleStartup.Checked = (bool)GetConfigValue(vrcSSTSettings, "StartupApp", toggleStartup.Checked, typeof(bool));
            lastFilePath = (string)GetConfigValue(vrcSSTSettings, "LastImagePath", "", typeof(string));
            toggleCompression.Checked = (bool)GetConfigValue(vrcSSTSettings, "EnableCompress", toggleCompression.Checked, typeof(bool));
            comboBoxCompressMethod.SelectedIndex = (int)GetConfigValue(vrcSSTSettings, "CompressSeparateType", comboBoxCompressMethod.SelectedIndex, typeof(int));
            comboBoxCompressLevel.SelectedIndex = (int)GetConfigValue(vrcSSTSettings, "CompressLevel", comboBoxCompressLevel.SelectedIndex, typeof(int));
            textBoxCompressDays.Text = (string)GetConfigValue(vrcSSTSettings, "CompressDayLimit", textBoxCompressDays.Text, typeof(string));
            lastCompressDate = (byte)GetConfigValue(vrcSSTSettings, "LastCompressDate", lastCompressDate, typeof(byte));
            accessToken = (string)GetConfigValue(vrcSSTSettings, "AccessToken", accessToken, typeof(string));
            accessSecret = (string)GetConfigValue(vrcSSTSettings, "AccessTokenSecret", accessSecret, typeof(string));
            hashTagItems.Clear();
            foreach (var v in ((string)GetConfigValue(vrcSSTSettings, "HashTags", accessSecret, typeof(string))).Split(','))
            hashTagItems.Add(new TagItem() { Name = v });
            toggleNewSSToQueue.Checked = (bool)GetConfigValue(vrcSSTSettings, "NewSSToQueue", toggleNewSSToQueue.Checked, typeof(bool));
            msgLastGotTime = (DateTime)GetConfigValue(vrcSSTSettings, "MessageGotTime", msgLastGotTime, typeof(DateTime));
            textBoxBorderHour.Text = (string)GetConfigValue(vrcSSTSettings, "DayBorder", textBoxBorderHour.Text, typeof(string));
            toggleUseDarkMode.Checked = (bool)GetConfigValue(vrcSSTSettings, "DarkMode", toggleUseDarkMode.Checked, typeof(bool));
        }
        private void LogOutput(string log)
        {
            var date = DateTime.Now;
            var now = string.Format("{0}-{1}-{2}_{3}-{4}-{5}", date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
            File.WriteAllText(Directory.GetCurrentDirectory() + "\\output_" + now + ".txt", log);
        }
        private void SaveConfig()
        {
            var path = Directory.GetCurrentDirectory() + @"\config.xml";
            var xmlFile = new XElement("VRCSSTSettings");
            xmlFile.Add(new XElement("SSDirectoryPath", ssFolderPath.Text));
            xmlFile.Add(new XElement("ObserveScreenShot", toggleObserveSS.Checked));
            xmlFile.Add(new XElement("ObserveWithVRCRunning", toggleObserveRunningVRC.Checked));
            xmlFile.Add(new XElement("DetectBarcode", toggleDetectBarcode.Checked));
            xmlFile.Add(new XElement("AutoOpenBarcode", toggleOpenBarcode.Checked));
            xmlFile.Add(new XElement("SortScreenShot", toggleSortSS.Checked));
            xmlFile.Add(new XElement("StartupApp", toggleStartup.Checked));
            xmlFile.Add(new XElement("LastImagePath", lastFilePath));
            xmlFile.Add(new XElement("EnableCompress", toggleCompression.Checked));
            xmlFile.Add(new XElement("CompressSeparateType", comboBoxCompressMethod.SelectedIndex));
            xmlFile.Add(new XElement("CompressLevel", comboBoxCompressLevel.SelectedIndex));
            xmlFile.Add(new XElement("CompressDayLimit", textBoxCompressDays.Text));
            xmlFile.Add(new XElement("LastCompressDate", lastCompressDate));
            xmlFile.Add(new XElement("AccessToken", accessToken));
            xmlFile.Add(new XElement("AccessTokenSecret", accessSecret));
            xmlFile.Add(new XElement("HashTags", string.Join(",", hashTagItems.Select(n => n.Name))));
            xmlFile.Add(new XElement("NewSSToQueue", toggleNewSSToQueue.Checked));
            xmlFile.Add(new XElement("MessageGotTime", msgLastGotTime));
            xmlFile.Add(new XElement("DayBorder", textBoxBorderHour.Text));
            xmlFile.Add(new XElement("DarkMode", toggleUseDarkMode.Checked));
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
                containTagList.Add(element.Name.LocalName.Replace("SHA256_", ""), tags);
            }
        }
        private void SaveTags()
        {
            var path = Directory.GetCurrentDirectory() + @"\tags.xml";
            var xmlFile = new XElement("VRCSSTTags");
            foreach (var tagData in containTagList)
            {
                xmlFile.Add(new XElement("SHA256_" + tagData.Key, string.Join(",", tagData.Value)));
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
                            return converter.ConvertFromString(input.Value);
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
            var files = Directory.GetFiles(ssFolderPath.Text, "*.png");
            var dialog = new ProgressWindow(metroStyleManager1);
            dialog.Title = "スクリーンショット整理中...";
            dialog.MaxValue = files.Length;
            dialog.Owner = this;
            dialog.Show();
            Task.Factory.StartNew(() =>
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    toggleSortSS.Enabled = false;
                    windowTabControl.Enabled = false;
                }));
                foreach (var path in files)
                {
                    var info = new FileInfo(path);
                    var date = info.CreationTime;
                    var folderName = string.Format("{0}-{1:D2}-{2:D2}", date.Year, date.Month, date.Day);
                    var directoryPath = ssFolderPath.Text + "\\" + folderName;
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
                    if (compressAfter && toggleCompression.Checked && lastCompressDate != DateTime.Now.Day)
                        CompressScrenshot();
                    else
                    {
                        toggleSortSS.Enabled = true;
                        windowTabControl.Enabled = true;
                        LoadRecentlyImage(lastFilePath);
                        fileListRefresher.RunWorkerAsync();
                    }
                }));
                
            });
        }
        private void metroButton6_Click(object sender, EventArgs e)
        {
            var files = Directory.GetFiles(ssFolderPath.Text, "*.png", SearchOption.AllDirectories);
            foreach (var path in files)
            {
                if (Path.GetDirectoryName(path) != ssFolderPath.Text)
                    File.Move(path, ssFolderPath.Text + "\\" + Path.GetFileName(path));
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
                    v.Remove(name);
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
                LoadTagsData(currentPreviewImageSHA256);
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
            var files = Directory.GetFiles(ssFolderPath.Text, "*.png", SearchOption.AllDirectories);
            var dialog = new ProgressWindow(metroStyleManager1);
            dialog.Title = "スクリーンショット圧縮中...";
            dialog.MaxValue = files.Length;
            dialog.Owner = this;
            dialog.Show();
            var bindmode = comboBoxCompressMethod.SelectedIndex;
            var levelIndex = comboBoxCompressLevel.SelectedIndex;
            var level = (CompressionLevel)levelIndex;
            Console.WriteLine(comboBoxCompressLevel.SelectedIndex);
            Console.WriteLine(level);
            Task.Factory.StartNew(() =>
            {
                this.Invoke((MethodInvoker)(() => 
                {
                    toggleCompression.Enabled = false;
                    windowTabControl.Enabled = false;
                }));
                ZipFile zip = null;
                var currentZipFile = "";
                var limit = int.Parse(textBoxCompressDays.Text);
                long byteCount = 0;
                var deletePath = new List<string>();
                if (!Directory.Exists(ssFolderPath.Text + "\\Archives"))
                    Directory.CreateDirectory(ssFolderPath.Text + "\\Archives");
                foreach (var path in files)
                {
                    var info = new FileInfo(path);
                    var date = info.CreationTime;
                    if((DateTime.Now - date).TotalDays >= limit)
                    {
                        var zipName = "";
                        if (bindmode == 0)
                            zipName = ssFolderPath.Text + "\\Archives\\" + string.Format("{0}-{1:D2}-{2:D2}_L{3}.zip", date.Year, date.Month, date.Day, levelIndex);
                        else if (bindmode == 1)
                            zipName = ssFolderPath.Text + "\\Archives\\" + string.Format("{0}-{1:D1}_L{2}.zip", date.Year, date.Month, levelIndex);
                        else
                            zipName = ssFolderPath.Text + "\\Archives\\" + string.Format("CompressedSS_L{0}.zip", levelIndex);
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
                        var entryName = Path.GetDirectoryName(path).Replace(ssFolderPath.Text + "\\", "");
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
                    toggleCompression.Enabled = true;
                    windowTabControl.Enabled = true;
                    LoadRecentlyImage(lastFilePath); 
                    fileListRefresher.RunWorkerAsync();
                }));

            });
        }

        private void dayChangeDetector_Tick(object sender, EventArgs e)
        {
            if (toggleCompression.Enabled && lastCompressDate != DateTime.Now.Day)
            {
                CheckNewMessage();
                CompressScrenshot();
            }
        }

        private void 終了ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

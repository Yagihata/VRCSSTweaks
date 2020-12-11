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
        private BindingSource fileListBindingSource = new BindingSource();
        private BindingSource tagListBindingSource = new BindingSource();
        private BindingSource tagSelectorBindingSource = new BindingSource();
        private bool finishInit = false;
        private string lastFilePath = "";
        public vrcsstMainWindow()
        {
            InitializeComponent();
            this.components.SetDefaultStyle(this, metroStyleManager1.Style);
            this.components.SetDefaultTheme(this, metroStyleManager1.Theme);
            this.StyleManager = metroStyleManager1;
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

            //Load Config XML
            LoadConfig();
            LoadTags();

            //監視を開始する
            fileSystemWatcher.EnableRaisingEvents = toggleObserveSS.Checked;
            fileListBindingSource.DataSource = fileItems;
            metroGrid1.DataSource = fileListBindingSource;
            metroGrid2.AutoGenerateColumns = false;
            tagListBindingSource.DataSource = tagItems;
            metroGrid2.DataSource = tagListBindingSource;
            tagSelectorBindingSource.DataSource = tagItems;
            metroComboBox2.DataSource = tagSelectorBindingSource;
            currentDirectory = ssFolderPath.Text;
            LoadRecentlyImage(null, false);
            LoadPreviewImage(null);
            finishInit = true;
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
            if (toggleSortSS.Checked)
                SortScreenshot();
            else
                fileListRefresher.RunWorkerAsync();
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

        private void SortScreenshot()
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
                toggleSortSS.Invoke((MethodInvoker)(() => toggleSortSS.Enabled = false));
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

                if (!toggleSortSS.Enabled)
                {
                    toggleSortSS.Invoke((MethodInvoker)(() => toggleSortSS.Enabled = true));
                }
                this.Invoke((MethodInvoker)(() => { LoadRecentlyImage(lastFilePath); fileListRefresher.RunWorkerAsync(); }));
                
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
    }
}

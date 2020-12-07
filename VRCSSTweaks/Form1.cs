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

        private List<Control> resizeSuspendedComponents = new List<Control>();
        private FileSystemWatcher fileSystemWatcher = null;

        public vrcsstMainWindow()
        {
            InitializeComponent();
            this.components.SetDefaultStyle(this, metroStyleManager1.Style);
            this.components.SetDefaultTheme(this, metroStyleManager1.Theme);
            this.StyleManager = metroStyleManager1;
            resizeSuspendedComponents.Add(panelNewScreenshot);
            //Load Config XML
            LoadConfig();
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
            var lastImage = Directory.GetFiles(ssFolderPath.Text, "*.png", SearchOption.TopDirectoryOnly)
                                    .OrderByDescending(n => File.GetLastWriteTime(n).Ticks).First();
            LoadRecentlyImage(lastImage, false);


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

            //監視を開始する
            fileSystemWatcher.EnableRaisingEvents = toggleObserveSS.Checked;
        }

        private void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (CheckIsVRCRunning())
            {
                LoadRecentlyImage(e.FullPath);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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

        private void toggleDetectBarcode_CheckedChanged(object sender, EventArgs e)
        {
            metroPanel3.Enabled = (sender as MetroToggle).Checked;
        }

        private void toggleObserveSS_CheckedChanged(object sender, EventArgs e)
        {
            metroPanel2.Enabled = (sender as MetroToggle).Checked;
            fileSystemWatcher.EnableRaisingEvents = toggleObserveSS.Checked;
        }
        private void vrcsstMainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveConfig();
        }
        private void LoadConfig()
        {
            var path = Directory.GetCurrentDirectory() + @"\config.xml";
            if (!File.Exists(path))
                return;
            var xmlFile = XElement.Load(path);
            var vrcSSTSettings = xmlFile;
            ssFolderPath.Text = vrcSSTSettings.Element("SSDirectoryPath").Value;
            toggleObserveSS.Checked = Boolean.Parse(vrcSSTSettings.Element("ObserveScreenShot").Value);
            toggleObserveRunningVRC.Checked = Boolean.Parse(vrcSSTSettings.Element("ObserveWithVRCRunning").Value);
            toggleDetectBarcode.Checked = Boolean.Parse(vrcSSTSettings.Element("DetectBarcode").Value);
            toggleOpenBarcode.Checked = Boolean.Parse(vrcSSTSettings.Element("AutoOpenBarcode").Value);
            toggleSortSS.Checked = Boolean.Parse(vrcSSTSettings.Element("SortScreenShot").Value);
            toggleStartup.Checked = Boolean.Parse(vrcSSTSettings.Element("StartupApp").Value);
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
            Console.WriteLine(path);
        }

        private void vrcsstMainWindow_ResizeBegin(object sender, EventArgs e)
        {
        }

        private void vrcsstMainWindow_ResizeEnd(object sender, EventArgs e)
        {
        }
        private void LoadRecentlyImage(string path, bool openBarcode = true)
        {
            var info = new FileInfo(path);
            labelRecentlySSName.Text = info.Name;
            labelRecentlySSDate.Text = info.LastWriteTime.ToString();
            labelRecentlySSSize.Text = FormatSize(info.Length, 2).ToString();
            var file = Image.FromStream(info.OpenRead());
            panelNewScreenshot.BackgroundImage = file;
            if (toggleDetectBarcode.Checked)
            {
                BarcodeReader reader = new BarcodeReader();
                Result result = reader.Decode(file as Bitmap);
                if (result != null)
                {
                    textBoxRecentlySSURL.Text = result.Text;
                    if (toggleOpenBarcode.Checked && IsUrl(result.Text) && openBarcode)
                        Process.Start(result.Text);
                }
                else
                    textBoxRecentlySSURL.Text = "未検出";
            }
            else
                textBoxRecentlySSURL.Text = "未検出";
        }
        public string FormatSize(long amt, int rounding)
        {
            /// <summary>
            /// ByteをKB, MB, GB...のような他の形式に変換する
            /// KB, MB, GB, TB, PB, EB, ZB or YB
            /// 第１引数:long型
            /// 第２引数：小数点第何位まで表示するか
            /// </summary>

            if (amt >= Math.Pow(2, 80)) return Math.Round(amt
                / Math.Pow(2, 70), rounding).ToString() + " YB"; //yettabyte
            if (amt >= Math.Pow(2, 70)) return Math.Round(amt
                / Math.Pow(2, 70), rounding).ToString() + " ZB"; //zettabyte
            if (amt >= Math.Pow(2, 60)) return Math.Round(amt
                / Math.Pow(2, 60), rounding).ToString() + " EB"; //exabyte
            if (amt >= Math.Pow(2, 50)) return Math.Round(amt
                / Math.Pow(2, 50), rounding).ToString() + " PB"; //petabyte
            if (amt >= Math.Pow(2, 40)) return Math.Round(amt
                / Math.Pow(2, 40), rounding).ToString() + " TB"; //terabyte
            if (amt >= Math.Pow(2, 30)) return Math.Round(amt
                / Math.Pow(2, 30), rounding).ToString() + " GB"; //gigabyte
            if (amt >= Math.Pow(2, 20)) return Math.Round(amt
                / Math.Pow(2, 20), rounding).ToString() + " MB"; //megabyte
            if (amt >= Math.Pow(2, 10)) return Math.Round(amt
                / Math.Pow(2, 10), rounding).ToString() + " KB"; //kilobyte

            return amt.ToString() + " Bytes"; //byte
        }
        private bool CheckIsVRCRunning()
        {
            return Process.GetProcessesByName("vrchat.exe").Any();
        }
        private void ssFolderSelectButton_Click(object sender, EventArgs e)
        {
            using (var ofd = new CommonOpenFileDialog() { DefaultDirectory = ssFolderPath.Text, IsFolderPicker = true })
            {
                if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    ssFolderPath.Text = ofd.FileName;
                    var lastImage = Directory.GetFiles(ssFolderPath.Text, "*.png", SearchOption.TopDirectoryOnly).OrderByDescending(n => File.GetLastWriteTime(n).Ticks).First();
                    LoadRecentlyImage(lastImage);
                }
            }
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

        private void toggleStartup_CheckedChanged(object sender, EventArgs e)
        {
            if(toggleStartup.Checked)
            {
                RegistryKey regkey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                regkey.SetValue(Application.ProductName, Application.ExecutablePath);
                regkey.Close();
            }
            else
            {
                //Runキーを開く
                RegistryKey regkey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                regkey.DeleteValue(Application.ProductName);
                //閉じる
                regkey.Close();
            }
        }
    }
}

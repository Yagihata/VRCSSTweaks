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
            ssFolderPath.Text = LoadConfig();
            if (!Directory.Exists(ssFolderPath.Text))
                ssFolderPath.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\VRChat";

            var lastImage = Directory.GetFiles(ssFolderPath.Text, "*.png", SearchOption.TopDirectoryOnly)
                                    .OrderByDescending(n => File.GetLastWriteTime(n).Ticks).First();
            LoadRecentlyImage(lastImage);


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
            fileSystemWatcher.EnableRaisingEvents = true;
            Console.WriteLine("監視を開始しました。");
            if (toggleObserveSS.Checked)
            {

            }
        }

        private void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine(e.FullPath);
            LoadRecentlyImage(e.FullPath);
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
        }
        private void vrcsstMainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveConfig();
        }
        private string LoadConfig()
        {
            var path = Directory.GetCurrentDirectory() + @"\config.xml";
            if (!File.Exists(path))
                return "";
            var xmlFile = XElement.Load(path);
            var vrcSSTSettings = xmlFile;
            var ssDirectoryPath = vrcSSTSettings.Element("SSDirectoryPath");
            return ssDirectoryPath.Value.ToString();
        }
        private void SaveConfig()
        {
            var path = Directory.GetCurrentDirectory() + @"\config.xml";
            var xmlFile = new XElement("VRCSSTSettings");
            xmlFile.Add(new XElement("SSDirectoryPath", ssFolderPath.Text));
            xmlFile.Save(path);
            Console.WriteLine(path);
        }

        private void vrcsstMainWindow_ResizeBegin(object sender, EventArgs e)
        {
            resizeSuspendedComponents.ForEach(n => n.SuspendLayout());
            resizeSuspendedComponents.ForEach(n => BeginControlUpdate(n));
        }

        private void vrcsstMainWindow_ResizeEnd(object sender, EventArgs e)
        {
            resizeSuspendedComponents.ForEach(n => EndControlUpdate(n));
            resizeSuspendedComponents.ForEach(n => n.ResumeLayout());
        }
        private void LoadRecentlyImage(string path)
        {
            var info = new FileInfo(path);
            labelRecentlySSName.Text = info.Name;
            labelRecentlySSDate.Text = info.LastWriteTime.ToString();
            labelRecentlySSSize.Text = FormatSize(info.Length, 2).ToString();
            var file = Image.FromStream(info.OpenRead());
            panelNewScreenshot.BackgroundImage = file;
            BarcodeReader reader = new BarcodeReader();
            Result result = reader.Decode(file as Bitmap);
            if (result != null)
            {
                textBoxRecentlySSURL.Text = result.Text;
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
    }
}

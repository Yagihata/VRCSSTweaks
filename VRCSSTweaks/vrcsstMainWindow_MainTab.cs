using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZXing;

namespace VRCSSTweaks
{
    public partial class vrcsstMainWindow
    {
        private void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (!toggleObserveRunningVRC.Checked || CheckIsVRCRunning())
            {
                LoadRecentlyImage(e.FullPath);
            }
        }
        private void LoadRecentlyImage(string path, bool openBarcode = true)
        {
            var info = new FileInfo(path);
            labelRecentlySSName.Text = info.Name;
            labelRecentlySSDate.Text = info.LastWriteTime.ToString();
            labelRecentlySSSize.Text = new FileSize(info).ToString();
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
                textBoxRecentlySSURL.Text = "バーコード読み取り:無効";
        }
        private void metroButton1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBoxRecentlySSURL.Text);
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            var txt = textBoxRecentlySSURL.Text;
            if (IsUrl(txt))
                Process.Start(txt);
        }
    }
}

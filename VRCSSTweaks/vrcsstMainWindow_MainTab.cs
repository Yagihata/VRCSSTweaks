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
                if(toggleSortSS.Checked)
                {
                    var path = e.FullPath;
                    var info = new FileInfo(path);
                    var date = info.CreationTime;
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
                        return;
                    }
                    catch
                    {
                    }
                }
                lastFilePath = e.FullPath;
                LoadRecentlyImage(e.FullPath);
            }
        }
        private void LoadRecentlyImage(string path, bool openBarcode = true)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                labelRecentlySSName.Text = "";
                labelRecentlySSDate.Text = "";
                labelRecentlySSSize.Text = "";
                panelNewScreenshot.BackgroundImage = null;
                textBoxRecentlySSURL.Text = "未検出";
            }
            else
            {
                var info = new FileInfo(path);
                labelRecentlySSName.Text = info.Name;
                labelRecentlySSDate.Text = info.LastWriteTime.ToString();
                labelRecentlySSSize.Text = new FileSize(info).ToString();
                var file = CreateImage(path);
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

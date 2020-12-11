using MetroFramework.Controls;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VRCSSTweaks
{
    public partial class vrcsstMainWindow
    {

        private void toggleStartup_CheckedChanged(object sender, EventArgs e)
        {
            if (toggleStartup.Checked)
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

        private void toggleDetectBarcode_CheckedChanged(object sender, EventArgs e)
        {
            metroPanel3.Enabled = (sender as MetroToggle).Checked;
        }

        private void toggleObserveSS_CheckedChanged(object sender, EventArgs e)
        {
            metroPanel2.Enabled = (sender as MetroToggle).Checked;
            fileSystemWatcher.EnableRaisingEvents = toggleObserveSS.Checked;
        }

        private void toggleSortSS_CheckedChanged(object sender, EventArgs e)
        {
            if (finishInit)
            {
                if (toggleSortSS.Checked)
                {
                    if (MessageBox.Show("既に存在するファイルを日付分けしますか？", "確認", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        SortScreenshot();
                    }
                }
            }
        }
    }
}

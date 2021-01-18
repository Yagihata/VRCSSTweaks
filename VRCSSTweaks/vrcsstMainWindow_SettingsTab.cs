using MetroFramework.Controls;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            using (var ofd = new CommonOpenFileDialog() { DefaultDirectory = textBoxSSFolder.Text, IsFolderPicker = true })
            {
                if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    textBoxSSFolder.Text = ofd.FileName;
                }
            }
        }

        private void textBoxSSFolder_TextChanged(object sender, EventArgs e)
        {
            if (finishInit)
            {
                CheckSSFolderIsExist();
                if (!IsSSFolderLinked())
                    return;
                fileSystemWatcher.Path = GetSSFolderPath();
                currentDirectory = GetSSFolderPath();
                if (MessageBox.Show("元フォルダ内のファイルをリンク先へ移動しますか？", "確認", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var srcPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\VRChat";
                    var destPath = textBoxSSFolder.Text;
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
                            LoadRecentlyImage(lastImage);
                            fileListRefresher.RunWorkerAsync();
                        }));

                    });
                }
                else
                {
                    var lastImage = Directory.GetFiles(GetSSFolderPath(), "*.png", SearchOption.TopDirectoryOnly).OrderByDescending(n => File.GetLastWriteTime(n).Ticks).FirstOrDefault();
                    LoadRecentlyImage(lastImage);
                    fileListRefresher.RunWorkerAsync();
                }
            }
        }

        private void toggleDetectBarcode_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void toggleObserveSS_CheckedChanged(object sender, EventArgs e)
        {
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

        private void textBoxCompressDays_Validating(object sender, CancelEventArgs e)
        {
            var textbox = sender as MetroTextBox;
            var num = 1;
            int.TryParse(Regex.Replace(textbox.Text, @"[^0-9]", ""), out num);
            if (num <= 0)
                num = 1;
            textbox.Text = num.ToString();
        }

        private void toggleCompression_CheckedChanged(object sender, EventArgs e)
        {
            if (toggleCompression.Checked)
            {
                if (!dayChangeDetector.Enabled)
                    dayChangeDetector.Start();
                if (finishInit && MessageBox.Show("既に存在するファイルを圧縮しますか？", "確認", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CompressScrenshot();
                }
            }
            else if (dayChangeDetector.Enabled)
                dayChangeDetector.Stop();
        }

        private void textBoxBorderHour_Validating(object sender, CancelEventArgs e)
        {
            var textbox = sender as MetroTextBox;
            var num = 1;
            int.TryParse(Regex.Replace(textbox.Text, @"[^0-9]", ""), out num);
            if (num <= 0)
                num = 1;
            textbox.Text = num.ToString();
        }

        private void toggleUseDarkMode_CheckedChanged(object sender, EventArgs e)
        {
            if (finishInit)
                MessageBox.Show("ダークモードの切り替えは\nアプリケーションの再起動時に反映されます", "確認", MessageBoxButtons.OK);
        }
    }
}

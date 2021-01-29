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
            if (settingsUseStartup.Checked)
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
        private void settingsFolderLinkSrc_FilePathChanged(object sender, EventArgs e)
        {
            if (finishInit)
            {
                CheckSSFolderIsExist();
                if (!IsSSFolderLinked())
                    return;
                newSSMoverWatcher.Path = settingsFolderLinkSrc.FilePath;
                fileSystemWatcher.Path = GetSSFolderPath();
                currentDirectory = GetSSFolderPath();
                if (MessageBox.Show("元フォルダ内のファイルをリンク先へ移動しますか？", "確認", MessageBoxButtons.YesNo) == DialogResult.Yes)
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
        private void textBoxSSFolder_TextChanged(object sender, EventArgs e)
        {
            if (finishInit)
            {
                CheckSSFolderIsExist();
                if (!IsSSFolderLinked())
                    return;
                newSSMoverWatcher.Path = settingsFolderLinkSrc.FilePath;
                fileSystemWatcher.Path = GetSSFolderPath();
                currentDirectory = GetSSFolderPath();
                if (MessageBox.Show("元フォルダ内のファイルをリンク先へ移動しますか？", "確認", MessageBoxButtons.YesNo) == DialogResult.Yes)
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

        private void toggleObserveSS_CheckedChanged(object sender, EventArgs e)
        {
            if (finishInit)
                fileSystemWatcher.EnableRaisingEvents = settingsObserveSS.Checked;
        }

        private void toggleSortSS_CheckedChanged(object sender, EventArgs e)
        {
            if (finishInit)
            {
                if (settingsSortSS.Checked)
                {
                    if (MessageBox.Show("既に存在するファイルを日付分けしますか？", "確認", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        SortScreenshot();
                    }
                }
            }
        }

        private void toggleCompression_CheckedChanged(object sender, EventArgs e)
        {
            if(finishInit)
            {
                if (settingsUseCompress.Checked)
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
        }

        private void toggleUseDarkMode_CheckedChanged(object sender, EventArgs e)
        {
            if (finishInit)
                MessageBox.Show("ダークモードの切り替えは\nアプリケーションの再起動時に反映されます", "確認", MessageBoxButtons.OK);
        }
    }
}

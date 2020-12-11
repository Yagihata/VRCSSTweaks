using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;

namespace VRCSSTweaks
{
    public partial class vrcsstMainWindow
    {
        private List<FileItem> rawFileItems = new List<FileItem>();
        public SortableFileItems fileItems = new SortableFileItems();
        private int lastSelectedFileIndex = -1;
        private string currentDirectory = "";
        private string currentPreviewImageSHA256 = "";
        private void LoadPreviewImage(string path)
        {
            foreach (var v in tagItems)
                v.IsChecked = false;
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                currentPreviewImageSHA256 = "";
                labelPreviewSSName.Text = "";
                labelPreviewSSDate.Text = "";
                labelPreviewSSSize.Text = "";
                panelScreenshotPreview.BackgroundImage = null;
                textBoxPreviewSSURL.Text = "未検出";
                metroGrid2.Enabled = false;
            }
            else
            {
                metroGrid2.Enabled = true;
                currentPreviewImageSHA256 = GetSHA256(path);
                if (containTagList.ContainsKey(currentPreviewImageSHA256))
                {
                    foreach (var v in containTagList[currentPreviewImageSHA256])
                    {
                        var index = tagItems.Select(n => n.Name).ToList().IndexOf(v);
                        if (index >= 0)
                            tagItems[index].IsChecked = true;
                        else
                        {
                            tagItems.Add(new TagItem() { Name = v, IsChecked = true });
                        }
                    }
                }
                metroGrid2.DataSource = null;
                metroGrid2.DataSource = tagListBindingSource;
                ((CurrencyManager)metroGrid2.BindingContext[tagListBindingSource]).Refresh();
                metroGrid2.Refresh();
                var info = new FileInfo(path);
                labelPreviewSSName.Text = info.Name;
                labelPreviewSSDate.Text = info.LastWriteTime.ToString();
                labelPreviewSSSize.Text = new FileSize(info).ToString();
                if (panelScreenshotPreview.BackgroundImage != null)
                    panelScreenshotPreview.BackgroundImage.Dispose();
                var file = Image.FromStream(info.OpenRead());
                panelScreenshotPreview.BackgroundImage = file;
                if (toggleDetectBarcode.Checked)
                {
                    BarcodeReader reader = new BarcodeReader();
                    Result result = reader.Decode(file as Bitmap);
                    if (result != null)
                    {
                        textBoxPreviewSSURL.Text = result.Text;
                    }
                    else
                        textBoxPreviewSSURL.Text = "未検出";
                }
                else
                    textBoxPreviewSSURL.Text = "バーコード読み取り:無効";
            }
        }
        private void UpdateFileList()
        {
            if (!fileListRefresher.IsBusy)
            {
                var selectedFile = "";
                if (lastSelectedFileIndex >= 0)
                    selectedFile = fileItems[lastSelectedFileIndex].Name;
                lastSelectedFileIndex = -1;
                fileItems.Clear();
                if (metroCheckBox1.Checked && metroComboBox2.SelectedItem != null)
                {
                    foreach (var v in rawFileItems)
                    {
                        if (v.Tags.Contains((metroComboBox2.SelectedItem as TagItem).Name) || v.Name == "...")
                            fileItems.Add(v);
                    }
                }
                else
                    rawFileItems.ForEach(v => fileItems.Add(v));

                metroGrid1.Enabled = true;
                metroGrid1.DataSource = fileItems;
                metroGrid1.ScrollBars = ScrollBars.Both;
                ((CurrencyManager)metroGrid1.BindingContext[fileItems]).Refresh();
                metroGrid1.Columns[0].Frozen = true;
                metroGrid1.Refresh();

                if (!string.IsNullOrEmpty(selectedFile))
                {
                    var item = fileItems.FirstOrDefault(n => n.Name == selectedFile);
                    var index = -1;
                    if(item != null)
                    {
                        index = fileItems.IndexOf(item);
                    }
                    if (index >= 0)
                    {
                        metroGrid1.Rows[index].Cells[0].Selected = true;
                        metroGrid1_SelectionChanged(metroGrid1, null);
                    }
                }
            }
        }
        private void PoseFileList()
        {
            metroGrid1.DataSource = null;
            metroGrid1.Rows.Clear();
            metroGrid1.ScrollBars = ScrollBars.None;
            metroGrid1.Refresh();
            metroGrid1.Enabled = false;
        }
        private void fileListRefresher_DoWork(object sender, DoWorkEventArgs e)
        {
            metroGrid1.Invoke((MethodInvoker)(() =>
            {
                PoseFileList();
            }));
            rawFileItems.Clear();
            var directories = Directory.GetDirectories(currentDirectory);
            foreach (var path in directories)
            {
                var info = new DirectoryInfo(path);
                var fileItem = new FileItem();
                fileItem.Name = info.Name;
                fileItem.Size = new FileSize(-1);
                fileItem.Date = info.LastWriteTime.ToString();
                fileItem.AddTag("Folder");
                rawFileItems.Add(fileItem);
            }
            var files = Directory.GetFiles(currentDirectory, "*.png");
            foreach (var path in files)
            {
                var info = new FileInfo(path);
                var fileItem = new FileItem();
                fileItem.Name = info.Name;
                fileItem.Size = new FileSize(info);
                fileItem.Date = info.LastWriteTime.ToString();
                var sha256 = GetSHA256(path);
                if (containTagList.ContainsKey(sha256))
                {
                    containTagList[sha256].ForEach(n => fileItem.AddTag(n));
                }
                rawFileItems.Add(fileItem);
            }
        }

        private void fileListRefresher_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (currentDirectory != ssFolderPath.Text)
            {
                var item = new FileItem() { Name = "...", Date = "", Size = new FileSize(-2) };
                item.AddTag("Folder");
                rawFileItems.Insert(0, item);
            }
            UpdateFileList();
        }

        private void metroGrid1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && metroGrid1.Rows[e.RowIndex].Cells[3] != null)
            {
                var tags = metroGrid1.Rows[e.RowIndex].Cells[3].Value.ToString().Split(',');
                if (tags.Contains("Folder"))
                {
                    if (metroGrid1.Rows[e.RowIndex].Cells[0].Value.ToString() == "...")
                    {
                        var pathList = currentDirectory.TrimEnd('\\').Split('\\').ToList();
                        pathList.RemoveAt(pathList.Count - 1);
                        currentDirectory = string.Join("\\", pathList);
                    }
                    else
                    {
                        currentDirectory += "\\" + metroGrid1.Rows[e.RowIndex].Cells[0].Value.ToString();
                    }
                    fileListRefresher.RunWorkerAsync();
                }
            }
        }
        private void metroGrid1_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            DataGridViewCell cell = dgv.CurrentCell;
            if (cell == null)
            {
                lastSelectedFileIndex = -1;
                LoadPreviewImage(null);
                return;
            }
            lastSelectedFileIndex = cell.RowIndex;
            if (metroGrid1.Rows[cell.RowIndex].Cells[3] != null)
            {
                var tags = metroGrid1.Rows[cell.RowIndex].Cells[3].Value.ToString().Split(',');
                if (tags.Contains("Folder"))
                {
                    LoadPreviewImage(null);
                    return;
                }
            }
            if (metroGrid1.Rows[cell.RowIndex].Cells[0] != null)
                LoadPreviewImage(currentDirectory + "\\" + metroGrid1.Rows[cell.RowIndex].Cells[0].Value.ToString());
        }

        private void metroButton4_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBoxPreviewSSURL.Text);
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            var txt = textBoxPreviewSSURL.Text;
            if (IsUrl(txt))
                Process.Start(txt);
        }

        private void metroButton5_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxAddTagName.Text) && textBoxAddTagName.Text != "Folder")
            {
                var tagName = ValidateTagName(textBoxAddTagName.Text);
                if (tagItems.Any(n => n.Name == tagName))
                {
                    var index = 1;
                    while (tagItems.Any(n => n.Name == tagName + index.ToString()))
                        ++index;
                    tagItems.Add(new TagItem() { Name = tagName + index.ToString(), IsChecked = false });
                }
                else
                    tagItems.Add(new TagItem() { Name = tagName, IsChecked = false });
                ((CurrencyManager)metroGrid2.BindingContext[tagItems]).Refresh();
            }
            textBoxAddTagName.Text = "";
        }


        private void metroGrid2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (tagItems.Count > 0 && e.RowIndex < metroGrid2.RowCount)
            {
                if (e.ColumnIndex == 0)
                {
                    var cell = metroGrid2.Rows[e.RowIndex].Cells[0];
                    if (cell.Value == null)
                        cell.Value = "";
                    var rowName = ValidateTagName(cell.Value.ToString());
                    if (string.IsNullOrEmpty(rowName) || tagItems.Count(n => n.Name == rowName) > 1)
                    {
                        var index = 1;
                        var name = rowName;
                        if (string.IsNullOrEmpty(name))
                            name = "新しいタグ";
                        while (tagItems.Any(n => n.Name == name + index.ToString()))
                            ++index;
                        cell.Value = ValidateTagName(name) + index.ToString();
                    }
                    else if (rowName.Contains(','))
                        cell.Value = ValidateTagName(rowName);
                    ((CurrencyManager)metroGrid2.BindingContext[tagItems]).Refresh();
                }
                else if (e.ColumnIndex == 1)
                {
                    if (!containTagList.ContainsKey(currentPreviewImageSHA256))
                        containTagList.Add(currentPreviewImageSHA256, new List<string>());
                    var name = metroGrid2.Rows[e.RowIndex].Cells[0];
                    var check = (bool)(metroGrid2.Rows[e.RowIndex].Cells[1] as DataGridViewCheckBoxCell).Value;
                    if (name != null && !string.IsNullOrEmpty(name.Value.ToString()))
                    {
                        var nameStr = name.Value.ToString();
                        var list = containTagList[currentPreviewImageSHA256];
                        if (check)
                        {
                            if (!list.Contains(nameStr))
                                list.Add(nameStr);
                            rawFileItems.First(n => n.Name == labelPreviewSSName.Text).AddTag(nameStr);
                        }
                        else if (!check)
                        {
                            if (list.Contains(nameStr))
                                list.Remove(nameStr);
                            rawFileItems.First(n => n.Name == labelPreviewSSName.Text).RemoveTag(nameStr);
                        }
                        //((CurrencyManager)metroGrid1.BindingContext[fileItems]).Refresh();
                        metroGrid1.Refresh();
                    }
                }
            }
        }

        private void metroGrid2_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
                (sender as DataGridView).CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void metroGrid2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
                (sender as DataGridView).CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
        private void metroGrid2_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void metroGrid2_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                    metroGrid2.Rows[e.RowIndex].Selected = true;
                if (metroGrid2.SelectedRows != null)
                    metroContextMenu1.Show(System.Windows.Forms.Cursor.Position);
            }
        }

        private void タグを削除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var rowIndex = metroGrid2.Rows.IndexOf(metroGrid2.SelectedRows[0]);
            var name = tagItems[rowIndex].Name;
            tagItems.RemoveAt(rowIndex);
            containTagList.Values.ToList().ForEach(n => n.Remove(name));
            rawFileItems.ToList().ForEach(n => n.RemoveTag(name));
            PoseFileList();
            UpdateFileList();
            ((CurrencyManager)metroGrid2.BindingContext[tagItems]).Refresh();
            ((CurrencyManager)metroGrid1.BindingContext[fileItems]).Refresh();
        }

        private void metroComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (metroCheckBox1.Checked)
            {
                PoseFileList();
                UpdateFileList();
            }
        }
        private string ValidateTagName(string rawName)
        {
            foreach (var v in new string[] { ",", "&", "|" })
                rawName = rawName.Replace(v, "");
            return rawName;
        }

        private void metroCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            PoseFileList();
            UpdateFileList();
        }
    }
}

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
        private List<FileItem> tempFileItems = null;
        public SortableFileItems fileItems = null;
        private int lastSelectedFileIndex = -1;
        private string currentDirectory = "";
        private string currentPreviewImagePath = "";
        private void ClearTagsData()
        {
            foreach (var v in tagItems)
                v.IsChecked = false;
        }
        private void LoadTagsData(string key)
        {
            foreach (var v in tagItems)
                v.IsChecked = false;
            if (containTagList.ContainsKey(key))
            {
                foreach (var v in containTagList[key].TagItems)
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
        }
        private string[] GetSelectedItemsPath(bool fileOnly = false)
        {
            var rows = metroGrid1.SelectedRows.Cast<DataGridViewRow>();
            var modif = "";
            if (!metroCheckBox1.Checked)
                modif = currentDirectory + "\\";
            if (rows != null && rows.Count() > 0)
            {
                var items = rows.Where(n => n.Cells[0] != null && n.Cells[0].Value != null && 
                (!fileOnly || n.Cells[3] == null || n.Cells[3].Value == null || !n.Cells[3].Value.ToString().Split(',').Contains("Folder")))
                    .Select(n => n.Cells[0].Value.ToString())
                    .Where(n => !string.IsNullOrEmpty(n) && n != "...");
                if (items.Count() > 0)
                    return items.Select(n => modif + n).ToArray();
            }
            return new string[] { };
        }
        private void RefreshFileListTags()
        {
            foreach(var file in fileItems)
            {
                var isFolder = file.Tags.Contains("Folder");
                file.Tags.Clear();
                if (isFolder)
                    file.Tags.Add("Folder");
                
                var sha256 = "";
                if (metroCheckBox1.Checked)
                    sha256 = GetSHA256(file.Name);
                else
                    sha256 = GetSHA256(currentDirectory + "\\" + file.Name);
                if (containTagList.ContainsKey(sha256))
                {
                    foreach (var v in containTagList[sha256].TagItems)
                        file.AddTag(v);
                }
            }
        }
        private void LoadPreviewImage(string path)
        {
            ClearTagsData();
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                currentPreviewImagePath = "";
                labelPreviewSSName.Text = "";
                labelPreviewSSDate.Text = "";
                labelPreviewSSSize.Text = "";
                panelScreenshotPreview.BackgroundImage = null;
                textBoxPreviewSSURL.Text = "未検出";
                gridPRSSTagList.Enabled = false;
            }
            else
            {
                gridPRSSTagList.Enabled = true;
                currentPreviewImagePath = path;
                LoadTagsData(GetSHA256(path));
                gridPRSSTagList.DataSource = null;
                gridPRSSTagList.DataSource = tagListBindingSource;
                ((CurrencyManager)gridPRSSTagList.BindingContext[tagListBindingSource]).Refresh();
                gridPRSSTagList.Refresh();
                var info = new FileInfo(path);
                labelPreviewSSName.Text = info.Name;
                labelPreviewSSDate.Text = info.LastWriteTime.ToString();
                labelPreviewSSSize.Text = new FileSize(info).ToString();
                if (panelScreenshotPreview.BackgroundImage != null)
                    panelScreenshotPreview.BackgroundImage.Dispose();
                var file = CreateImage(path);
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
                var flag = false;
                if(fileItems == null)
                {
                    flag = true;
                    fileItems = new SortableFileItems();
                }
                var selectedFile = "";
                if (lastSelectedFileIndex >= 0)
                    selectedFile = fileItems[lastSelectedFileIndex].Name;
                lastSelectedFileIndex = -1;
                fileItems.Clear();
                if (tempFileItems != null)
                {
                    rawFileItems = tempFileItems;
                    tempFileItems = null;
                }
                if (metroCheckBox1.Checked && metroComboBox2.SelectedItem != null)
                {
                    tempFileItems = rawFileItems;
                    rawFileItems = new List<FileItem>();
                    var tagContainFiles = new Dictionary<string, string>();
                    var selectedTag = (metroComboBox2.SelectedItem as TagItem).Name;
                    List<string> missingFiles = null;

                    foreach(var v in containTagList.Where(n => n.Value.TagItems.Contains(selectedTag)))
                    {
                        var path = v.Value.FilePath;
                        if (File.Exists(path))
                        {
                            var info = new FileInfo(path);
                            var fileItem = new FileItem();
                            fileItem.Name = path;
                            fileItem.Size = new FileSize(info);
                            fileItem.Date = info.LastWriteTime.ToString();
                            fileItem.Tags.AddRange(v.Value.TagItems);
                            rawFileItems.Add(fileItem);
                        }
                        else
                        {
                            if (missingFiles == null)
                                missingFiles = new List<string>();

                            missingFiles.Add(v.Key);
                        }
                    }
                    if (missingFiles != null)
                    {
                        foreach (var v in missingFiles)
                            containTagList.Remove(v);
                    }
                }
                foreach (var v in rawFileItems)
                    fileItems.Add(v);
                metroGrid1.Enabled = true;
                metroGrid1.DataSource = fileItems;
                metroGrid1.ScrollBars = ScrollBars.Both;
                ((CurrencyManager)metroGrid1.BindingContext[fileItems]).Refresh();
                metroGrid1.Columns[0].Frozen = true;
                metroGrid1.Refresh();
                if (flag)
                    metroGrid1.Sort(metroGrid1.Columns[0], ListSortDirection.Descending);
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
                SortRows(metroGrid1.SortedColumn, false);
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
            GetFileItemsList().Clear();
            var directories = Directory.GetDirectories(currentDirectory);
            foreach (var path in directories)
            {
                var info = new DirectoryInfo(path);
                var fileItem = new FileItem();
                fileItem.Name = info.Name;
                fileItem.Size = new FileSize(-1);
                fileItem.Date = info.LastWriteTime.ToString();
                fileItem.AddTag("Folder");
                GetFileItemsList().Add(fileItem);
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
                    foreach (var v in containTagList[sha256].TagItems)
                        fileItem.AddTag(v);
                }
                GetFileItemsList().Add(fileItem);
            }
        }

        private void fileListRefresher_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (currentDirectory != GetSSFolderPath())
            {
                var item = new FileItem() { Name = "...", Date = "", Size = new FileSize(-2) };
                item.AddTag("Folder");
                GetFileItemsList().Insert(0, item);
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
                else
                {
                    foreach (var v in GetSelectedItemsPath(true))
                        RegisterToImageQueue(v);
                }
            }
        }
        private void metroGrid1_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            DataGridViewCell cell = dgv.CurrentCell;
            var count = GetSelectedItemsPath(true).Count();
            if (count == 1)
            {
                gridPRSSTagList.Columns[1].Visible = true;
                gridPRSSTagList.Columns[2].Visible = false;
                gridPRSSTagList.Columns[3].Visible = false;
            }
            else if (count > 1)
            {
                gridPRSSTagList.Columns[1].Visible = false;
                gridPRSSTagList.Columns[2].Visible = true;
                gridPRSSTagList.Columns[3].Visible = true;
            }
            else
            {
                gridPRSSTagList.Columns[1].Visible = false;
                gridPRSSTagList.Columns[2].Visible = false;
                gridPRSSTagList.Columns[3].Visible = false;
            }
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
                LoadPreviewImage(GetSelectedItemsPath().FirstOrDefault());
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
            if (!string.IsNullOrEmpty(textBoxPRSSAddTagName.Text) && textBoxPRSSAddTagName.Text != "Folder")
            {
                var tagName = ValidateTagName(textBoxPRSSAddTagName.Text);
                if (tagItems.Any(n => n.Name == tagName))
                {
                    var index = 1;
                    while (tagItems.Any(n => n.Name == tagName + index.ToString()))
                        ++index;
                    tagItems.Add(new TagItem() { Name = tagName + index.ToString(), IsChecked = false });
                }
                else
                    tagItems.Add(new TagItem() { Name = tagName, IsChecked = false });
                ((CurrencyManager)gridPRSSTagList.BindingContext[tagItems]).Refresh();
            }
            textBoxPRSSAddTagName.Text = "";
        }


        private void metroGrid2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (windowTabControl.SelectedIndex == 1)
            {
                if (tagItems.Count > 0 && e.RowIndex < gridPRSSTagList.RowCount)
                {
                    if (e.ColumnIndex == 0)
                    {
                        var cell = gridPRSSTagList.Rows[e.RowIndex].Cells[0];
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
                        ((CurrencyManager)gridPRSSTagList.BindingContext[tagItems]).Refresh();
                    }
                    else if (e.ColumnIndex == 1)
                    {
                        var sha = GetSHA256(currentPreviewImagePath);
                        if (!containTagList.ContainsKey(sha))
                            containTagList.Add(sha, new FilePathTagList() { FilePath = currentPreviewImagePath });
                        var name = gridPRSSTagList.Rows[e.RowIndex].Cells[0];
                        var check = (bool)(gridPRSSTagList.Rows[e.RowIndex].Cells[1] as DataGridViewCheckBoxCell).Value;
                        var filelist = GetFileItemsList();
                        if (name != null && !string.IsNullOrEmpty(name.Value.ToString()))
                        {
                            var nameStr = name.Value.ToString();
                            var list = containTagList[sha];
                            if (check)
                            {
                                if (!list.TagItems.Contains(nameStr))
                                    list.TagItems.Add(nameStr);
                                filelist.First(n => n.Name == labelPreviewSSName.Text).AddTag(nameStr);
                            }
                            else if (!check)
                            {
                                if (list.TagItems.Contains(nameStr))
                                    list.TagItems.Remove(nameStr);
                                filelist.First(n => n.Name == labelPreviewSSName.Text).RemoveTag(nameStr);
                            }
                            metroGrid1.Refresh();
                        }
                    }
                }
            }
        }

        private void metroGrid2_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (windowTabControl.SelectedIndex == 1)
                if (e.ColumnIndex == 1)
                (sender as DataGridView).CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void metroGrid2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (windowTabControl.SelectedIndex == 1)
            {
                if (e.ColumnIndex == 1)
                    (sender as DataGridView).CommitEdit(DataGridViewDataErrorContexts.Commit);
                else if(e.ColumnIndex == 2 || e.ColumnIndex == 3)
                {
                    var files = GetSelectedItemsPath(true);
                    var check = e.ColumnIndex == 2;
                    var tagFilterMode = metroCheckBox1.Checked;
                    foreach (var file in files)
                    {
                        var sha = GetSHA256(file);
                        if (!containTagList.ContainsKey(sha))
                            containTagList.Add(sha, new FilePathTagList() { FilePath = file });
                        var name = gridPRSSTagList.Rows[e.RowIndex].Cells[0];
                        if (name != null && !string.IsNullOrEmpty(name.Value.ToString()))
                        {
                            var nameStr = name.Value.ToString();
                            var list = containTagList[sha];
                            var fileName = Path.GetFileName(file);
                            if (check)
                            {
                                if (!list.TagItems.Contains(nameStr))
                                    list.TagItems.Add(nameStr);
                                if (!tagFilterMode)
                                    GetFileItemsList().First(n => n.Name == fileName).AddTag(nameStr);
                                else
                                {
                                    rawFileItems.Find(n => n.Name == file)?.AddTag(nameStr);
                                    tempFileItems.Find(n => n.Name == fileName)?.AddTag(nameStr);
                                }
                            }
                            else if (!check)
                            {
                                if (list.TagItems.Contains(nameStr))
                                    list.TagItems.Remove(nameStr);
                                if (!tagFilterMode)
                                    GetFileItemsList().First(n => n.Name == fileName).RemoveTag(nameStr);
                                else
                                {
                                    rawFileItems.Find(n => n.Name == file)?.RemoveTag(nameStr);
                                    tempFileItems.Find(n => n.Name == fileName)?.RemoveTag(nameStr);
                                }
                            }
                        }
                    }
                    metroGrid1.Refresh();
                }
            }
        }

        private void metroGrid2_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (windowTabControl.SelectedIndex == 1)
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                        gridPRSSTagList.Rows[e.RowIndex].Selected = true;
                    if (gridPRSSTagList.SelectedRows != null)
                        metroContextMenu1.Show(System.Windows.Forms.Cursor.Position);
                }
            }
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
        private void metroButton10_Click(object sender, EventArgs e)
        {
            var name = GetSelectedItemsPath().FirstOrDefault();
            if (!string.IsNullOrEmpty(name))
                RegisterToImageQueue(name);
        }

        private void エクスプローラで開くToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var v in GetSelectedItemsPath())
            {
                var arg = string.Format(@"/select,""{0}""", v);
                Process.Start("EXPLORER.EXE", arg);
            }
        }

        private void コピーToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] fileNames = GetSelectedItemsPath();
            if (fileNames.Any())
            {
                IDataObject data = new DataObject(DataFormats.FileDrop, fileNames);
                byte[] bs = new byte[] { (byte)DragDropEffects.Copy, 0, 0, 0 };
                MemoryStream ms = new MemoryStream(bs);
                data.SetData("Preferred DropEffect", ms);
                Clipboard.SetDataObject(data);

            }
        }

        private void 削除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var items = GetSelectedItemsPath();
                if (items.Any())
                {
                    var message = "選択したアイテムを削除します";
                    if (MessageBox.Show(message, "確認", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        foreach (var path in items)
                        {
                            if (IsFolder(path))
                            {
                                CleanUp(path);
                            }
                            else
                            {
                                File.Delete(path);
                            }
                            var name = path.Split('\\').Last();
                            if(metroCheckBox1.Checked)
                            {
                                var index = rawFileItems.IndexOf(rawFileItems.FirstOrDefault(n => n.Name == path));
                                if (index >= 0)
                                    rawFileItems.RemoveAt(index);

                                index = tempFileItems.IndexOf(tempFileItems.FirstOrDefault(n => n.Name == Path.GetFileName(path)));
                                if (index >= 0)
                                    tempFileItems.RemoveAt(index);
                            }
                            else
                            {
                                var index = GetFileItemsList().IndexOf(GetFileItemsList().FirstOrDefault(n => n.Name == name));
                                if (index >= 0)
                                    GetFileItemsList().RemoveAt(index);
                            }
                        }
                        UpdateFileList();
                    }
                }
            }
            catch
            {

            }
        }

        private void ツイートキューに追加ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var path in GetSelectedItemsPath(true))
                RegisterToImageQueue(path);
        }

        private void 切り取りToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] fileNames = GetSelectedItemsPath();
            if (fileNames.Any())
            {
                IDataObject data = new DataObject(DataFormats.FileDrop, fileNames);
                byte[] bs = new byte[] { (byte)DragDropEffects.Move, 0, 0, 0 };
                MemoryStream ms = new MemoryStream(bs);
                data.SetData("Preferred DropEffect", ms);
                Clipboard.SetDataObject(data);
            }
        }

        private void 貼り付けToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var items = GetSelectedItemsPath();
                if (items.Any())
                {
                    var path = items.First();
                    if (!IsFolder(path))
                        path = Path.GetDirectoryName(path);
                    PasteFile(path);


                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void エクスプローラで開くToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Process.Start(currentDirectory);
        }

        private void 貼り付けToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                PasteFile(currentDirectory);
            }
            catch
            {

            }
        }

        private void metroGrid1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                metroContextMenu5.Show(System.Windows.Forms.Cursor.Position);
        }

        private void PasteFile(string path)
        {
            var type = PasteFiles(path, out string[] destPath, out string[] sourcePath);
            if (type != 0)
            {
                foreach (var v in Enumerable.Range(0, destPath.Length))
                {
                    var dest = destPath[v];
                    var src = sourcePath[v];
                    var sha = GetSHA256(src);
                    var withTags = containTagList.ContainsKey(sha);
                    string[] tagData = null;
                    if (withTags)
                        tagData = containTagList[sha].TagItems.ToArray();
                    if (type == 2)
                    {
                        if (Path.GetDirectoryName(src) == currentDirectory)
                        {
                            var fileName = Path.GetFileName(src);
                            var index = GetFileItemsList().IndexOf(fileItems.FirstOrDefault(n => n.Name == fileName));
                            if (index >= 0)
                                GetFileItemsList().RemoveAt(index);
                            if (metroCheckBox1.Checked)
                            {
                                index = rawFileItems.IndexOf(fileItems.FirstOrDefault(n => n.Name == fileName));
                                if (index >= 0)
                                    rawFileItems.RemoveAt(index);
                            }
                            UpdateFileList();
                        }
                        if (withTags)
                            containTagList.Remove(sha);
                    }
                    if (dest.Contains(GetSSFolderPath()))
                    {
                        var newSHA = GetSHA256(dest);
                        if (tagData != null)
                        {
                            if (containTagList.ContainsKey(newSHA))
                                containTagList.Add(newSHA, new FilePathTagList() { TagItems = tagData.ToList(), FilePath = dest });
                            else
                                containTagList[newSHA].TagItems = tagData.ToList();
                        }
                        if (Path.GetDirectoryName(dest) == currentDirectory)
                        {
                            if (File.GetAttributes(dest).HasFlag(FileAttributes.Directory))
                            {
                                var dirInfo = new DirectoryInfo(dest);
                                var fileItem = new FileItem();
                                fileItem.Name = dirInfo.Name;
                                fileItem.Size = new FileSize(-1);
                                fileItem.Date = dirInfo.LastWriteTime.ToString();
                                fileItem.AddTag("Folder");
                                GetFileItemsList().Add(fileItem);
                            }
                            else
                            {
                                var info = new FileInfo(dest);
                                var fileItem = new FileItem();
                                fileItem.Name = info.Name;
                                fileItem.Size = new FileSize(info);
                                fileItem.Date = info.LastWriteTime.ToString();
                                if (containTagList.ContainsKey(newSHA))
                                    foreach (var tag in containTagList[newSHA].TagItems)
                                        fileItem.AddTag(tag);
                                GetFileItemsList().Add(fileItem);
                            }
                            UpdateFileList();
                        }
                    }
                }
            }
        }
        private void metroGrid1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (windowTabControl.SelectedIndex == 1)
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                        metroGrid1.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;

                    if (metroGrid1.SelectedRows != null)
                        metroContextMenu4.Show(System.Windows.Forms.Cursor.Position);
                }
            }
        }
        private void CleanUp(string targetDirectoryPath)
        {
            if (Directory.Exists(targetDirectoryPath))
            {
                string[] filePaths = Directory.GetFiles(targetDirectoryPath);
                foreach (string filePath in filePaths)
                {
                    File.SetAttributes(filePath, FileAttributes.Normal);
                    File.Delete(filePath);
                }

                string[] directoryPaths = Directory.GetDirectories(targetDirectoryPath);
                foreach (string directoryPath in directoryPaths)
                {
                    CleanUp(directoryPath);
                }
            }

            Directory.Delete(targetDirectoryPath, false);
        }
        public int PasteFiles(string destDir, out string[] destFilePath, out string[] files)
        {
            int type = 0;
            destFilePath = null;
            files = null;
            IDataObject data = Clipboard.GetDataObject();
            if (data != null && data.GetDataPresent(DataFormats.FileDrop))
            {
                files = (string[])data.GetData(DataFormats.FileDrop);
                DragDropEffects dde = GetPreferredDropEffect(data);
                Console.WriteLine(dde);
                if (dde == DragDropEffects.Copy || dde == DragDropEffects.Link || dde == (DragDropEffects.Copy | DragDropEffects.Link))
                {
                    destFilePath = CopyFilesToDirectory(files, destDir, false);
                    type = 1;
                }
                else if (dde == DragDropEffects.Move)
                {
                    destFilePath = CopyFilesToDirectory(files, destDir, true);
                    type = 2;
                }
            }
            return type;
        }
        public DragDropEffects GetPreferredDropEffect(IDataObject data)
        {
            DragDropEffects dde = DragDropEffects.None;

            if (data != null)
            {
                MemoryStream ms = (MemoryStream)data.GetData("Preferred DropEffect");
                if (ms != null)
                    dde = (DragDropEffects)ms.ReadByte();
            }

            return dde;
        }
        public string[] CopyFilesToDirectory(string[] sourceFiles, string destDir, bool move)
        {
            var destPaths = new string[sourceFiles.Length];
            foreach (var v in Enumerable.Range(0, sourceFiles.Length))
            {
                var sourcePath = sourceFiles[v];
                string destName = Path.GetFileName(sourcePath);
                var splittedName = destName.Split('.');
                string extension = "";
                if (splittedName.Count() >= 2)
                    extension = "." + splittedName[splittedName.Count() - 1];
                var nameExtRemoved = Path.GetFileNameWithoutExtension(sourcePath);
                string destPath = Path.Combine(destDir, destName);
                int index = 0;
                while (File.Exists(destPath))
                {
                    ++index;
                    destPath = Path.Combine(destDir, string.Format("{0} ({1}){2}", nameExtRemoved, index, extension));
                }
                if (!move)
                {
                    File.Copy(sourcePath, destPath);
                }
                else
                {
                    File.Move(sourcePath, destPath);
                }
                destPaths[v] = destPath;
            }
            return destPaths;
        }
        private void SortRows(DataGridViewColumn sortColumn, bool orderToggle)
        {
            if (sortColumn == null)
                return;

            if (sortColumn.SortMode == DataGridViewColumnSortMode.Programmatic &&
                metroGrid1.SortedColumn != null &&
                !metroGrid1.SortedColumn.Equals(sortColumn))
            {
                metroGrid1.SortedColumn.HeaderCell.SortGlyphDirection =
                    SortOrder.None;
            }

            ListSortDirection sortDirection;
            if (orderToggle)
            {
                sortDirection =
                    metroGrid1.SortOrder == SortOrder.Descending ?
                    ListSortDirection.Ascending : ListSortDirection.Descending;
            }
            else
            {
                sortDirection =
                    metroGrid1.SortOrder == SortOrder.Descending ?
                    ListSortDirection.Descending : ListSortDirection.Ascending;
            }
            SortOrder sortOrder =
                sortDirection == ListSortDirection.Ascending ?
                SortOrder.Ascending : SortOrder.Descending;

            metroGrid1.Sort(sortColumn, sortDirection);

            if (sortColumn.SortMode == DataGridViewColumnSortMode.Programmatic)
            {
                sortColumn.HeaderCell.SortGlyphDirection = sortOrder;
            }
        }
        private bool IsFolder(string path)
        {
            return GetAttr(path) == "Directory";
        }

        private string GetAttr(string path)
        {
            FileAttributes attr;
            string result;
            try
            {
                attr = File.GetAttributes(path);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    result = "Directory";
                }
                else if ((attr & FileAttributes.Archive) == FileAttributes.Archive)
                {
                    result = "Archive";
                }
                else if ((attr & FileAttributes.Normal) == FileAttributes.Normal)
                {
                    result = "File";
                }
                else
                {
                    result = "Unknown";
                }
            }
            catch (Exception)
            {
                result = "Error";
            }

            return result;
        }
    }
}

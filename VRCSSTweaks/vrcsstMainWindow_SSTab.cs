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
        public SortableFileItems fileItems = null;
        private int lastSelectedFileIndex = -1;
        private string currentDirectory = "";
        private string currentPreviewImageSHA256 = "";
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
                foreach (var v in containTagList[key])
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
        private void RefreshFileListTags()
        {
            foreach(var file in fileItems)
            {
                var isFolder = file.Tags.Contains("Folder");
                file.Tags.Clear();
                if (isFolder)
                    file.Tags.Add("Folder");
                var sha256 = GetSHA256(currentDirectory + "\\" + file.Name);
                if (containTagList.ContainsKey(sha256))
                {
                    foreach (var v in containTagList[sha256])
                        file.AddTag(v);
                }
            }
        }
        private void LoadPreviewImage(string path)
        {
            ClearTagsData();
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                currentPreviewImageSHA256 = "";
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
                currentPreviewImageSHA256 = GetSHA256(path);
                LoadTagsData(currentPreviewImageSHA256);
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
                if (metroCheckBox1.Checked && metroComboBox2.SelectedItem != null)
                {
                    foreach (var v in rawFileItems)
                    {
                        if (v.Tags.Contains((metroComboBox2.SelectedItem as TagItem).Name) || v.Name == "...")
                            fileItems.Add(v);
                    }
                }
                else
                {
                    foreach (var v in rawFileItems)
                        fileItems.Add(v);
                }

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
                    foreach (var v in containTagList[sha256])
                        fileItem.AddTag(v);
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
                else
                {
                    RegisterToImageQueue(currentDirectory + "\\" + metroGrid1.Rows[e.RowIndex].Cells[0].Value.ToString());
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
                        if (!containTagList.ContainsKey(currentPreviewImageSHA256))
                            containTagList.Add(currentPreviewImageSHA256, new List<string>());
                        var name = gridPRSSTagList.Rows[e.RowIndex].Cells[0];
                        var check = (bool)(gridPRSSTagList.Rows[e.RowIndex].Cells[1] as DataGridViewCheckBoxCell).Value;
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
                if (e.ColumnIndex == 1)
                (sender as DataGridView).CommitEdit(DataGridViewDataErrorContexts.Commit);
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
            if (!string.IsNullOrEmpty(labelPreviewSSName.Text))
                RegisterToImageQueue(currentDirectory + "\\" + labelPreviewSSName.Text);
        }

        private void エクスプローラで開くToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var cell = metroGrid1.CurrentCell;
            if (metroGrid1.SelectedRows != null)
            {
                var arg = string.Format(@"/select,""{0}\{1}""", currentDirectory, metroGrid1.Rows[cell.RowIndex].Cells[0].Value.ToString());
                Process.Start("EXPLORER.EXE", arg);
            }
        }

        private void コピーToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var cell = metroGrid1.CurrentCell;
            if (metroGrid1.SelectedRows != null)
            {
                string[] fileNames = { string.Format(@"{0}\{1}", currentDirectory, metroGrid1.Rows[cell.RowIndex].Cells[0].Value.ToString()) };
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
                var cell = metroGrid1.CurrentCell;
                if (metroGrid1.SelectedRows != null)
                {
                    var name = metroGrid1.Rows[cell.RowIndex].Cells[0].Value.ToString();
                    var path = string.Format(@"{0}\{1}", currentDirectory, name);
                    var isFolder = metroGrid1.Rows[cell.RowIndex].Cells[3].Value.ToString().Split(',').Contains("Folder");
                    var message = "選択した" + (isFolder ? "フォルダ" : "ファイル") + "を削除します";
                    if (MessageBox.Show(message, "確認", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        if (isFolder)
                        {
                            CleanUp(path);
                        }
                        else
                        {
                            File.Delete(path);
                        }
                        var index = rawFileItems.IndexOf(fileItems.FirstOrDefault(n => n.Name == name));
                        if (index >= 0)
                            rawFileItems.RemoveAt(index);
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
            var cell = metroGrid1.CurrentCell;
            if (metroGrid1.SelectedRows != null)
            {
                var name = metroGrid1.Rows[cell.RowIndex].Cells[0].Value.ToString();
                var path = string.Format(@"{0}\{1}", currentDirectory, name);
                var isFolder = metroGrid1.Rows[cell.RowIndex].Cells[3].Value.ToString().Split(',').Contains("Folder");
                if (isFolder)
                    MessageBox.Show("フォルダをキューに追加することは出来ません。");
                else
                    RegisterToImageQueue(path);
            }
        }

        private void 切り取りToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var cell = metroGrid1.CurrentCell;
            if (metroGrid1.SelectedRows != null)
            {
                var name = metroGrid1.Rows[cell.RowIndex].Cells[0].Value.ToString();
                string[] fileNames = { string.Format(@"{0}\{1}", currentDirectory, name) };
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
                var cell = metroGrid1.CurrentCell;
                if (metroGrid1.SelectedRows != null)
                {
                    var name = metroGrid1.Rows[cell.RowIndex].Cells[0].Value.ToString();
                    var path = string.Format(@"{0}\{1}", currentDirectory, name);
                    var isFolder = metroGrid1.Rows[cell.RowIndex].Cells[3].Value.ToString().Split(',').Contains("Folder");
                    if (!isFolder)
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
                        tagData = containTagList[sha].ToArray();
                    if (type == 2)
                    {
                        if (Path.GetDirectoryName(src) == currentDirectory)
                        {
                            var fileName = Path.GetFileName(src);
                            var index = rawFileItems.IndexOf(fileItems.FirstOrDefault(n => n.Name == fileName));
                            if (index >= 0)
                                rawFileItems.RemoveAt(index);
                            UpdateFileList();
                        }
                        if (withTags)
                            containTagList.Remove(sha);
                    }
                    if (dest.Contains(ssFolderPath.Text))
                    {
                        var newSHA = GetSHA256(dest);
                        if (tagData != null)
                        {
                            if (containTagList.ContainsKey(newSHA))
                                containTagList.Add(newSHA, tagData.ToList());
                            else
                                containTagList[newSHA] = tagData.ToList();
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
                                rawFileItems.Add(fileItem);
                            }
                            else
                            {
                                var info = new FileInfo(dest);
                                var fileItem = new FileItem();
                                fileItem.Name = info.Name;
                                fileItem.Size = new FileSize(info);
                                fileItem.Date = info.LastWriteTime.ToString();
                                if (containTagList.ContainsKey(newSHA))
                                    foreach (var tag in containTagList[newSHA])
                                        fileItem.AddTag(tag);
                                rawFileItems.Add(fileItem);
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
    }
}

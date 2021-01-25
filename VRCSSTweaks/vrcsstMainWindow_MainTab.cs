using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using ZXing;
using Clipboard = System.Windows.Forms.Clipboard;

namespace VRCSSTweaks
{
    public partial class vrcsstMainWindow
    {
        private void LoadRecentlyImage(string path, bool openBarcode = true)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                labelRecentlySSName.Text = "";
                labelRecentlySSDate.Text = "";
                labelRecentlySSSize.Text = "";
                panelNewScreenshot.BackgroundImage = null;
                textBoxRecentlySSURL.Text = "未検出";
                gridNSSTagList.Enabled = false;
            }
            else
            {
                gridNSSTagList.Enabled = true;
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


        private void metroButton7_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxNSSTagName.Text) && textBoxNSSTagName.Text != "Folder")
            {
                var tagName = ValidateTagName(textBoxNSSTagName.Text);
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
                ((CurrencyManager)gridNSSTagList.BindingContext[tagItems]).Refresh();
            }
            textBoxNSSTagName.Text = "";
        }

        private void gridNSSTagList_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            if (windowTabControl.SelectedIndex == 0)
            {
                if (tagItems.Count > 0 && e.RowIndex < gridNSSTagList.RowCount)
                {
                    if (e.ColumnIndex == 0)
                    {
                        var cell = gridNSSTagList.Rows[e.RowIndex].Cells[0];
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
                        ((CurrencyManager)gridNSSTagList.BindingContext[tagItems]).Refresh();
                    }
                    else if (e.ColumnIndex == 1)
                    {
                        var sha = GetSHA256(lastFilePath);
                        if (!containTagList.ContainsKey(sha))
                            containTagList.Add(sha, new FilePathTagList() { FilePath = lastFilePath});
                        var name = gridNSSTagList.Rows[e.RowIndex].Cells[0];
                        var check = (bool)(gridNSSTagList.Rows[e.RowIndex].Cells[1] as DataGridViewCheckBoxCell).Value;
                        if (name != null && !string.IsNullOrEmpty(name.Value.ToString()))
                        {
                            var nameStr = name.Value.ToString();
                            var list = containTagList[sha].TagItems;
                            if (check)
                            {
                                if (!list.Contains(nameStr))
                                    list.Add(nameStr);
                            }
                            else if (!check)
                            {
                                if (list.Contains(nameStr))
                                    list.Remove(nameStr);
                            }
                            //((CurrencyManager)metroGrid1.BindingContext[fileItems]).Refresh();
                            metroGrid1.Refresh();
                        }
                    }
                }
            }
        }

        private void gridNSSTagList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (windowTabControl.SelectedIndex == 0)
                if (e.ColumnIndex == 1)
                    (sender as DataGridView).CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void gridNSSTagList_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (windowTabControl.SelectedIndex == 0)
                if (e.ColumnIndex == 1)
                    (sender as DataGridView).CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void gridNSSTagList_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (windowTabControl.SelectedIndex == 0)
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                        gridNSSTagList.Rows[e.RowIndex].Selected = true;
                    if (gridNSSTagList.SelectedRows != null)
                        metroContextMenu1.Show(System.Windows.Forms.Cursor.Position);
                }
            }
        }
    }
}

using CoreTweet;
using Manina.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VRCSSTweaks
{
    public partial class vrcsstMainWindow
    {
        private List<string> tweetImageQueue = new List<string>();
        private List<string> selectedPostImage = new List<string>();
        private void ReloadQueueImages()
        {
            listViewTweetQueue.Items.Clear();
            foreach (var v in Enumerable.Range(0, tweetImageQueue.Count))
            {
                var path = tweetImageQueue[v];
                listViewTweetQueue.Items.Add(new Manina.Windows.Forms.ImageListViewItem(path));
            }
            listViewTweetImage.Items.Clear();
            foreach (var v in Enumerable.Range(0, selectedPostImage.Count))
            {
                var path = selectedPostImage[v];
                listViewTweetImage.Items.Add(new Manina.Windows.Forms.ImageListViewItem(path));
            }
        }
        private void listViewTweetQueue_ItemDoubleClick(object sender, Manina.Windows.Forms.ItemClickEventArgs e)
        {
            if (selectedPostImage.Count < 4) 
            {
                var index = listViewTweetQueue.Items.Cast<ImageListViewItem>().ToList().IndexOf(e.Item);
                selectedPostImage.Add(tweetImageQueue[index]);
                tweetImageQueue.RemoveAt(index);
                ReloadQueueImages();
            }
        }

        private void listViewTweetImage_ItemDoubleClick(object sender, Manina.Windows.Forms.ItemClickEventArgs e)
        {
            var index = listViewTweetImage.Items.Cast<ImageListViewItem>().ToList().IndexOf(e.Item);
            tweetImageQueue.Add(selectedPostImage[index]);
            selectedPostImage.RemoveAt(index);
            ReloadQueueImages();
        }

        private void metroButton8_Click(object sender, EventArgs e)
        {
            var name = textBoxTweetHashtagName.Text.Replace("#", "").Replace(" ", "");
            if (!string.IsNullOrEmpty(name))
            {
                var tagName = ValidateTagName(name);
                if (hashTagItems.Any(n => n.Name == tagName))
                {
                    var index = 1;
                    while (hashTagItems.Any(n => n.Name == tagName + index.ToString()))
                        ++index;
                    hashTagItems.Add(new TagItem() { Name = tagName + index.ToString(), IsChecked = false });
                }
                else
                    hashTagItems.Add(new TagItem() { Name = tagName, IsChecked = false });
                ((CurrencyManager)gridTweetHashtagList.BindingContext[hashTagItems]).Refresh();
            }
            textBoxTweetHashtagName.Text = "";
        }

        private void gridTweetHashtagList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (windowTabControl.SelectedIndex == 2)
                if (e.ColumnIndex == 1)
                    (sender as DataGridView).CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
        private void gridTweetHashtagList_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (windowTabControl.SelectedIndex == 2)
                if (e.ColumnIndex == 1)
                    (sender as DataGridView).CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
        private void RegisterToImageQueue(string path)
        {
            if (!tweetImageQueue.Contains(path) && !selectedPostImage.Contains(path))
                tweetImageQueue.Add(path);
        }
        private void gridTweetHashtagList_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (windowTabControl.SelectedIndex == 2)
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                        gridTweetHashtagList.Rows[e.RowIndex].Selected = true;
                    if (gridTweetHashtagList.SelectedRows != null)
                        metroContextMenu1.Show(System.Windows.Forms.Cursor.Position);
                }
            }
        }

        private void metroButton9_Click(object sender, EventArgs e)
        {
            try
            {
                List<MediaUploadResult> pics = new List<MediaUploadResult>();
                foreach (var v in selectedPostImage)
                {
                    if (File.Exists(v))
                        pics.Add(tweeetToken.Media.Upload(media: new FileInfo(v)));
                }
                string tags = "";
                if (hashTagItems.Any(n => n.IsChecked))
                    tags += " #" + string.Join(" #", hashTagItems.Where(n => n.IsChecked).Select(n => n.Name));
                Status s = tweeetToken.Statuses.Update(
                    status: metroTextBox1.Text + tags,
                    media_ids: pics.Select(n => n.MediaId)
                );
                metroTextBox1.Text = "";
                if(!metroCheckBox2.Checked)
                {
                    foreach (var v in hashTagItems)
                        v.IsChecked = false;
                    ((CurrencyManager)gridTweetHashtagList.BindingContext[hashTagItems]).Refresh();
                }
                selectedPostImage.Clear();
                ReloadQueueImages();
            }
            catch(Exception ex)
            {
                LogOutput(ex.ToString());
                MessageBox.Show("ツイートに失敗しました。");
            }
        }
        private void listViewTweetQueue_ItemClick(object sender, Manina.Windows.Forms.ItemClickEventArgs e)
        {
            if (e.Buttons == MouseButtons.Right)
            {
                e.Item.Selected = true;
                metroContextMenu2.Show(Cursor.Position);
            }
        }

        private void 画像を添付ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedPostImage.Count < 4)
            {
                var index = listViewTweetQueue.Items.Cast<ImageListViewItem>().ToList().IndexOf(listViewTweetQueue.SelectedItems[0]);
                selectedPostImage.Add(tweetImageQueue[index]);
                tweetImageQueue.RemoveAt(index);
                ReloadQueueImages();
            }
        }

        private void キューから削除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var index = listViewTweetQueue.Items.Cast<ImageListViewItem>().ToList().IndexOf(listViewTweetQueue.SelectedItems[0]);
            tweetImageQueue.RemoveAt(index);
            ReloadQueueImages();
        }

        private void 上から4つをすべて添付ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(var v in Enumerable.Range(0, 4))
            {
                if (selectedPostImage.Count < 4 && tweetImageQueue.Count > 0)
                {
                    selectedPostImage.Add(tweetImageQueue[0]);
                    tweetImageQueue.RemoveAt(0);
                }
                else
                    break;
            }
            ReloadQueueImages();
        }

        private void キューのクリアToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tweetImageQueue.Clear();
            ReloadQueueImages();
        }

        private void キューに戻すToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var index = listViewTweetImage.Items.Cast<ImageListViewItem>().ToList().IndexOf(listViewTweetImage.SelectedItems[0]);
            tweetImageQueue.Add(selectedPostImage[index]);
            selectedPostImage.RemoveAt(index);
            ReloadQueueImages();
        }

        private void すべてキューに戻すToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(var index in Enumerable.Range(0, selectedPostImage.Count))
            {
                tweetImageQueue.Add(selectedPostImage[0]);
                selectedPostImage.RemoveAt(0);
            }
            ReloadQueueImages();
        }

        private void 選択画像を登録解除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var index = listViewTweetImage.Items.Cast<ImageListViewItem>().ToList().IndexOf(listViewTweetImage.SelectedItems[0]);
            selectedPostImage.RemoveAt(index);
            ReloadQueueImages();
        }

        private void すべて登録解除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var v in Enumerable.Range(0, selectedPostImage.Count))
            {
                selectedPostImage.RemoveAt(0);
            }
            ReloadQueueImages();
        }

        private void metroButton6_Click_1(object sender, EventArgs e)
        {
            var auth = new AuthWindow(metroStyleManager1) { ApiKey = apiKey, ApiSecret = apiSecret, Owner = this };
            auth.ShowDialog();
            if(auth.AccountToken != null)
            {
                tweeetToken = auth.AccountToken;
                accessToken = tweeetToken.AccessToken;
                accessSecret = tweeetToken.AccessTokenSecret;
                labelTwitterUserID.Text = "@" + tweeetToken.ScreenName;
                metroPanel8.Enabled = true;
            }

        }

        private void listViewTweetImage_ItemClick(object sender, Manina.Windows.Forms.ItemClickEventArgs e)
        {
            if (e.Buttons == MouseButtons.Right)
            {
                e.Item.Selected = true;
                metroContextMenu3.Show(Cursor.Position);
            }
        }
    }
}

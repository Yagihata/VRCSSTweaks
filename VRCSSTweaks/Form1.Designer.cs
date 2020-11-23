namespace VRCSSTweaks
{
    partial class vrcsstMainWindow
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vrcsstMainWindow));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.metroStyleManager1 = new MetroFramework.Components.MetroStyleManager(this.components);
            this.windowTabControl = new MetroFramework.Controls.MetroTabControl();
            this.tabMain = new MetroFramework.Controls.MetroTabPage();
            this.tabSettings = new MetroFramework.Controls.MetroTabPage();
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.ssFolderPath = new MetroFramework.Controls.MetroTextBox();
            this.ssFolderSelectButton = new MetroFramework.Controls.MetroButton();
            this.toggleObserveSS = new MetroFramework.Controls.MetroToggle();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.toggleSortSS = new MetroFramework.Controls.MetroToggle();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.toggleObserveRunningVRC = new MetroFramework.Controls.MetroToggle();
            this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
            this.toggleStartup = new MetroFramework.Controls.MetroToggle();
            this.metroLabel6 = new MetroFramework.Controls.MetroLabel();
            this.toggleDetectBarcode = new MetroFramework.Controls.MetroToggle();
            this.metroLabel7 = new MetroFramework.Controls.MetroLabel();
            this.toggleOpenBarcode = new MetroFramework.Controls.MetroToggle();
            this.metroPanel2 = new MetroFramework.Controls.MetroPanel();
            this.metroPanel3 = new MetroFramework.Controls.MetroPanel();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager1)).BeginInit();
            this.windowTabControl.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.metroPanel1.SuspendLayout();
            this.metroPanel2.SuspendLayout();
            this.metroPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // metroStyleManager1
            // 
            this.metroStyleManager1.Owner = this;
            this.metroStyleManager1.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // windowTabControl
            // 
            this.windowTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.windowTabControl.Controls.Add(this.tabMain);
            this.windowTabControl.Controls.Add(this.tabSettings);
            this.windowTabControl.Location = new System.Drawing.Point(23, 63);
            this.windowTabControl.Name = "windowTabControl";
            this.windowTabControl.SelectedIndex = 1;
            this.windowTabControl.Size = new System.Drawing.Size(754, 364);
            this.windowTabControl.TabIndex = 3;
            this.windowTabControl.UseSelectable = true;
            // 
            // tabMain
            // 
            this.tabMain.HorizontalScrollbarBarColor = true;
            this.tabMain.HorizontalScrollbarHighlightOnWheel = false;
            this.tabMain.HorizontalScrollbarSize = 10;
            this.tabMain.Location = new System.Drawing.Point(4, 38);
            this.tabMain.Name = "tabMain";
            this.tabMain.Size = new System.Drawing.Size(746, 322);
            this.tabMain.TabIndex = 0;
            this.tabMain.Text = "ホーム";
            this.tabMain.VerticalScrollbarBarColor = true;
            this.tabMain.VerticalScrollbarHighlightOnWheel = false;
            this.tabMain.VerticalScrollbarSize = 10;
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.metroPanel1);
            this.tabSettings.HorizontalScrollbarBarColor = true;
            this.tabSettings.HorizontalScrollbarHighlightOnWheel = false;
            this.tabSettings.HorizontalScrollbarSize = 10;
            this.tabSettings.Location = new System.Drawing.Point(4, 38);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Size = new System.Drawing.Size(746, 322);
            this.tabSettings.TabIndex = 1;
            this.tabSettings.Text = "設定";
            this.tabSettings.VerticalScrollbarBarColor = true;
            this.tabSettings.VerticalScrollbarHighlightOnWheel = false;
            this.tabSettings.VerticalScrollbarSize = 10;
            // 
            // metroPanel1
            // 
            this.metroPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.metroPanel1.Controls.Add(this.toggleStartup);
            this.metroPanel1.Controls.Add(this.toggleSortSS);
            this.metroPanel1.Controls.Add(this.toggleObserveSS);
            this.metroPanel1.Controls.Add(this.metroLabel5);
            this.metroPanel1.Controls.Add(this.metroLabel3);
            this.metroPanel1.Controls.Add(this.metroLabel2);
            this.metroPanel1.Controls.Add(this.ssFolderSelectButton);
            this.metroPanel1.Controls.Add(this.ssFolderPath);
            this.metroPanel1.Controls.Add(this.metroLabel1);
            this.metroPanel1.Controls.Add(this.metroPanel2);
            this.metroPanel1.HorizontalScrollbar = true;
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Location = new System.Drawing.Point(3, 3);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Size = new System.Drawing.Size(740, 316);
            this.metroPanel1.TabIndex = 2;
            this.metroPanel1.VerticalScrollbar = true;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(3, 7);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(134, 19);
            this.metroLabel1.TabIndex = 2;
            this.metroLabel1.Text = "スクリーンショットフォルダ";
            // 
            // ssFolderPath
            // 
            this.ssFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.ssFolderPath.CustomButton.Image = null;
            this.ssFolderPath.CustomButton.Location = new System.Drawing.Point(540, 1);
            this.ssFolderPath.CustomButton.Name = "";
            this.ssFolderPath.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.ssFolderPath.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.ssFolderPath.CustomButton.TabIndex = 1;
            this.ssFolderPath.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.ssFolderPath.CustomButton.UseSelectable = true;
            this.ssFolderPath.CustomButton.Visible = false;
            this.ssFolderPath.Lines = new string[] {
        "metroTextBox1"};
            this.ssFolderPath.Location = new System.Drawing.Point(143, 3);
            this.ssFolderPath.MaxLength = 32767;
            this.ssFolderPath.Name = "ssFolderPath";
            this.ssFolderPath.PasswordChar = '\0';
            this.ssFolderPath.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.ssFolderPath.SelectedText = "";
            this.ssFolderPath.SelectionLength = 0;
            this.ssFolderPath.SelectionStart = 0;
            this.ssFolderPath.ShortcutsEnabled = true;
            this.ssFolderPath.Size = new System.Drawing.Size(562, 23);
            this.ssFolderPath.TabIndex = 3;
            this.ssFolderPath.Text = "metroTextBox1";
            this.ssFolderPath.UseSelectable = true;
            this.ssFolderPath.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.ssFolderPath.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // ssFolderSelectButton
            // 
            this.ssFolderSelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ssFolderSelectButton.Location = new System.Drawing.Point(711, 3);
            this.ssFolderSelectButton.Name = "ssFolderSelectButton";
            this.ssFolderSelectButton.Size = new System.Drawing.Size(23, 23);
            this.ssFolderSelectButton.TabIndex = 4;
            this.ssFolderSelectButton.Text = "...";
            this.ssFolderSelectButton.UseSelectable = true;
            // 
            // toggleObserveSS
            // 
            this.toggleObserveSS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.toggleObserveSS.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.toggleObserveSS.Location = new System.Drawing.Point(637, 32);
            this.toggleObserveSS.Name = "toggleObserveSS";
            this.toggleObserveSS.Size = new System.Drawing.Size(97, 23);
            this.toggleObserveSS.Style = MetroFramework.MetroColorStyle.Black;
            this.toggleObserveSS.TabIndex = 5;
            this.toggleObserveSS.Text = "Off";
            this.toggleObserveSS.UseSelectable = true;
            this.toggleObserveSS.CheckedChanged += new System.EventHandler(this.toggleObserveSS_CheckedChanged);
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(3, 36);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(196, 19);
            this.metroLabel2.TabIndex = 6;
            this.metroLabel2.Text = "スクリーンショットの撮影を検出する";
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.Location = new System.Drawing.Point(3, 172);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(293, 19);
            this.metroLabel3.TabIndex = 7;
            this.metroLabel3.Text = "撮影されたスクリーンショットを日付でフォルダ分けする";
            // 
            // toggleSortSS
            // 
            this.toggleSortSS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.toggleSortSS.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.toggleSortSS.Location = new System.Drawing.Point(637, 168);
            this.toggleSortSS.Name = "toggleSortSS";
            this.toggleSortSS.Size = new System.Drawing.Size(97, 23);
            this.toggleSortSS.Style = MetroFramework.MetroColorStyle.Black;
            this.toggleSortSS.TabIndex = 8;
            this.toggleSortSS.Text = "Off";
            this.toggleSortSS.UseSelectable = true;
            // 
            // metroLabel4
            // 
            this.metroLabel4.AutoSize = true;
            this.metroLabel4.Location = new System.Drawing.Point(23, 8);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(168, 19);
            this.metroLabel4.TabIndex = 9;
            this.metroLabel4.Text = "VRChat起動時のみ検出する";
            // 
            // toggleObserveRunningVRC
            // 
            this.toggleObserveRunningVRC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.toggleObserveRunningVRC.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.toggleObserveRunningVRC.Location = new System.Drawing.Point(634, 4);
            this.toggleObserveRunningVRC.Name = "toggleObserveRunningVRC";
            this.toggleObserveRunningVRC.Size = new System.Drawing.Size(97, 23);
            this.toggleObserveRunningVRC.Style = MetroFramework.MetroColorStyle.Black;
            this.toggleObserveRunningVRC.TabIndex = 10;
            this.toggleObserveRunningVRC.Text = "Off";
            this.toggleObserveRunningVRC.UseSelectable = true;
            // 
            // metroLabel5
            // 
            this.metroLabel5.AutoSize = true;
            this.metroLabel5.Location = new System.Drawing.Point(3, 201);
            this.metroLabel5.Name = "metroLabel5";
            this.metroLabel5.Size = new System.Drawing.Size(271, 19);
            this.metroLabel5.TabIndex = 12;
            this.metroLabel5.Text = "Windows起動時に本アプリケーションを起動する";
            // 
            // toggleStartup
            // 
            this.toggleStartup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.toggleStartup.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.toggleStartup.Location = new System.Drawing.Point(637, 197);
            this.toggleStartup.Name = "toggleStartup";
            this.toggleStartup.Size = new System.Drawing.Size(97, 23);
            this.toggleStartup.Style = MetroFramework.MetroColorStyle.Black;
            this.toggleStartup.TabIndex = 11;
            this.toggleStartup.Text = "Off";
            this.toggleStartup.UseSelectable = true;
            // 
            // metroLabel6
            // 
            this.metroLabel6.AutoSize = true;
            this.metroLabel6.Location = new System.Drawing.Point(23, 37);
            this.metroLabel6.Name = "metroLabel6";
            this.metroLabel6.Size = new System.Drawing.Size(160, 19);
            this.metroLabel6.TabIndex = 14;
            this.metroLabel6.Text = "2次元バーコードを検出する";
            // 
            // toggleDetectBarcode
            // 
            this.toggleDetectBarcode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.toggleDetectBarcode.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.toggleDetectBarcode.Location = new System.Drawing.Point(634, 33);
            this.toggleDetectBarcode.Name = "toggleDetectBarcode";
            this.toggleDetectBarcode.Size = new System.Drawing.Size(97, 23);
            this.toggleDetectBarcode.Style = MetroFramework.MetroColorStyle.Black;
            this.toggleDetectBarcode.TabIndex = 13;
            this.toggleDetectBarcode.Text = "Off";
            this.toggleDetectBarcode.UseSelectable = true;
            this.toggleDetectBarcode.CheckedChanged += new System.EventHandler(this.toggleDetectBarcode_CheckedChanged);
            // 
            // metroLabel7
            // 
            this.metroLabel7.AutoSize = true;
            this.metroLabel7.Location = new System.Drawing.Point(24, 9);
            this.metroLabel7.Name = "metroLabel7";
            this.metroLabel7.Size = new System.Drawing.Size(211, 19);
            this.metroLabel7.TabIndex = 16;
            this.metroLabel7.Text = "2次元バーコード検出時に自動で開く";
            // 
            // toggleOpenBarcode
            // 
            this.toggleOpenBarcode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.toggleOpenBarcode.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.toggleOpenBarcode.Location = new System.Drawing.Point(611, 5);
            this.toggleOpenBarcode.Name = "toggleOpenBarcode";
            this.toggleOpenBarcode.Size = new System.Drawing.Size(97, 23);
            this.toggleOpenBarcode.Style = MetroFramework.MetroColorStyle.Black;
            this.toggleOpenBarcode.TabIndex = 15;
            this.toggleOpenBarcode.Text = "Off";
            this.toggleOpenBarcode.UseSelectable = true;
            // 
            // metroPanel2
            // 
            this.metroPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.metroPanel2.Controls.Add(this.toggleObserveRunningVRC);
            this.metroPanel2.Controls.Add(this.toggleDetectBarcode);
            this.metroPanel2.Controls.Add(this.metroLabel6);
            this.metroPanel2.Controls.Add(this.metroLabel4);
            this.metroPanel2.Controls.Add(this.metroPanel3);
            this.metroPanel2.HorizontalScrollbarBarColor = true;
            this.metroPanel2.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel2.HorizontalScrollbarSize = 10;
            this.metroPanel2.Location = new System.Drawing.Point(3, 61);
            this.metroPanel2.Name = "metroPanel2";
            this.metroPanel2.Size = new System.Drawing.Size(734, 101);
            this.metroPanel2.TabIndex = 17;
            this.metroPanel2.VerticalScrollbarBarColor = true;
            this.metroPanel2.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel2.VerticalScrollbarSize = 10;
            // 
            // metroPanel3
            // 
            this.metroPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.metroPanel3.Controls.Add(this.toggleOpenBarcode);
            this.metroPanel3.Controls.Add(this.metroLabel7);
            this.metroPanel3.HorizontalScrollbarBarColor = true;
            this.metroPanel3.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel3.HorizontalScrollbarSize = 10;
            this.metroPanel3.Location = new System.Drawing.Point(23, 62);
            this.metroPanel3.Name = "metroPanel3";
            this.metroPanel3.Size = new System.Drawing.Size(708, 31);
            this.metroPanel3.TabIndex = 18;
            this.metroPanel3.VerticalScrollbarBarColor = true;
            this.metroPanel3.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel3.VerticalScrollbarSize = 10;
            // 
            // vrcsstMainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.windowTabControl);
            this.Name = "vrcsstMainWindow";
            this.Text = "VRCSSTweaks";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager1)).EndInit();
            this.windowTabControl.ResumeLayout(false);
            this.tabSettings.ResumeLayout(false);
            this.metroPanel1.ResumeLayout(false);
            this.metroPanel1.PerformLayout();
            this.metroPanel2.ResumeLayout(false);
            this.metroPanel2.PerformLayout();
            this.metroPanel3.ResumeLayout(false);
            this.metroPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private MetroFramework.Components.MetroStyleManager metroStyleManager1;
        private MetroFramework.Controls.MetroTabControl windowTabControl;
        private MetroFramework.Controls.MetroTabPage tabMain;
        private MetroFramework.Controls.MetroTabPage tabSettings;
        private MetroFramework.Controls.MetroPanel metroPanel1;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroButton ssFolderSelectButton;
        private MetroFramework.Controls.MetroTextBox ssFolderPath;
        private MetroFramework.Controls.MetroToggle toggleObserveSS;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroLabel metroLabel5;
        private MetroFramework.Controls.MetroToggle toggleStartup;
        private MetroFramework.Controls.MetroToggle toggleObserveRunningVRC;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private MetroFramework.Controls.MetroToggle toggleSortSS;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private MetroFramework.Controls.MetroPanel metroPanel3;
        private MetroFramework.Controls.MetroPanel metroPanel2;
        private MetroFramework.Controls.MetroLabel metroLabel7;
        private MetroFramework.Controls.MetroToggle toggleOpenBarcode;
        private MetroFramework.Controls.MetroLabel metroLabel6;
        private MetroFramework.Controls.MetroToggle toggleDetectBarcode;
    }
}


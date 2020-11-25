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
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("タグ１");
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem("✓　タグ２");
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem("タグ３");
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.metroStyleManager1 = new MetroFramework.Components.MetroStyleManager(this.components);
            this.windowTabControl = new MetroFramework.Controls.MetroTabControl();
            this.tabMain = new MetroFramework.Controls.MetroTabPage();
            this.tabScreenshots = new MetroFramework.Controls.MetroTabPage();
            this.tab2DBarcode = new MetroFramework.Controls.MetroTabPage();
            this.tabSettings = new MetroFramework.Controls.MetroTabPage();
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.toggleStartup = new MetroFramework.Controls.MetroToggle();
            this.toggleSortSS = new MetroFramework.Controls.MetroToggle();
            this.toggleObserveSS = new MetroFramework.Controls.MetroToggle();
            this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.ssFolderSelectButton = new MetroFramework.Controls.MetroButton();
            this.ssFolderPath = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel2 = new MetroFramework.Controls.MetroPanel();
            this.toggleObserveRunningVRC = new MetroFramework.Controls.MetroToggle();
            this.toggleDetectBarcode = new MetroFramework.Controls.MetroToggle();
            this.metroLabel6 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel3 = new MetroFramework.Controls.MetroPanel();
            this.toggleOpenBarcode = new MetroFramework.Controls.MetroToggle();
            this.metroLabel7 = new MetroFramework.Controls.MetroLabel();
            this.panelNewScreenshot = new MetroFramework.Controls.MetroPanel();
            this.metroLabel8 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel9 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel10 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel11 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel12 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel13 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel14 = new MetroFramework.Controls.MetroLabel();
            this.labelRecentlySSName = new MetroFramework.Controls.MetroLabel();
            this.labelRecentlySSDate = new MetroFramework.Controls.MetroLabel();
            this.labelRecentlySSSize = new MetroFramework.Controls.MetroLabel();
            this.metroLabel18 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel19 = new MetroFramework.Controls.MetroLabel();
            this.textBoxRecentlySSURL = new MetroFramework.Controls.MetroTextBox();
            this.metroButton1 = new MetroFramework.Controls.MetroButton();
            this.metroButton2 = new MetroFramework.Controls.MetroButton();
            this.metroListView1 = new MetroFramework.Controls.MetroListView();
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager1)).BeginInit();
            this.windowTabControl.SuspendLayout();
            this.tabMain.SuspendLayout();
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
            this.windowTabControl.Controls.Add(this.tabScreenshots);
            this.windowTabControl.Controls.Add(this.tab2DBarcode);
            this.windowTabControl.Controls.Add(this.tabSettings);
            this.windowTabControl.Location = new System.Drawing.Point(23, 63);
            this.windowTabControl.Name = "windowTabControl";
            this.windowTabControl.SelectedIndex = 0;
            this.windowTabControl.Size = new System.Drawing.Size(757, 477);
            this.windowTabControl.TabIndex = 3;
            this.windowTabControl.UseSelectable = true;
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.metroListView1);
            this.tabMain.Controls.Add(this.metroButton2);
            this.tabMain.Controls.Add(this.metroButton1);
            this.tabMain.Controls.Add(this.textBoxRecentlySSURL);
            this.tabMain.Controls.Add(this.metroLabel19);
            this.tabMain.Controls.Add(this.metroLabel18);
            this.tabMain.Controls.Add(this.labelRecentlySSSize);
            this.tabMain.Controls.Add(this.labelRecentlySSDate);
            this.tabMain.Controls.Add(this.labelRecentlySSName);
            this.tabMain.Controls.Add(this.metroLabel14);
            this.tabMain.Controls.Add(this.metroLabel13);
            this.tabMain.Controls.Add(this.metroLabel12);
            this.tabMain.Controls.Add(this.metroLabel11);
            this.tabMain.Controls.Add(this.metroLabel10);
            this.tabMain.Controls.Add(this.metroLabel9);
            this.tabMain.Controls.Add(this.metroLabel8);
            this.tabMain.Controls.Add(this.panelNewScreenshot);
            this.tabMain.HorizontalScrollbarBarColor = true;
            this.tabMain.HorizontalScrollbarHighlightOnWheel = false;
            this.tabMain.HorizontalScrollbarSize = 10;
            this.tabMain.Location = new System.Drawing.Point(4, 38);
            this.tabMain.Name = "tabMain";
            this.tabMain.Size = new System.Drawing.Size(749, 435);
            this.tabMain.TabIndex = 0;
            this.tabMain.Text = "ホーム";
            this.tabMain.VerticalScrollbarBarColor = true;
            this.tabMain.VerticalScrollbarHighlightOnWheel = false;
            this.tabMain.VerticalScrollbarSize = 10;
            // 
            // tabScreenshots
            // 
            this.tabScreenshots.HorizontalScrollbarBarColor = true;
            this.tabScreenshots.HorizontalScrollbarHighlightOnWheel = false;
            this.tabScreenshots.HorizontalScrollbarSize = 10;
            this.tabScreenshots.Location = new System.Drawing.Point(4, 38);
            this.tabScreenshots.Name = "tabScreenshots";
            this.tabScreenshots.Size = new System.Drawing.Size(746, 322);
            this.tabScreenshots.TabIndex = 2;
            this.tabScreenshots.Text = "スクリーンショット";
            this.tabScreenshots.VerticalScrollbarBarColor = true;
            this.tabScreenshots.VerticalScrollbarHighlightOnWheel = false;
            this.tabScreenshots.VerticalScrollbarSize = 10;
            // 
            // tab2DBarcode
            // 
            this.tab2DBarcode.HorizontalScrollbarBarColor = true;
            this.tab2DBarcode.HorizontalScrollbarHighlightOnWheel = false;
            this.tab2DBarcode.HorizontalScrollbarSize = 10;
            this.tab2DBarcode.Location = new System.Drawing.Point(4, 38);
            this.tab2DBarcode.Name = "tab2DBarcode";
            this.tab2DBarcode.Size = new System.Drawing.Size(746, 322);
            this.tab2DBarcode.TabIndex = 3;
            this.tab2DBarcode.Text = "バーコード";
            this.tab2DBarcode.VerticalScrollbarBarColor = true;
            this.tab2DBarcode.VerticalScrollbarHighlightOnWheel = false;
            this.tab2DBarcode.VerticalScrollbarSize = 10;
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.metroPanel1);
            this.tabSettings.HorizontalScrollbarBarColor = true;
            this.tabSettings.HorizontalScrollbarHighlightOnWheel = false;
            this.tabSettings.HorizontalScrollbarSize = 10;
            this.tabSettings.Location = new System.Drawing.Point(4, 38);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Size = new System.Drawing.Size(749, 435);
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
            this.metroPanel1.Size = new System.Drawing.Size(743, 429);
            this.metroPanel1.TabIndex = 2;
            this.metroPanel1.VerticalScrollbar = true;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // toggleStartup
            // 
            this.toggleStartup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.toggleStartup.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.toggleStartup.Location = new System.Drawing.Point(640, 197);
            this.toggleStartup.Name = "toggleStartup";
            this.toggleStartup.Size = new System.Drawing.Size(97, 23);
            this.toggleStartup.Style = MetroFramework.MetroColorStyle.Black;
            this.toggleStartup.TabIndex = 11;
            this.toggleStartup.Text = "Off";
            this.toggleStartup.UseSelectable = true;
            // 
            // toggleSortSS
            // 
            this.toggleSortSS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.toggleSortSS.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.toggleSortSS.Location = new System.Drawing.Point(640, 168);
            this.toggleSortSS.Name = "toggleSortSS";
            this.toggleSortSS.Size = new System.Drawing.Size(97, 23);
            this.toggleSortSS.Style = MetroFramework.MetroColorStyle.Black;
            this.toggleSortSS.TabIndex = 8;
            this.toggleSortSS.Text = "Off";
            this.toggleSortSS.UseSelectable = true;
            // 
            // toggleObserveSS
            // 
            this.toggleObserveSS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.toggleObserveSS.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.toggleObserveSS.Location = new System.Drawing.Point(640, 32);
            this.toggleObserveSS.Name = "toggleObserveSS";
            this.toggleObserveSS.Size = new System.Drawing.Size(97, 23);
            this.toggleObserveSS.Style = MetroFramework.MetroColorStyle.Black;
            this.toggleObserveSS.TabIndex = 5;
            this.toggleObserveSS.Text = "Off";
            this.toggleObserveSS.UseSelectable = true;
            this.toggleObserveSS.CheckedChanged += new System.EventHandler(this.toggleObserveSS_CheckedChanged);
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
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.Location = new System.Drawing.Point(3, 172);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(293, 19);
            this.metroLabel3.TabIndex = 7;
            this.metroLabel3.Text = "撮影されたスクリーンショットを日付でフォルダ分けする";
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
            // ssFolderSelectButton
            // 
            this.ssFolderSelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ssFolderSelectButton.Location = new System.Drawing.Point(714, 3);
            this.ssFolderSelectButton.Name = "ssFolderSelectButton";
            this.ssFolderSelectButton.Size = new System.Drawing.Size(23, 23);
            this.ssFolderSelectButton.TabIndex = 4;
            this.ssFolderSelectButton.Text = "...";
            this.ssFolderSelectButton.UseSelectable = true;
            // 
            // ssFolderPath
            // 
            this.ssFolderPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.ssFolderPath.CustomButton.Image = null;
            this.ssFolderPath.CustomButton.Location = new System.Drawing.Point(543, 1);
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
            this.ssFolderPath.Size = new System.Drawing.Size(565, 23);
            this.ssFolderPath.TabIndex = 3;
            this.ssFolderPath.Text = "metroTextBox1";
            this.ssFolderPath.UseSelectable = true;
            this.ssFolderPath.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.ssFolderPath.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
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
            this.metroPanel2.Size = new System.Drawing.Size(737, 101);
            this.metroPanel2.TabIndex = 17;
            this.metroPanel2.VerticalScrollbarBarColor = true;
            this.metroPanel2.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel2.VerticalScrollbarSize = 10;
            // 
            // toggleObserveRunningVRC
            // 
            this.toggleObserveRunningVRC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.toggleObserveRunningVRC.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.toggleObserveRunningVRC.Location = new System.Drawing.Point(637, 4);
            this.toggleObserveRunningVRC.Name = "toggleObserveRunningVRC";
            this.toggleObserveRunningVRC.Size = new System.Drawing.Size(97, 23);
            this.toggleObserveRunningVRC.Style = MetroFramework.MetroColorStyle.Black;
            this.toggleObserveRunningVRC.TabIndex = 10;
            this.toggleObserveRunningVRC.Text = "Off";
            this.toggleObserveRunningVRC.UseSelectable = true;
            // 
            // toggleDetectBarcode
            // 
            this.toggleDetectBarcode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.toggleDetectBarcode.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.toggleDetectBarcode.Location = new System.Drawing.Point(637, 33);
            this.toggleDetectBarcode.Name = "toggleDetectBarcode";
            this.toggleDetectBarcode.Size = new System.Drawing.Size(97, 23);
            this.toggleDetectBarcode.Style = MetroFramework.MetroColorStyle.Black;
            this.toggleDetectBarcode.TabIndex = 13;
            this.toggleDetectBarcode.Text = "Off";
            this.toggleDetectBarcode.UseSelectable = true;
            this.toggleDetectBarcode.CheckedChanged += new System.EventHandler(this.toggleDetectBarcode_CheckedChanged);
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
            // metroLabel4
            // 
            this.metroLabel4.AutoSize = true;
            this.metroLabel4.Location = new System.Drawing.Point(23, 8);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(168, 19);
            this.metroLabel4.TabIndex = 9;
            this.metroLabel4.Text = "VRChat起動時のみ検出する";
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
            this.metroPanel3.Size = new System.Drawing.Size(711, 31);
            this.metroPanel3.TabIndex = 18;
            this.metroPanel3.VerticalScrollbarBarColor = true;
            this.metroPanel3.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel3.VerticalScrollbarSize = 10;
            // 
            // toggleOpenBarcode
            // 
            this.toggleOpenBarcode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.toggleOpenBarcode.FontSize = MetroFramework.MetroLinkSize.Medium;
            this.toggleOpenBarcode.Location = new System.Drawing.Point(614, 5);
            this.toggleOpenBarcode.Name = "toggleOpenBarcode";
            this.toggleOpenBarcode.Size = new System.Drawing.Size(97, 23);
            this.toggleOpenBarcode.Style = MetroFramework.MetroColorStyle.Black;
            this.toggleOpenBarcode.TabIndex = 15;
            this.toggleOpenBarcode.Text = "Off";
            this.toggleOpenBarcode.UseSelectable = true;
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
            // panelNewScreenshot
            // 
            this.panelNewScreenshot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelNewScreenshot.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panelNewScreenshot.HorizontalScrollbarBarColor = true;
            this.panelNewScreenshot.HorizontalScrollbarHighlightOnWheel = false;
            this.panelNewScreenshot.HorizontalScrollbarSize = 10;
            this.panelNewScreenshot.Location = new System.Drawing.Point(3, 22);
            this.panelNewScreenshot.Name = "panelNewScreenshot";
            this.panelNewScreenshot.Size = new System.Drawing.Size(581, 326);
            this.panelNewScreenshot.TabIndex = 2;
            this.panelNewScreenshot.VerticalScrollbarBarColor = true;
            this.panelNewScreenshot.VerticalScrollbarHighlightOnWheel = false;
            this.panelNewScreenshot.VerticalScrollbarSize = 10;
            // 
            // metroLabel8
            // 
            this.metroLabel8.AutoSize = true;
            this.metroLabel8.Location = new System.Drawing.Point(3, 0);
            this.metroLabel8.Name = "metroLabel8";
            this.metroLabel8.Size = new System.Drawing.Size(134, 19);
            this.metroLabel8.TabIndex = 3;
            this.metroLabel8.Text = "最新のスクリーンショット";
            // 
            // metroLabel9
            // 
            this.metroLabel9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.metroLabel9.AutoSize = true;
            this.metroLabel9.Location = new System.Drawing.Point(3, 351);
            this.metroLabel9.Name = "metroLabel9";
            this.metroLabel9.Size = new System.Drawing.Size(61, 19);
            this.metroLabel9.TabIndex = 4;
            this.metroLabel9.Text = "ファイル名";
            // 
            // metroLabel10
            // 
            this.metroLabel10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.metroLabel10.AutoSize = true;
            this.metroLabel10.Location = new System.Drawing.Point(3, 370);
            this.metroLabel10.Name = "metroLabel10";
            this.metroLabel10.Size = new System.Drawing.Size(65, 19);
            this.metroLabel10.TabIndex = 5;
            this.metroLabel10.Text = "撮影日時";
            // 
            // metroLabel11
            // 
            this.metroLabel11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.metroLabel11.AutoSize = true;
            this.metroLabel11.Location = new System.Drawing.Point(3, 389);
            this.metroLabel11.Name = "metroLabel11";
            this.metroLabel11.Size = new System.Drawing.Size(79, 19);
            this.metroLabel11.TabIndex = 6;
            this.metroLabel11.Text = "ファイルサイズ";
            // 
            // metroLabel12
            // 
            this.metroLabel12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.metroLabel12.AutoSize = true;
            this.metroLabel12.Location = new System.Drawing.Point(88, 389);
            this.metroLabel12.Name = "metroLabel12";
            this.metroLabel12.Size = new System.Drawing.Size(23, 19);
            this.metroLabel12.TabIndex = 7;
            this.metroLabel12.Text = "：";
            // 
            // metroLabel13
            // 
            this.metroLabel13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.metroLabel13.AutoSize = true;
            this.metroLabel13.Location = new System.Drawing.Point(88, 370);
            this.metroLabel13.Name = "metroLabel13";
            this.metroLabel13.Size = new System.Drawing.Size(23, 19);
            this.metroLabel13.TabIndex = 8;
            this.metroLabel13.Text = "：";
            // 
            // metroLabel14
            // 
            this.metroLabel14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.metroLabel14.AutoSize = true;
            this.metroLabel14.Location = new System.Drawing.Point(88, 351);
            this.metroLabel14.Name = "metroLabel14";
            this.metroLabel14.Size = new System.Drawing.Size(23, 19);
            this.metroLabel14.TabIndex = 9;
            this.metroLabel14.Text = "：";
            // 
            // labelRecentlySSName
            // 
            this.labelRecentlySSName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelRecentlySSName.AutoSize = true;
            this.labelRecentlySSName.Location = new System.Drawing.Point(117, 351);
            this.labelRecentlySSName.Name = "labelRecentlySSName";
            this.labelRecentlySSName.Size = new System.Drawing.Size(102, 19);
            this.labelRecentlySSName.TabIndex = 10;
            this.labelRecentlySSName.Text = "デフォルトテキスト";
            // 
            // labelRecentlySSDate
            // 
            this.labelRecentlySSDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelRecentlySSDate.AutoSize = true;
            this.labelRecentlySSDate.Location = new System.Drawing.Point(117, 370);
            this.labelRecentlySSDate.Name = "labelRecentlySSDate";
            this.labelRecentlySSDate.Size = new System.Drawing.Size(102, 19);
            this.labelRecentlySSDate.TabIndex = 11;
            this.labelRecentlySSDate.Text = "デフォルトテキスト";
            // 
            // labelRecentlySSSize
            // 
            this.labelRecentlySSSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelRecentlySSSize.AutoSize = true;
            this.labelRecentlySSSize.Location = new System.Drawing.Point(117, 389);
            this.labelRecentlySSSize.Name = "labelRecentlySSSize";
            this.labelRecentlySSSize.Size = new System.Drawing.Size(102, 19);
            this.labelRecentlySSSize.TabIndex = 12;
            this.labelRecentlySSSize.Text = "デフォルトテキスト";
            // 
            // metroLabel18
            // 
            this.metroLabel18.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.metroLabel18.AutoSize = true;
            this.metroLabel18.Location = new System.Drawing.Point(3, 411);
            this.metroLabel18.Name = "metroLabel18";
            this.metroLabel18.Size = new System.Drawing.Size(60, 19);
            this.metroLabel18.TabIndex = 13;
            this.metroLabel18.Text = "検出URL";
            // 
            // metroLabel19
            // 
            this.metroLabel19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.metroLabel19.AutoSize = true;
            this.metroLabel19.Location = new System.Drawing.Point(88, 411);
            this.metroLabel19.Name = "metroLabel19";
            this.metroLabel19.Size = new System.Drawing.Size(23, 19);
            this.metroLabel19.TabIndex = 14;
            this.metroLabel19.Text = "：";
            // 
            // textBoxRecentlySSURL
            // 
            this.textBoxRecentlySSURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.textBoxRecentlySSURL.CustomButton.Image = null;
            this.textBoxRecentlySSURL.CustomButton.Location = new System.Drawing.Point(445, 1);
            this.textBoxRecentlySSURL.CustomButton.Name = "";
            this.textBoxRecentlySSURL.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.textBoxRecentlySSURL.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.textBoxRecentlySSURL.CustomButton.TabIndex = 1;
            this.textBoxRecentlySSURL.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.textBoxRecentlySSURL.CustomButton.UseSelectable = true;
            this.textBoxRecentlySSURL.CustomButton.Visible = false;
            this.textBoxRecentlySSURL.Lines = new string[] {
        "metroTextBox1"};
            this.textBoxRecentlySSURL.Location = new System.Drawing.Point(117, 411);
            this.textBoxRecentlySSURL.MaxLength = 32767;
            this.textBoxRecentlySSURL.Name = "textBoxRecentlySSURL";
            this.textBoxRecentlySSURL.PasswordChar = '\0';
            this.textBoxRecentlySSURL.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.textBoxRecentlySSURL.SelectedText = "";
            this.textBoxRecentlySSURL.SelectionLength = 0;
            this.textBoxRecentlySSURL.SelectionStart = 0;
            this.textBoxRecentlySSURL.ShortcutsEnabled = true;
            this.textBoxRecentlySSURL.Size = new System.Drawing.Size(467, 23);
            this.textBoxRecentlySSURL.TabIndex = 15;
            this.textBoxRecentlySSURL.Text = "metroTextBox1";
            this.textBoxRecentlySSURL.UseSelectable = true;
            this.textBoxRecentlySSURL.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.textBoxRecentlySSURL.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroButton1
            // 
            this.metroButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.metroButton1.Location = new System.Drawing.Point(590, 412);
            this.metroButton1.Name = "metroButton1";
            this.metroButton1.Size = new System.Drawing.Size(75, 23);
            this.metroButton1.TabIndex = 16;
            this.metroButton1.Text = "コピー";
            this.metroButton1.UseSelectable = true;
            // 
            // metroButton2
            // 
            this.metroButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.metroButton2.Location = new System.Drawing.Point(671, 412);
            this.metroButton2.Name = "metroButton2";
            this.metroButton2.Size = new System.Drawing.Size(75, 23);
            this.metroButton2.TabIndex = 17;
            this.metroButton2.Text = "開く";
            this.metroButton2.UseSelectable = true;
            // 
            // metroListView1
            // 
            this.metroListView1.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.metroListView1.FullRowSelect = true;
            this.metroListView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem7,
            listViewItem8,
            listViewItem9});
            this.metroListView1.Location = new System.Drawing.Point(590, 22);
            this.metroListView1.Name = "metroListView1";
            this.metroListView1.OwnerDraw = true;
            this.metroListView1.Size = new System.Drawing.Size(156, 326);
            this.metroListView1.TabIndex = 18;
            this.metroListView1.UseCompatibleStateImageBehavior = false;
            this.metroListView1.UseSelectable = true;
            this.metroListView1.View = System.Windows.Forms.View.List;
            // 
            // vrcsstMainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(803, 563);
            this.Controls.Add(this.windowTabControl);
            this.Name = "vrcsstMainWindow";
            this.Text = "VRCSSTweaks";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.vrcsstMainWindow_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResizeBegin += new System.EventHandler(this.vrcsstMainWindow_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.vrcsstMainWindow_ResizeEnd);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.metroStyleManager1)).EndInit();
            this.windowTabControl.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.tabMain.PerformLayout();
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
        private MetroFramework.Controls.MetroTabPage tabScreenshots;
        private MetroFramework.Controls.MetroTabPage tab2DBarcode;
        private MetroFramework.Controls.MetroPanel panelNewScreenshot;
        private MetroFramework.Controls.MetroLabel metroLabel8;
        private MetroFramework.Controls.MetroButton metroButton2;
        private MetroFramework.Controls.MetroButton metroButton1;
        private MetroFramework.Controls.MetroTextBox textBoxRecentlySSURL;
        private MetroFramework.Controls.MetroLabel metroLabel19;
        private MetroFramework.Controls.MetroLabel metroLabel18;
        private MetroFramework.Controls.MetroLabel labelRecentlySSSize;
        private MetroFramework.Controls.MetroLabel labelRecentlySSDate;
        private MetroFramework.Controls.MetroLabel labelRecentlySSName;
        private MetroFramework.Controls.MetroLabel metroLabel14;
        private MetroFramework.Controls.MetroLabel metroLabel13;
        private MetroFramework.Controls.MetroLabel metroLabel12;
        private MetroFramework.Controls.MetroLabel metroLabel11;
        private MetroFramework.Controls.MetroLabel metroLabel10;
        private MetroFramework.Controls.MetroLabel metroLabel9;
        private MetroFramework.Controls.MetroListView metroListView1;
    }
}


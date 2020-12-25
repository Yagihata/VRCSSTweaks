using CoreTweet;
using MetroFramework.Components;
using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VRCSSTweaks
{
    public partial class AuthWindow : MetroForm
    {

        MetroStyleManager metroStyleManager;
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public Tokens AccountToken { get; set; } = null;
        private OAuth.OAuthSession session;
        public AuthWindow(MetroStyleManager metroStyleManager)
        {
            InitializeComponent();
            this.metroStyleManager = metroStyleManager;
            this.StyleManager = metroStyleManager;
            this.components.SetDefaultStyle(this, metroStyleManager.Style);
            this.components.SetDefaultTheme(this, metroStyleManager.Theme);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            metroPanel1.Enabled = false;
            metroPanel1.Visible = false;
        }
        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.Demand,
                Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                const int CS_NOCLOSE = 0x200;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle = cp.ClassStyle | CS_NOCLOSE;

                return cp;
            }
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            session = OAuth.Authorize(ApiKey, ApiSecret);
            Process.Start(session.AuthorizeUri.ToString());
            metroPanel1.Enabled = true;
            metroPanel1.Visible = true;
            metroButton1.Enabled = false;
            metroButton1.Visible = false;
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            metroButton2.Enabled = false;
            try
            {
                AccountToken = OAuth.GetTokens(session, metroTextBox1.Text);
            }
            catch 
            {
                MessageBox.Show("Twitterの認証に失敗しました。");
            }
            this.Close();
        }
    }
}

using MetroFramework.Components;
using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VRCSSTweaks
{
    public partial class NewMessageWindow : MetroForm
    {
        public string MessageText { set { metroTextBox1.Lines = value.Split('\n'); } }
        MetroStyleManager metroStyleManager;
        public NewMessageWindow(MetroStyleManager metroStyleManager)
        {
            InitializeComponent();
            this.metroStyleManager = metroStyleManager;
            this.StyleManager = metroStyleManager;
            this.components.SetDefaultStyle(this, metroStyleManager.Style);
            this.components.SetDefaultTheme(this, metroStyleManager.Theme);
            metroTextBox1.ReadOnly = true;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

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
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

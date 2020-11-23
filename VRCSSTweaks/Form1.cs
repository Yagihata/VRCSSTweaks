using MetroFramework;
using MetroFramework.Controls;
using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VRCSSTweaks
{
    public partial class vrcsstMainWindow : MetroForm
    {
        public vrcsstMainWindow()
        {
            InitializeComponent();
            this.components.SetDefaultStyle(this, metroStyleManager1.Style);
            this.components.SetDefaultTheme(this, metroStyleManager1.Theme);
            this.StyleManager = metroStyleManager1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
                return;
            Visible = false;
            notifyIcon1.Visible = true;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Visible = true;
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void toggleDetectBarcode_CheckedChanged(object sender, EventArgs e)
        {
            metroPanel3.Enabled = (sender as MetroToggle).Checked;
        }

        private void toggleObserveSS_CheckedChanged(object sender, EventArgs e)
        {
            metroPanel2.Enabled = (sender as MetroToggle).Checked;
        }
    }
}

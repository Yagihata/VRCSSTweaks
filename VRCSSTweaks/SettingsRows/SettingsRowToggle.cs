using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace VRCSSTweaks.SettingsRows
{
    public partial class SettingsRowToggle : MetroUserControl
    {
        public string LabelText { get { return metroLabel1.Text; } set { metroLabel1.Text = value; } }
        public bool Checked { get { return metroToggle1.Checked; } set { metroToggle1.Checked = value; } }
        public event EventHandler<EventArgs> CheckChanged;
        public SettingsRowToggle()
        {
            InitializeComponent();
        }

        private void metroToggle1_CheckedChanged(object sender, EventArgs e)
        {
            CheckChanged?.Invoke(this, e);
        }
    }
}

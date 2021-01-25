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
using System.Text.RegularExpressions;

namespace VRCSSTweaks.SettingsRows
{
    public partial class SettingsRowNumField : MetroUserControl
    {
        public string LabelText { get { return metroLabel1.Text; } set { metroLabel1.Text = value; } }
        public string EndLabelText { get { return metroLabel2.Text; } set { metroLabel2.Text = value; } }
        public int Value { get { return int.Parse(metroTextBox1.Text); } set { metroTextBox1.Text = value.ToString(); metroLabel2_TextChanged(null, null); } }
        public event EventHandler<EventArgs> ValueChanged;
        public SettingsRowNumField()
        {
            InitializeComponent();
        }

        private void metroTextBox1_Validating(object sender, CancelEventArgs e)
        {
            var textbox = sender as MetroTextBox;
            int num;
            int.TryParse(Regex.Replace(textbox.Text, @"[^0-9]", ""), out num);
            if (num <= 0)
                num = 1;
            textbox.Text = num.ToString();
            ValueChanged?.Invoke(this, e);
        }

        private void metroLabel2_TextChanged(object sender, EventArgs e)
        {
            metroLabel2.Left = this.Size.Width - metroLabel2.Width - 3;
            metroTextBox1.Left = this.Size.Width - metroLabel2.Width - 178;
        }

        private void metroTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || '9' < e.KeyChar) && e.KeyChar != '\b')
                e.Handled = true;
        }
    }
}

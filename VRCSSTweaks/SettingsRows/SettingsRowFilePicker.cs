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
    public partial class SettingsRowFilePicker : MetroUserControl
    {
        public string LabelText { get { return metroLabel1.Text; } set { metroLabel1.Text = value; } }
        public string FilePath { get { return metroTextBox1.Text; } set { metroTextBox1.Text = value; } }
        public event EventHandler<EventArgs> FilePathChanged;
        public SettingsRowFilePicker()
        {
            InitializeComponent();
        }

        private void metroTextBox1_TextChanged(object sender, EventArgs e)
        {
            FilePathChanged?.Invoke(this, e);
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            using (var ofd = new CommonOpenFileDialog() { DefaultDirectory = FilePath, IsFolderPicker = true })
            {
                if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    FilePath = ofd.FileName;
                }
            }
        }
    }
}

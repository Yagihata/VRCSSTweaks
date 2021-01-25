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
using static System.Windows.Forms.ComboBox;
using Microsoft.VisualBasic.Compatibility.VB6;
using System.Drawing.Design;

namespace VRCSSTweaks.SettingsRows
{
    public partial class SettingsRowComboBox : MetroUserControl
    {
        public string LabelText { get { return metroLabel1.Text; } set { metroLabel1.Text = value; } }
        public object SelectedItem { get { return metroComboBox1.SelectedItem; } set { metroComboBox1.SelectedItem = value; } }
        public int SelectedIndex { get { return metroComboBox1.SelectedIndex; } set { metroComboBox1.SelectedIndex = value; } }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Localizable(true)]
        [MergableProperty(false)]
        public ObjectCollection Items { get { return metroComboBox1.Items; } }
        public event EventHandler<EventArgs> SelectedIndexChanged;
        public SettingsRowComboBox()
        {
            InitializeComponent();
        }

        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedIndexChanged?.Invoke(this, e);
        }
    }
}

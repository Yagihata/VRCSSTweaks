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
    public partial class ProgressWindow : MetroForm
    {
        public string Title { get { return this.Text; } set{ this.Text = value; } }
        private int _currentValue = 0;
        private int _maxValue = 0;
        public int CurrentValue
        {
            get { return _currentValue; }
            set
            {
                _currentValue = value;
                var v = 1f;
                if (_currentValue <= _maxValue && _maxValue != 0)
                    v = (float)_currentValue / _maxValue * 100f;
                metroProgressBar1.Value = (int)v;
                metroLabel1.Text = string.Format("{0}/{1}({2:f2}%)", _currentValue, _maxValue, v);
            }
        }
        public int MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                var v = 1f;
                if (_currentValue <= _maxValue && _maxValue != 0)
                    v = (float)_currentValue / _maxValue * 100f;
                metroProgressBar1.Value = (int)v;
                metroLabel1.Text = string.Format("{0}/{1}({2:f2}%)", _currentValue, _maxValue, v);
            }
        }

        MetroStyleManager metroStyleManager;
        public ProgressWindow(MetroStyleManager metroStyleManager)
        {
            InitializeComponent();
            this.metroStyleManager = metroStyleManager;
            this.StyleManager = metroStyleManager;
            this.components.SetDefaultStyle(this, metroStyleManager.Style);
            this.components.SetDefaultTheme(this, metroStyleManager.Theme);
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
    }
}

using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCSSTweaks
{
    class DoubleBufferedMetroPanel :MetroPanel
    {
        public DoubleBufferedMetroPanel()
        {
            DoubleBuffered = true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VRCSSTweaks
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
#if DEBUG
            try
            {
                Application.Run(new vrcsstMainWindow());
            }
            catch(Exception ex)
            {
                vrcsstMainWindow.LogOutput(ex.ToString());
                MessageBox.Show("アプリケーションの実行中にエラーが発生しました。");
            }
#else
            Application.Run(new vrcsstMainWindow());
#endif
        }
    }
}

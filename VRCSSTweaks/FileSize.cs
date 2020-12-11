using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCSSTweaks
{
    public class FileSize : IComparable
    {
        private long rawFileSize = 0;
        public FileSize(long byteCount)
        {
            rawFileSize = byteCount;
        }
        public FileSize(FileInfo info)
        {
            rawFileSize = info.Length;
        }
        public override string ToString()
        {
            if (rawFileSize > 0)
                return FormatSize(rawFileSize, 2).ToString();
            else
                return "";
        }
        public string FormatSize(long amt, int rounding)
        {
            /// <summary>
            /// ByteをKB, MB, GB...のような他の形式に変換する
            /// KB, MB, GB, TB, PB, EB, ZB or YB
            /// 第１引数:long型
            /// 第２引数：小数点第何位まで表示するか
            /// </summary>

            if (amt >= Math.Pow(2, 80)) return Math.Round(amt
                / Math.Pow(2, 70), rounding).ToString() + " YB"; //yettabyte
            if (amt >= Math.Pow(2, 70)) return Math.Round(amt
                / Math.Pow(2, 70), rounding).ToString() + " ZB"; //zettabyte
            if (amt >= Math.Pow(2, 60)) return Math.Round(amt
                / Math.Pow(2, 60), rounding).ToString() + " EB"; //exabyte
            if (amt >= Math.Pow(2, 50)) return Math.Round(amt
                / Math.Pow(2, 50), rounding).ToString() + " PB"; //petabyte
            if (amt >= Math.Pow(2, 40)) return Math.Round(amt
                / Math.Pow(2, 40), rounding).ToString() + " TB"; //terabyte
            if (amt >= Math.Pow(2, 30)) return Math.Round(amt
                / Math.Pow(2, 30), rounding).ToString() + " GB"; //gigabyte
            if (amt >= Math.Pow(2, 20)) return Math.Round(amt
                / Math.Pow(2, 20), rounding).ToString() + " MB"; //megabyte
            if (amt >= Math.Pow(2, 10)) return Math.Round(amt
                / Math.Pow(2, 10), rounding).ToString() + " KB"; //kilobyte

            return amt.ToString() + " Bytes"; //byte
        }

        public int CompareTo(object obj)
        {
            return rawFileSize.CompareTo((obj as FileSize).rawFileSize);
        }
    }
}

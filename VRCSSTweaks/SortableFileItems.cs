using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCSSTweaks
{
    public class SortableFileItems : SortableBindingList<FileItem>
    {
        protected override int Compare(FileItem lhs, FileItem rhs)
        {
            // 比較を行う
            if (lhs.Name == "...")
                return -1;
            else if (rhs.Name == "...")
                return 1;
            var result = this.OnComparison(lhs, rhs);

            // 昇順の場合 そのまま、降順の場合 反転させる
            return (this.sortDirection == ListSortDirection.Ascending) ? result : -result;
        }

    }
}

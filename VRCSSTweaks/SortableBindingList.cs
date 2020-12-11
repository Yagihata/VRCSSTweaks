using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCSSTweaks
{
    public class SortableBindingList<T> : BindingList<T>
    {
        /// <summary>
        /// ソート済みかどうか
        /// </summary>
        private bool isSorted;

        /// <summary>
        /// 並べ替え操作の方向
        /// </summary>
        protected ListSortDirection sortDirection = ListSortDirection.Ascending;

        /// <summary>
        /// ソートを行う抽象化プロパティ
        /// </summary>
        private PropertyDescriptor sortProperty;

        /// <summary>
        /// SortableBindingList クラス の 新しいインスタンス を初期化します。
        /// </summary>
        public SortableBindingList()
        {
        }

        /// <summary>
        /// 指定した リストクラス を利用して SortableBindingList クラス の 新しいインスタンス を初期化します。
        /// </summary>
        /// <param name="list">SortableBindingList に格納される System.Collection.Generic.IList</param>
        public SortableBindingList(IList<T> list)
            : base(list)
        {
        }

        /// <summary>
        /// リストがソートをサポートしているかどうかを示す値を取得します。
        /// </summary>
        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        /// <summary>
        /// リストがソートされたかどうかを示す値を取得します。
        /// </summary>
        protected override bool IsSortedCore
        {
            get { return this.isSorted; }
        }

        /// <summary>
        /// ソートされたリストの並べ替え操作の方向を取得します。
        /// </summary>
        protected override ListSortDirection SortDirectionCore
        {
            get { return this.sortDirection; }
        }

        /// <summary>
        /// ソートに利用する抽象化プロパティを取得します。
        /// </summary>
        protected override PropertyDescriptor SortPropertyCore
        {
            get { return this.sortProperty; }
        }

        /// <summary>
        /// ApplySortCore で適用されたソートに関する情報を削除します。
        /// </summary>
        protected override void RemoveSortCore()
        {
            this.sortDirection = ListSortDirection.Ascending;
            this.sortProperty = null;
            this.isSorted = false;
        }

        /// <summary>
        /// 指定されたプロパティおよび方向でソートを行います。
        /// </summary>
        /// <param name="prop">抽象化プロパティ</param>
        /// <param name="direction">並べ替え操作の方向</param>
        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            // ソートに使う情報を記録
            this.sortProperty = prop;
            this.sortDirection = direction;

            // ソートを行うリストを取得
            var list = Items as List<T>;
            if (list == null)
            {
                return;
            }

            // ソート処理
            list.Sort(this.Compare);

            // ソート完了を記録
            this.isSorted = true;

            // ListChanged イベントを発生させます
            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        /// <summary>
        /// 比較処理を行います。
        /// </summary>
        /// <param name="lhs">左側の値</param>
        /// <param name="rhs">右側の値</param>
        /// <returns>比較結果</returns>
        protected virtual int Compare(T lhs, T rhs)
        {
            // 比較を行う
            var result = this.OnComparison(lhs, rhs);

            // 昇順の場合 そのまま、降順の場合 反転させる
            return (this.sortDirection == ListSortDirection.Ascending) ? result : -result;
        }

        /// <summary>
        /// 昇順として比較処理を行います。
        /// </summary>
        /// <param name="lhs">左側の値</param>
        /// <param name="rhs">右側の値</param>
        /// <returns>比較結果</returns>
        protected int OnComparison(T lhs, T rhs)
        {
            object lhsValue = (lhs == null) ? null : this.sortProperty.GetValue(lhs);
            object rhsValue = (rhs == null) ? null : this.sortProperty.GetValue(rhs);

            if (lhsValue == null)
            {
                return (rhsValue == null) ? 0 : -1;
            }

            if (rhsValue == null)
            {
                return 1;
            }

            if (lhsValue is IComparable)
            {
                return ((IComparable)lhsValue).CompareTo(rhsValue);
            }

            if (lhsValue.Equals(rhsValue))
            {
                return 0;
            }

            return lhsValue.ToString().CompareTo(rhsValue.ToString());
        }
    }
}

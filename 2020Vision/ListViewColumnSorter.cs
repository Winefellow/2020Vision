using System;
using System.Collections;
using System.Windows.Forms;

namespace Vision2020
{
    class ListViewItemComparer : IComparer
    {
        public int Col { get; private set; }
        public bool Reverse { get; private set; }
        public ListViewItemComparer()
        {
            Col = 0;
            Reverse = false;
        }

        public void ReverseOrder()
        {
            Reverse = !Reverse;
        }
        public ListViewItemComparer(int column, bool reverse)
        {
            Col = column;
            Reverse = reverse;
        }
        public int Compare(object x, object y)
        {
            if (Reverse)
                return -String.Compare(((ListViewItem)x).SubItems[Col].Text, ((ListViewItem)y).SubItems[Col].Text);
            else
                return String.Compare(((ListViewItem)x).SubItems[Col].Text, ((ListViewItem)y).SubItems[Col].Text);
        }
    }
}
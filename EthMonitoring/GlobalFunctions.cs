using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EthMonitoring
{
    internal class GlobalFunctions
    {
        public static int listViewCountItems(ListView varControl)
        {
            if (varControl.InvokeRequired)
            {
                return (int)varControl.Invoke(new Func<int>(() => listViewCountItems(varControl)));
            }
            else
            {
                return varControl.Items.Count;
            }
        }

        public static ListViewItem getListViewItem(ListView varControl, int row)
        {
            if (varControl.InvokeRequired)
            {
                return (ListViewItem)varControl.Invoke(new Func<ListViewItem>(() => getListViewItem(varControl, row)));
            }
            else
            {
                return varControl.Items[row];
            }
        }

        public static void listViewEditItem(ListView varListView, int varRow, int varColumn, string varText)
        {
            if (varListView.InvokeRequired)
            {
                varListView.BeginInvoke(new MethodInvoker(() => listViewEditItem(varListView, varRow, varColumn, varText)));
            }
            else
            {
                varListView.Items[varRow].SubItems[varColumn].Text = varText;
            }
        }

        public static void updateColumnSizesForListView(ListView varListView)
        {
            if (varListView.InvokeRequired)
            {
                varListView.BeginInvoke(new MethodInvoker(() => updateColumnSizesForListView(varListView)));
            }
            else
            {
                //varListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                //varListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
        }

        #region Nested type: ListViewHandlerItem
        private delegate void ListViewHandlerItem(ListView varListView, ListViewItem item);
        #endregion
    }
}

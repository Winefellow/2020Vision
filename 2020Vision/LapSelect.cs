using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vision2020
{
    public partial class LapSelect : Form
    {
        public LapSelect()
        {
            InitializeComponent();
            lapList.FullRowSelect = true;
            
            foreach(var lap in LapDatabase.Laps)
            {
                var item = lapList.Items.Add(lap.FileName, lap.CircuitName,0);
                item.SubItems.Add(lap.SessionType);
                item.SubItems.Add(lap.CarNumber);
                item.SubItems.Add(lap.PlayerName);
                item.SubItems.Add(lap.TeamName);
                item.SubItems.Add(lap.LapTime.ToString("0.000"));
            }
        }

        private void lapLst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lapList.SelectedItems.Count > 0)
            {
                //var lap = LapDatabase.Laps.First(l => l.FileName == lapList.SelectedItems[0].Text);

                //textBox1.AppendText("F" + (lap.circuitInfo.formula + 1).ToString() + " " + Constants.SessionTypeList[lap.circuitInfo.sessionType] + " " +
                //                    Constants.TrackList[lap.circuitInfo.trackId] + " " + lap.circuitInfo.trackLength.ToString() + "m" + Environment.NewLine);
                //textBox1.AppendText(lap.playerInfo.raceNumber.ToString() + " " + PacketHelper.GetString(lap.playerInfo.name, 48) + " ");
                //if (lap.lap == null)
                //{
                //    textBox1.AppendText("no lap" + Environment.NewLine);
                //}
                //else
                //{
                //    textBox1.AppendText(lap.lap.lapTime.ToString("0.000") + Environment.NewLine);
                //}
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void lapList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (ignoreClicks) return;
            ignoreClicks = true;
            lvSelected.BeginUpdate();
            lapList.BeginUpdate();
            try
            {
                if (e.Item.Checked)
                {
                    // checkedList.Add(item);
                    ListViewItem newItem = e.Item.Clone() as ListViewItem;
                    newItem.Name = e.Item.Name;
                    newItem.Checked = true;
                    lvSelected.Items.Add(newItem);
                }
                else
                {
                    var otherItem = lvSelected.Items.Find(e.Item.Name, true);
                    if (otherItem != null && otherItem.Length == 1)
                    {
                        lvSelected.Items.Remove(otherItem[0]);
                    }
                }
            }
            finally
            {
                lvSelected.EndUpdate();
                lapList.EndUpdate();
                ignoreClicks = false;
            }
        }

        private bool ignoreClicks = false;

        public List<LapInfo> GetSelectedLaps()
        {
            if (lvSelected.Items.Count == 0)
            {
                return null;
            }
            List<LapInfo> checkedList = new List<LapInfo>();
            foreach(ListViewItem item in lvSelected.Items)
            {
                checkedList.Add(LapDatabase.Laps.First(l => l.FileName == item.Name));
            }
            return checkedList;
        }

        private void lvSelected_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (ignoreClicks) return;
            ignoreClicks = true;
            List<LapInfo> checkedList = new List<LapInfo>();
            lvSelected.BeginUpdate();
            lapList.BeginUpdate();
            try
            {
                if (! e.Item.Checked)
                {
                    var otherItem = lapList.Items.Find(e.Item.Name, true);
                    if (otherItem == null || otherItem.Length == 0)
                    {
                        // not found
                        otherItem = null;
                    }
                    else
                    {
                        otherItem[0].Checked = false;
                        lvSelected.Items.Remove(e.Item);
                    }
                }
            }
            finally
            {
                lvSelected.EndUpdate();
                lapList.EndUpdate();
                ignoreClicks = false;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (ignoreClicks) return;
            using (new HourGlass())
            {
                ignoreClicks = true;
                lvSelected.BeginUpdate();
                lapList.BeginUpdate();
                try
                {
                    foreach (ListViewItem item in lapList.CheckedItems)
                    {
                        LapDatabase.RemoveLap(LapDatabase.Laps.FirstOrDefault(la =>
                            la.CircuitName == item.SubItems[0].Text &&
                            la.SessionType == item.SubItems[1].Text &&
                            la.CarNumber == item.SubItems[2].Text &&
                            la.PlayerName == item.SubItems[3].Text &&
                            la.TeamName == item.SubItems[4].Text &&
                            la.LapTime.ToString("0.000") == item.SubItems[5].Text));
                        lapList.Items.Remove(item);
                    }
                    while (lvSelected.Items.Count > 0)
                    {
                        lvSelected.Items.RemoveAt(0);
                    }
                }
                finally
                {
                    Application.UseWaitCursor = false;
                    lvSelected.EndUpdate();
                    lapList.EndUpdate();
                    ignoreClicks = false;
                }
            }
        }

        private void lapList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewItemComparer comparer = null;
            if (lapList.ListViewItemSorter is ListViewItemComparer)
            {
                comparer = (ListViewItemComparer)lapList.ListViewItemSorter;
            }

            if (e.Column == comparer?.Col)
            {
                comparer.ReverseOrder();
            }
            else
            {
                ListViewItemComparer sorter = new ListViewItemComparer(e.Column, false);
                lapList.ListViewItemSorter = sorter;
            }
            lapList.Sort();
        }

        private void lapList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }

   public class HourGlass : IDisposable
    {
        public HourGlass()
        {
            Enabled = true;
        }
        public void Dispose()
        {
            Enabled = false;
        }
        public static bool Enabled
        {
            get { return Application.UseWaitCursor; }
            set
            {
                if (value == Application.UseWaitCursor) return;
                Application.UseWaitCursor = value;
                Form f = Form.ActiveForm;
                if (f != null && f.Handle != null)   // Send WM_SETCURSOR
                    SendMessage(f.Handle, 0x20, f.Handle, (IntPtr)1);
            }
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
    }

   
}

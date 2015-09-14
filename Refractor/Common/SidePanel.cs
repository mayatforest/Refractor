using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Refractor.Common
{
    /// <summary>
    /// Local ignores for a diagram. The main panel from here is added as a side 
    /// panel to the base diagram form.
    /// </summary>
    public partial class SidePanel : Form
    {
        public DRefreshDiagram RefreshDiagram;
        public DHidePane HidePane;

        public SidePanel()
        {
            InitializeComponent();
            _grid.Rows.Clear();
        }

        public void SetCaption(string caption)
        {
            _grid.Columns[0].HeaderText = caption;
        }

        public virtual void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _windowManager = (WindowManager)serviceProvider.GetService(typeof(IWindowManager));
            _lookup = new LookupHelper(_windowManager);
        }

        public void Toggle(BaseItem item)
        {
            if (item == null) return;

            if (_rows.ContainsKey(item))
            {
                RemoveItem(item);
            }
            else
            {
                AddItem(item);
            }
        }

        public void ToggleList(List<BaseItem> list)
        {
            if (list == null) return;

            foreach (BaseItem item in list)
            {
                Toggle(item);
            }
        }

        public void RemoveItem(BaseItem item)
        {
            if (!_rows.ContainsKey(item)) return;
                
            _grid.Rows.Remove(_rows[item]);
            _rows.Remove(item);
            _lookup.Remove(item.GetID());
        }

        public void AddItem(BaseItem item)
        {
            if (_rows.ContainsKey(item)) return;

            _grid.Rows.Add(new object[] { item.Name });
            _grid.CommitEdit(DataGridViewDataErrorContexts.CurrentCellChange);
            DataGridViewRow row = _grid.Rows[_grid.Rows.Count - 1];

            _rows.Add(item, row);

            row.Tag = item;

            row.Selected = true;

            _lookup.Add(item.GetID(), item);
        }

        public bool Lookup(string id)
        {
            return _lookup.Find(id) != null;
        }

        public void Clear()
        {
            if (_rows.Count == 0) return;

            _grid.Rows.Clear();
            _rows.Clear();
            _lookup.Clear();

            if (AutoRefreshCheckBox.Checked) RefreshDiagram();   
        }

        public int Count { get { return _rows.Count; } }

        public List<BaseItem> GetItems()
        {
            List<BaseItem> result = new List<BaseItem>();
            foreach (KeyValuePair<BaseItem, DataGridViewRow> pair in _rows)
            {
                result.Add(pair.Key);
            }
            return result;
        }

        // todo, multiple items per row. 
        private WindowManager _windowManager;
        private Dictionary<BaseItem, DataGridViewRow> _rows = new Dictionary<BaseItem, DataGridViewRow>();
        private LookupHelper _lookup;
        
        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RefreshDiagram == null) return;

            RefreshDiagram();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_grid.SelectedRows.Count == 0) return;

            BaseItem item = _grid.SelectedRows[0].Tag as BaseItem;
            if (item == null) return;

            RemoveItem(item);

            if (AutoRefreshCheckBox.Checked) RefreshDiagram();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HidePane();
        }

        private void _grid_DoubleClick(object sender, EventArgs e)
        {
            removeToolStripMenuItem_Click(null, null);
        }

        private void _grid_Click(object sender, EventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                removeToolStripMenuItem_Click(null, null);
            }
        }

        private void panel1_DoubleClick(object sender, EventArgs e)
        {
            HidePane();
        }

        private void MainPanel_Paint(object sender, PaintEventArgs e)
        {

        }

    }

}

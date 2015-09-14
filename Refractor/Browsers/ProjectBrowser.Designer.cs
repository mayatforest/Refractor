namespace Refractor
{
	partial class ProjectBrowser
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param itemPath="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.treeColumn1 = new Aga.Controls.Tree.TreeColumn();
            this.treeColumn2 = new Aga.Controls.Tree.TreeColumn();
            this.treeColumn3 = new Aga.Controls.Tree.TreeColumn();
            this._icon = new Aga.Controls.Tree.NodeControls.NodeStateIcon();
            this._name = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this._treeView = new Aga.Controls.Tree.TreeViewAdv();
            this.treeColumn7 = new Aga.Controls.Tree.TreeColumn();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.workerEnabledToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkLookupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._count = new Aga.Controls.Tree.NodeControls.NodeIntegerTextBox();
            this._total = new Aga.Controls.Tree.NodeControls.NodeIntegerTextBox();
            this._kind = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeColumn1
            // 
            this.treeColumn1.Header = "Name";
            this.treeColumn1.Sortable = true;
            this.treeColumn1.SortOrder = System.Windows.Forms.SortOrder.None;
            this.treeColumn1.TooltipText = "Item Name";
            this.treeColumn1.Width = 200;
            // 
            // treeColumn2
            // 
            this.treeColumn2.Header = "Count";
            this.treeColumn2.Sortable = true;
            this.treeColumn2.SortOrder = System.Windows.Forms.SortOrder.None;
            this.treeColumn2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.treeColumn2.TooltipText = "Count of child items";
            // 
            // treeColumn3
            // 
            this.treeColumn3.Header = "Total";
            this.treeColumn3.Sortable = true;
            this.treeColumn3.SortOrder = System.Windows.Forms.SortOrder.None;
            this.treeColumn3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.treeColumn3.TooltipText = "Total children";
            this.treeColumn3.Width = 60;
            // 
            // _icon
            // 
            this._icon.DataPropertyName = "Icon";
            this._icon.LeftMargin = 1;
            this._icon.ParentColumn = this.treeColumn1;
            // 
            // _name
            // 
            this._name.DataPropertyName = "Name";
            this._name.EditEnabled = false;
            this._name.IncrementalSearchEnabled = true;
            this._name.LeftMargin = 3;
            this._name.ParentColumn = this.treeColumn1;
            this._name.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            this._name.UseCompatibleTextRendering = true;
            // 
            // _treeView
            // 
            this._treeView.AllowColumnReorder = true;
            this._treeView.AllowDrop = true;
            this._treeView.AutoRowHeight = true;
            this._treeView.BackColor = System.Drawing.SystemColors.Window;
            this._treeView.Columns.Add(this.treeColumn1);
            this._treeView.Columns.Add(this.treeColumn2);
            this._treeView.Columns.Add(this.treeColumn3);
            this._treeView.Columns.Add(this.treeColumn7);
            this._treeView.ContextMenuStrip = this.contextMenuStrip;
            this._treeView.Cursor = System.Windows.Forms.Cursors.Default;
            this._treeView.DefaultToolTipProvider = null;
            this._treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._treeView.DragDropMarkColor = System.Drawing.Color.Black;
            this._treeView.FullRowSelect = true;
            this._treeView.LineColor = System.Drawing.SystemColors.ControlDark;
            this._treeView.LoadOnDemand = true;
            this._treeView.Location = new System.Drawing.Point(0, 0);
            this._treeView.Model = null;
            this._treeView.Name = "_treeView";
            this._treeView.NodeControls.Add(this._icon);
            this._treeView.NodeControls.Add(this._name);
            this._treeView.NodeControls.Add(this._count);
            this._treeView.NodeControls.Add(this._total);
            this._treeView.NodeControls.Add(this._kind);
            this._treeView.RowHeight = 15;
            this._treeView.SelectedNode = null;
            this._treeView.Size = new System.Drawing.Size(527, 482);
            this._treeView.TabIndex = 0;
            this._treeView.UseColumns = true;
            this._treeView.SelectionChanged += new System.EventHandler(this._treeView_SelectionChanged);
            this._treeView.DragOver += new System.Windows.Forms.DragEventHandler(this._treeView_DragOver);
            this._treeView.ColumnClicked += new System.EventHandler<Aga.Controls.Tree.TreeColumnEventArgs>(this._treeView_ColumnClicked);
            this._treeView.DragDrop += new System.Windows.Forms.DragEventHandler(this._treeView_DragDrop);
            this._treeView.ColumnReordered += new System.EventHandler<Aga.Controls.Tree.TreeColumnEventArgs>(this._treeView_ColumnReordered);
            this._treeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this._treeView_KeyDown);
            this._treeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this._treeView_ItemDrag);
            this._treeView.Click += new System.EventHandler(this._treeView_Click);
            // 
            // treeColumn7
            // 
            this.treeColumn7.Header = "Kind";
            this.treeColumn7.Sortable = true;
            this.treeColumn7.SortOrder = System.Windows.Forms.SortOrder.None;
            this.treeColumn7.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.treeColumn7.TooltipText = "Classification";
            this.treeColumn7.Width = 100;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandAllToolStripMenuItem,
            this.collapseAllToolStripMenuItem,
            this.toolStripMenuItem2,
            this.OpenToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripMenuItem1,
            this.workerEnabledToolStripMenuItem,
            this.checkLookupToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(162, 148);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // expandAllToolStripMenuItem
            // 
            this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.expandAllToolStripMenuItem.Text = "Expand All";
            this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem_Click);
            // 
            // collapseAllToolStripMenuItem
            // 
            this.collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
            this.collapseAllToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.collapseAllToolStripMenuItem.Text = "Collapse All";
            this.collapseAllToolStripMenuItem.Click += new System.EventHandler(this.collapseAllToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(158, 6);
            // 
            // OpenToolStripMenuItem
            // 
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            this.OpenToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.OpenToolStripMenuItem.Text = "Open File";
            this.OpenToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.deleteToolStripMenuItem.Text = "Remove File";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(158, 6);
            // 
            // workerEnabledToolStripMenuItem
            // 
            this.workerEnabledToolStripMenuItem.Checked = true;
            this.workerEnabledToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.workerEnabledToolStripMenuItem.Name = "workerEnabledToolStripMenuItem";
            this.workerEnabledToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.workerEnabledToolStripMenuItem.Text = "Worker Enabled";
            this.workerEnabledToolStripMenuItem.Click += new System.EventHandler(this.workerEnabledToolStripMenuItem_Click);
            // 
            // checkLookupToolStripMenuItem
            // 
            this.checkLookupToolStripMenuItem.Name = "checkLookupToolStripMenuItem";
            this.checkLookupToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.checkLookupToolStripMenuItem.Text = "Check Lookup";
            this.checkLookupToolStripMenuItem.Click += new System.EventHandler(this.checkLookupToolStripMenuItem_Click);
            // 
            // _count
            // 
            this._count.DataPropertyName = "Count";
            this._count.EditEnabled = false;
            this._count.IncrementalSearchEnabled = true;
            this._count.LeftMargin = 3;
            this._count.ParentColumn = this.treeColumn2;
            this._count.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // _total
            // 
            this._total.DataPropertyName = "Total";
            this._total.EditEnabled = false;
            this._total.IncrementalSearchEnabled = true;
            this._total.LeftMargin = 3;
            this._total.ParentColumn = this.treeColumn3;
            this._total.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // _kind
            // 
            this._kind.DataPropertyName = "Kind";
            this._kind.EditEnabled = false;
            this._kind.IncrementalSearchEnabled = true;
            this._kind.LeftMargin = 3;
            this._kind.ParentColumn = this.treeColumn7;
            this._kind.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this._kind.Trimming = System.Drawing.StringTrimming.EllipsisWord;
            // 
            // ProjectBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(527, 482);
            this.Controls.Add(this._treeView);
            this.HideOnClose = true;
            this.Name = "ProjectBrowser";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
            this.TabText = "Project Browser";
            this.Text = "Project Browser";
            this.Load += new System.EventHandler(this.ProjectBrowser_Load);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

        private Aga.Controls.Tree.NodeControls.NodeStateIcon _icon;
        private Aga.Controls.Tree.NodeControls.NodeTextBox _name;
        private Aga.Controls.Tree.TreeColumn treeColumn1;
        private Aga.Controls.Tree.TreeColumn treeColumn2;
        private Aga.Controls.Tree.TreeColumn treeColumn3;

        private Aga.Controls.Tree.TreeViewAdv _treeView;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private Aga.Controls.Tree.TreeColumn treeColumn7;
        private Aga.Controls.Tree.NodeControls.NodeIntegerTextBox _count;
        private Aga.Controls.Tree.NodeControls.NodeIntegerTextBox _total;
        private Aga.Controls.Tree.NodeControls.NodeTextBox _kind;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem workerEnabledToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkLookupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
	}
}

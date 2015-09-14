namespace Refractor
{
    partial class FavouritesBrowser
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
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._nodeTextBox = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.treeColumn2 = new Aga.Controls.Tree.TreeColumn();
            this._tree = new Aga.Controls.Tree.TreeViewAdv();
            this.treeColumn1 = new Aga.Controls.Tree.TreeColumn();
            this.nodeStateIcon1 = new Aga.Controls.Tree.NodeControls.NodeStateIcon();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newFolderToolStripMenuItem,
            this.removeItemToolStripMenuItem,
            this.clearToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(140, 70);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // newFolderToolStripMenuItem
            // 
            this.newFolderToolStripMenuItem.Name = "newFolderToolStripMenuItem";
            this.newFolderToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.newFolderToolStripMenuItem.Text = "New Folder";
            this.newFolderToolStripMenuItem.Click += new System.EventHandler(this.newFolderToolStripMenuItem_Click);
            // 
            // removeItemToolStripMenuItem
            // 
            this.removeItemToolStripMenuItem.Name = "removeItemToolStripMenuItem";
            this.removeItemToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.removeItemToolStripMenuItem.Text = "Remove";
            this.removeItemToolStripMenuItem.Click += new System.EventHandler(this.removeItemToolStripMenuItem_Click);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // _nodeTextBox
            // 
            this._nodeTextBox.DataPropertyName = "Text";
            this._nodeTextBox.IncrementalSearchEnabled = true;
            this._nodeTextBox.LeftMargin = 3;
            this._nodeTextBox.ParentColumn = this.treeColumn2;
            // 
            // treeColumn2
            // 
            this.treeColumn2.Header = "";
            this.treeColumn2.SortOrder = System.Windows.Forms.SortOrder.None;
            this.treeColumn2.TooltipText = null;
            // 
            // _tree
            // 
            this._tree.AllowDrop = true;
            this._tree.AutoRowHeight = true;
            this._tree.BackColor = System.Drawing.SystemColors.Window;
            this._tree.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this._tree.Columns.Add(this.treeColumn1);
            this._tree.Columns.Add(this.treeColumn2);
            this._tree.ContextMenuStrip = this.contextMenuStrip1;
            this._tree.Cursor = System.Windows.Forms.Cursors.Default;
            this._tree.DefaultToolTipProvider = null;
            this._tree.DisplayDraggingNodes = true;
            this._tree.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tree.DragDropMarkColor = System.Drawing.Color.Black;
            this._tree.LineColor = System.Drawing.SystemColors.ControlDark;
            this._tree.LoadOnDemand = true;
            this._tree.Location = new System.Drawing.Point(0, 0);
            this._tree.Model = null;
            this._tree.Name = "_tree";
            this._tree.NodeControls.Add(this.nodeStateIcon1);
            this._tree.NodeControls.Add(this._nodeTextBox);
            this._tree.SelectedNode = null;
            this._tree.SelectionMode = Aga.Controls.Tree.TreeSelectionMode.MultiSameParent;
            this._tree.ShowNodeToolTips = true;
            this._tree.Size = new System.Drawing.Size(568, 288);
            this._tree.TabIndex = 1;
            this._tree.SelectionChanged += new System.EventHandler(this._tree_SelectionChanged);
            this._tree.DragOver += new System.Windows.Forms.DragEventHandler(this._tree_DragOver);
            this._tree.DoubleClick += new System.EventHandler(this._tree_DoubleClick);
            this._tree.DragDrop += new System.Windows.Forms.DragEventHandler(this._tree_DragDrop);
            this._tree.NodeMouseClick += new System.EventHandler<Aga.Controls.Tree.TreeNodeAdvMouseEventArgs>(this._tree_NodeMouseClick);
            this._tree.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this._tree_ItemDrag);
            // 
            // treeColumn1
            // 
            this.treeColumn1.Header = "";
            this.treeColumn1.SortOrder = System.Windows.Forms.SortOrder.None;
            this.treeColumn1.TooltipText = null;
            // 
            // nodeStateIcon1
            // 
            this.nodeStateIcon1.DataPropertyName = "Icon";
            this.nodeStateIcon1.LeftMargin = 1;
            this.nodeStateIcon1.ParentColumn = this.treeColumn1;
            // 
            // FavouritesBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(568, 288);
            this.Controls.Add(this._tree);
            this.HideOnClose = true;
            this.Name = "FavouritesBrowser";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottom;
            this.TabText = "Favourites";
            this.Text = "Favourites";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeItemToolStripMenuItem;
        private Aga.Controls.Tree.NodeControls.NodeTextBox _nodeTextBox;
        protected Aga.Controls.Tree.TreeViewAdv _tree;
        private Aga.Controls.Tree.TreeColumn treeColumn2;
        private Aga.Controls.Tree.TreeColumn treeColumn1;
        private Aga.Controls.Tree.NodeControls.NodeStateIcon nodeStateIcon1;

    }
}

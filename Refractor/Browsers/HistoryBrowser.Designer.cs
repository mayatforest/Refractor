namespace Refractor
{
    partial class HistoryBrowser
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
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goBackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goForwardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._treeView = new Aga.Controls.Tree.TreeViewAdv();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem,
            this.goBackToolStripMenuItem,
            this.goForwardToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 92);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            // 
            // goBackToolStripMenuItem
            // 
            this.goBackToolStripMenuItem.Name = "goBackToolStripMenuItem";
            this.goBackToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.goBackToolStripMenuItem.Text = "Go Back";
            this.goBackToolStripMenuItem.Click += new System.EventHandler(this.goBackToolStripMenuItem_Click);
            // 
            // goForwardToolStripMenuItem
            // 
            this.goForwardToolStripMenuItem.Name = "goForwardToolStripMenuItem";
            this.goForwardToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.goForwardToolStripMenuItem.Text = "Go Forward";
            this.goForwardToolStripMenuItem.Click += new System.EventHandler(this.goForwardToolStripMenuItem_Click);
            // 
            // _treeView
            // 
            this._treeView.AllowDrop = true;
            this._treeView.BackColor = System.Drawing.SystemColors.Window;
            this._treeView.ContextMenuStrip = this.contextMenuStrip1;
            this._treeView.Cursor = System.Windows.Forms.Cursors.Default;
            this._treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._treeView.DragDropMarkColor = System.Drawing.Color.Black;
            this._treeView.LineColor = System.Drawing.SystemColors.ControlDark;
            this._treeView.Location = new System.Drawing.Point(0, 0);
            this._treeView.Model = null;
            this._treeView.Name = "_treeView";
            this._treeView.SelectedNode = null;
            this._treeView.SelectionMode = Aga.Controls.Tree.TreeSelectionMode.MultiSameParent;
            this._treeView.ShowNodeToolTips = true;
            this._treeView.Size = new System.Drawing.Size(394, 393);
            this._treeView.TabIndex = 1;
            this._treeView.Text = "treeViewAdv1";
            this._treeView.SelectionChanged += new System.EventHandler(this._treeView_SelectionChanged);
            // 
            // HistoryBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 393);
            this.Controls.Add(this._treeView);
            this.HideOnClose = true;
            this.Name = "HistoryBrowser";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottom;
            this.TabText = "History";
            this.Text = "History";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private Aga.Controls.Tree.TreeViewAdv _treeView;
        private System.Windows.Forms.ToolStripMenuItem goBackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goForwardToolStripMenuItem;

    }
}

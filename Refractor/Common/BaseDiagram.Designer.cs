namespace Refractor.Common
{
    partial class BaseDiagram
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
            this.contextMenuStripDefault = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.diagramOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showMultipleEdgesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showMultipleEgdesAsThickLinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useLeftToRightLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showOrphanNodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showAllHiddenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideShowLocalIgnoresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleLocalIgnoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblCaption = new System.Windows.Forms.Label();
            this.lblPleaseWait = new System.Windows.Forms.Label();
            this.panelSpacerTransparent = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.contextMenuStripDefault.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // contextMenuStripDefault
            // 
            this.contextMenuStripDefault.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem,
            this.diagramOptionsToolStripMenuItem,
            this.hideNodeToolStripMenuItem,
            this.showAllHiddenToolStripMenuItem,
            this.hideShowLocalIgnoresToolStripMenuItem,
            this.toggleLocalIgnoreToolStripMenuItem});
            this.contextMenuStripDefault.Name = "contextMenuStripDefault";
            this.contextMenuStripDefault.Size = new System.Drawing.Size(258, 136);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // diagramOptionsToolStripMenuItem
            // 
            this.diagramOptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showMultipleEdgesToolStripMenuItem,
            this.showMultipleEgdesAsThickLinesToolStripMenuItem,
            this.useLeftToRightLayoutToolStripMenuItem,
            this.showOrphanNodesToolStripMenuItem});
            this.diagramOptionsToolStripMenuItem.Name = "diagramOptionsToolStripMenuItem";
            this.diagramOptionsToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.diagramOptionsToolStripMenuItem.Text = "Diagram Options";
            // 
            // showMultipleEdgesToolStripMenuItem
            // 
            this.showMultipleEdgesToolStripMenuItem.Name = "showMultipleEdgesToolStripMenuItem";
            this.showMultipleEdgesToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.showMultipleEdgesToolStripMenuItem.Text = "Show Multiple Edges";
            this.showMultipleEdgesToolStripMenuItem.Click += new System.EventHandler(this.showMultipleEdgesToolStripMenuItem_Click);
            // 
            // showMultipleEgdesAsThickLinesToolStripMenuItem
            // 
            this.showMultipleEgdesAsThickLinesToolStripMenuItem.Name = "showMultipleEgdesAsThickLinesToolStripMenuItem";
            this.showMultipleEgdesAsThickLinesToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.showMultipleEgdesAsThickLinesToolStripMenuItem.Text = "Show Multiple Egdes as thick lines";
            this.showMultipleEgdesAsThickLinesToolStripMenuItem.Click += new System.EventHandler(this.showMultipleEgdesAsThickLinesToolStripMenuItem_Click);
            // 
            // useLeftToRightLayoutToolStripMenuItem
            // 
            this.useLeftToRightLayoutToolStripMenuItem.Name = "useLeftToRightLayoutToolStripMenuItem";
            this.useLeftToRightLayoutToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.useLeftToRightLayoutToolStripMenuItem.Text = "Use Left To Right layout";
            this.useLeftToRightLayoutToolStripMenuItem.Click += new System.EventHandler(this.useLeftToRightLayoutToolStripMenuItem_Click);
            // 
            // showOrphanNodesToolStripMenuItem
            // 
            this.showOrphanNodesToolStripMenuItem.Name = "showOrphanNodesToolStripMenuItem";
            this.showOrphanNodesToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            this.showOrphanNodesToolStripMenuItem.Text = "Show Orphan Nodes";
            this.showOrphanNodesToolStripMenuItem.Click += new System.EventHandler(this.showOrphanNodesToolStripMenuItem_Click);
            // 
            // hideNodeToolStripMenuItem
            // 
            this.hideNodeToolStripMenuItem.Name = "hideNodeToolStripMenuItem";
            this.hideNodeToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.hideNodeToolStripMenuItem.Text = "Hide Node";
            this.hideNodeToolStripMenuItem.Click += new System.EventHandler(this.hideNodeToolStripMenuItem_Click);
            // 
            // showAllHiddenToolStripMenuItem
            // 
            this.showAllHiddenToolStripMenuItem.Name = "showAllHiddenToolStripMenuItem";
            this.showAllHiddenToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.showAllHiddenToolStripMenuItem.Text = "Show all hidden";
            this.showAllHiddenToolStripMenuItem.Click += new System.EventHandler(this.showAllHiddenToolStripMenuItem_Click);
            // 
            // hideShowLocalIgnoresToolStripMenuItem
            // 
            this.hideShowLocalIgnoresToolStripMenuItem.Name = "hideShowLocalIgnoresToolStripMenuItem";
            this.hideShowLocalIgnoresToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.hideShowLocalIgnoresToolStripMenuItem.Text = "Hide/show side panel [Double click]";
            this.hideShowLocalIgnoresToolStripMenuItem.Click += new System.EventHandler(this.hideShowLocalIgnoresToolStripMenuItem_Click);
            // 
            // toggleLocalIgnoreToolStripMenuItem
            // 
            this.toggleLocalIgnoreToolStripMenuItem.Name = "toggleLocalIgnoreToolStripMenuItem";
            this.toggleLocalIgnoreToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.toggleLocalIgnoreToolStripMenuItem.Text = "Toggle side panel include [Ctrl-Click]";
            this.toggleLocalIgnoreToolStripMenuItem.Click += new System.EventHandler(this.toggleSidePanelToolStripMenuItem_Click);
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(309, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(117, 352);
            this.panel2.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnRefresh);
            this.panel1.Controls.Add(this.lblCaption);
            this.panel1.Controls.Add(this.lblPleaseWait);
            this.panel1.Controls.Add(this.panelSpacerTransparent);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(309, 352);
            this.panel1.TabIndex = 3;
            this.panel1.DoubleClick += new System.EventHandler(this.panel1_DoubleClick);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(15, 34);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Visible = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lblCaption
            // 
            this.lblCaption.AutoSize = true;
            this.lblCaption.Location = new System.Drawing.Point(168, 9);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(58, 13);
            this.lblCaption.TabIndex = 4;
            this.lblCaption.Text = "Item Name";
            this.lblCaption.Click += new System.EventHandler(this.lblCaption_Click);
            // 
            // lblPleaseWait
            // 
            this.lblPleaseWait.Location = new System.Drawing.Point(12, 9);
            this.lblPleaseWait.Name = "lblPleaseWait";
            this.lblPleaseWait.Size = new System.Drawing.Size(114, 22);
            this.lblPleaseWait.TabIndex = 4;
            this.lblPleaseWait.Text = "Please wait...";
            // 
            // panelSpacerTransparent
            // 
            this.panelSpacerTransparent.BackColor = System.Drawing.Color.Transparent;
            this.panelSpacerTransparent.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelSpacerTransparent.Location = new System.Drawing.Point(305, 0);
            this.panelSpacerTransparent.Name = "panelSpacerTransparent";
            this.panelSpacerTransparent.Size = new System.Drawing.Size(4, 352);
            this.panelSpacerTransparent.TabIndex = 3;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(304, 0);
            this.splitter1.MinExtra = 0;
            this.splitter1.MinSize = 0;
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(5, 352);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // BaseDiagram
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 352);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.HideOnClose = true;
            this.Name = "BaseDiagram";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BaseDiagram_FormClosed);
            this.contextMenuStripDefault.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripDefault;
        private System.Windows.Forms.ToolStripMenuItem hideNodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showAllHiddenToolStripMenuItem;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolStripMenuItem hideShowLocalIgnoresToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleLocalIgnoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem diagramOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showMultipleEdgesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showMultipleEgdesAsThickLinesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useLeftToRightLayoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showOrphanNodesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.Panel panelSpacerTransparent;
        private System.Windows.Forms.Label lblPleaseWait;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblCaption;
    }
}

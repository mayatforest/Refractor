namespace Refractor
{
	partial class MainForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuItemHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemNewWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.toolBarButtonLogWindow = new System.Windows.Forms.ToolStripButton();
            this.toolBarButtonSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolBarButtonOpen = new System.Windows.Forms.ToolStripButton();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.toolBarButtonPropertyWindow = new System.Windows.Forms.ToolStripButton();
            this.toolBarButtonProjectBrowser = new System.Windows.Forms.ToolStripButton();
            this.toolBarButtonSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.toolBar = new System.Windows.Forms.ToolStrip();
            this.toolBarButtonNew = new System.Windows.Forms.ToolStripButton();
            this.toolBarButtonOpenProject = new System.Windows.Forms.ToolStripButton();
            this.toolBarButtonSaveProject = new System.Windows.Forms.ToolStripButton();
            this.toolBarButtonFavourites = new System.Windows.Forms.ToolStripButton();
            this.toolBarButtonGoBack = new System.Windows.Forms.ToolStripButton();
            this.toolBarButtonGoForward = new System.Windows.Forms.ToolStripButton();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.menuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitWithoutSavingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.favouritesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.historyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.goBackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goForwardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.fullScreenF11ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemTools = new System.Windows.Forms.ToolStripMenuItem();
            this.lockLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.toolBar.SuspendLayout();
            this.mainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuItemHelp
            // 
            this.menuItemHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.menuItemHelp.MergeIndex = 3;
            this.menuItemHelp.Name = "menuItemHelp";
            this.menuItemHelp.Size = new System.Drawing.Size(40, 20);
            this.menuItemHelp.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // menuItemNewWindow
            // 
            this.menuItemNewWindow.Name = "menuItemNewWindow";
            this.menuItemNewWindow.Size = new System.Drawing.Size(147, 22);
            this.menuItemNewWindow.Text = "&New Window";
            this.menuItemNewWindow.Visible = false;
            // 
            // menuItemWindow
            // 
            this.menuItemWindow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemNewWindow});
            this.menuItemWindow.MergeIndex = 2;
            this.menuItemWindow.Name = "menuItemWindow";
            this.menuItemWindow.Size = new System.Drawing.Size(57, 20);
            this.menuItemWindow.Text = "&Window";
            // 
            // toolBarButtonLogWindow
            // 
            this.toolBarButtonLogWindow.ImageIndex = 5;
            this.toolBarButtonLogWindow.Name = "toolBarButtonLogWindow";
            this.toolBarButtonLogWindow.Size = new System.Drawing.Size(23, 22);
            this.toolBarButtonLogWindow.ToolTipText = "View Log Window";
            this.toolBarButtonLogWindow.Click += new System.EventHandler(this.logViewToolStripMenuItem_Click);
            // 
            // toolBarButtonSeparator2
            // 
            this.toolBarButtonSeparator2.Name = "toolBarButtonSeparator2";
            this.toolBarButtonSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolBarButtonOpen
            // 
            this.toolBarButtonOpen.ImageIndex = 1;
            this.toolBarButtonOpen.Name = "toolBarButtonOpen";
            this.toolBarButtonOpen.Size = new System.Drawing.Size(23, 22);
            this.toolBarButtonOpen.ToolTipText = "Open File(s)";
            this.toolBarButtonOpen.Click += new System.EventHandler(this.menuItemOpen_Click);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "");
            this.imageList.Images.SetKeyName(1, "");
            this.imageList.Images.SetKeyName(2, "");
            this.imageList.Images.SetKeyName(3, "");
            this.imageList.Images.SetKeyName(4, "");
            this.imageList.Images.SetKeyName(5, "");
            this.imageList.Images.SetKeyName(6, "");
            this.imageList.Images.SetKeyName(7, "");
            this.imageList.Images.SetKeyName(8, "");
            // 
            // toolBarButtonPropertyWindow
            // 
            this.toolBarButtonPropertyWindow.ImageIndex = 3;
            this.toolBarButtonPropertyWindow.Name = "toolBarButtonPropertyWindow";
            this.toolBarButtonPropertyWindow.Size = new System.Drawing.Size(23, 22);
            this.toolBarButtonPropertyWindow.ToolTipText = "Property Window";
            this.toolBarButtonPropertyWindow.Visible = false;
            // 
            // toolBarButtonProjectBrowser
            // 
            this.toolBarButtonProjectBrowser.ImageIndex = 2;
            this.toolBarButtonProjectBrowser.Name = "toolBarButtonProjectBrowser";
            this.toolBarButtonProjectBrowser.Size = new System.Drawing.Size(23, 22);
            this.toolBarButtonProjectBrowser.ToolTipText = "View Project Browser";
            this.toolBarButtonProjectBrowser.Click += new System.EventHandler(this.projectBrowserToolStripMenuItem_Click);
            // 
            // toolBarButtonSeparator1
            // 
            this.toolBarButtonSeparator1.Name = "toolBarButtonSeparator1";
            this.toolBarButtonSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // menuItem4
            // 
            this.menuItem4.Name = "menuItem4";
            this.menuItem4.Size = new System.Drawing.Size(173, 6);
            // 
            // menuItemExit
            // 
            this.menuItemExit.Name = "menuItemExit";
            this.menuItemExit.Size = new System.Drawing.Size(176, 22);
            this.menuItemExit.Text = "&Exit";
            this.menuItemExit.Click += new System.EventHandler(this.menuItemExit_Click);
            // 
            // dockPanel
            // 
            this.dockPanel.ActiveAutoHideContent = null;
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.dockPanel.Location = new System.Drawing.Point(0, 49);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.Size = new System.Drawing.Size(689, 411);
            this.dockPanel.TabIndex = 8;
            // 
            // toolBar
            // 
            this.toolBar.ImageList = this.imageList;
            this.toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolBarButtonNew,
            this.toolBarButtonOpen,
            this.toolBarButtonOpenProject,
            this.toolBarButtonSaveProject,
            this.toolBarButtonSeparator1,
            this.toolBarButtonProjectBrowser,
            this.toolBarButtonPropertyWindow,
            this.toolBarButtonLogWindow,
            this.toolBarButtonFavourites,
            this.toolBarButtonSeparator2,
            this.toolBarButtonGoBack,
            this.toolBarButtonGoForward});
            this.toolBar.Location = new System.Drawing.Point(0, 24);
            this.toolBar.Name = "toolBar";
            this.toolBar.Size = new System.Drawing.Size(689, 25);
            this.toolBar.TabIndex = 10;
            // 
            // toolBarButtonNew
            // 
            this.toolBarButtonNew.ImageIndex = 0;
            this.toolBarButtonNew.Name = "toolBarButtonNew";
            this.toolBarButtonNew.Size = new System.Drawing.Size(23, 22);
            this.toolBarButtonNew.ToolTipText = "New Project";
            this.toolBarButtonNew.Visible = false;
            this.toolBarButtonNew.Click += new System.EventHandler(this.newProjectToolStripMenuItem_Click);
            // 
            // toolBarButtonOpenProject
            // 
            this.toolBarButtonOpenProject.Image = ((System.Drawing.Image)(resources.GetObject("toolBarButtonOpenProject.Image")));
            this.toolBarButtonOpenProject.ImageTransparentColor = System.Drawing.Color.Black;
            this.toolBarButtonOpenProject.Name = "toolBarButtonOpenProject";
            this.toolBarButtonOpenProject.Size = new System.Drawing.Size(23, 22);
            this.toolBarButtonOpenProject.ToolTipText = "Open Project";
            this.toolBarButtonOpenProject.Click += new System.EventHandler(this.openProjectToolStripMenuItem_Click);
            // 
            // toolBarButtonSaveProject
            // 
            this.toolBarButtonSaveProject.Image = ((System.Drawing.Image)(resources.GetObject("toolBarButtonSaveProject.Image")));
            this.toolBarButtonSaveProject.ImageTransparentColor = System.Drawing.Color.Black;
            this.toolBarButtonSaveProject.Name = "toolBarButtonSaveProject";
            this.toolBarButtonSaveProject.Size = new System.Drawing.Size(23, 22);
            this.toolBarButtonSaveProject.ToolTipText = "Save Project";
            this.toolBarButtonSaveProject.Click += new System.EventHandler(this.saveProjectToolStripMenuItem_Click);
            // 
            // toolBarButtonFavourites
            // 
            this.toolBarButtonFavourites.Image = ((System.Drawing.Image)(resources.GetObject("toolBarButtonFavourites.Image")));
            this.toolBarButtonFavourites.ImageTransparentColor = System.Drawing.Color.Silver;
            this.toolBarButtonFavourites.Name = "toolBarButtonFavourites";
            this.toolBarButtonFavourites.Size = new System.Drawing.Size(23, 22);
            this.toolBarButtonFavourites.ToolTipText = "View Favourites";
            this.toolBarButtonFavourites.Click += new System.EventHandler(this.favouritesToolStripMenuItem_Click);
            // 
            // toolBarButtonGoBack
            // 
            this.toolBarButtonGoBack.Image = ((System.Drawing.Image)(resources.GetObject("toolBarButtonGoBack.Image")));
            this.toolBarButtonGoBack.ImageTransparentColor = System.Drawing.Color.Silver;
            this.toolBarButtonGoBack.Name = "toolBarButtonGoBack";
            this.toolBarButtonGoBack.Size = new System.Drawing.Size(23, 22);
            this.toolBarButtonGoBack.ToolTipText = "Go Back";
            this.toolBarButtonGoBack.Click += new System.EventHandler(this.goBackToolStripMenuItem_Click);
            // 
            // toolBarButtonGoForward
            // 
            this.toolBarButtonGoForward.Image = ((System.Drawing.Image)(resources.GetObject("toolBarButtonGoForward.Image")));
            this.toolBarButtonGoForward.ImageTransparentColor = System.Drawing.Color.Silver;
            this.toolBarButtonGoForward.Name = "toolBarButtonGoForward";
            this.toolBarButtonGoForward.Size = new System.Drawing.Size(23, 22);
            this.toolBarButtonGoForward.ToolTipText = "Go Forward";
            this.toolBarButtonGoForward.Click += new System.EventHandler(this.goForwardToolStripMenuItem_Click);
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemFile,
            this.viewToolStripMenuItem,
            this.menuItemTools,
            this.menuItemWindow,
            this.menuItemHelp});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.MdiWindowListItem = this.menuItemWindow;
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(689, 24);
            this.mainMenu.TabIndex = 11;
            // 
            // menuItemFile
            // 
            this.menuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem,
            this.menuItem4,
            this.menuItemOpen,
            this.openProjectToolStripMenuItem,
            this.saveProjectToolStripMenuItem,
            this.saveProjectAsToolStripMenuItem,
            this.toolStripMenuItem2,
            this.menuItemExit,
            this.exitWithoutSavingToolStripMenuItem});
            this.menuItemFile.Name = "menuItemFile";
            this.menuItemFile.Size = new System.Drawing.Size(35, 20);
            this.menuItemFile.Text = "&File";
            // 
            // newProjectToolStripMenuItem
            // 
            this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.newProjectToolStripMenuItem.Text = "New Project";
            this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.newProjectToolStripMenuItem_Click);
            // 
            // menuItemOpen
            // 
            this.menuItemOpen.Name = "menuItemOpen";
            this.menuItemOpen.Size = new System.Drawing.Size(176, 22);
            this.menuItemOpen.Text = "&Open...";
            this.menuItemOpen.Click += new System.EventHandler(this.menuItemOpen_Click);
            // 
            // openProjectToolStripMenuItem
            // 
            this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
            this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.openProjectToolStripMenuItem.Text = "Open Project...";
            this.openProjectToolStripMenuItem.Click += new System.EventHandler(this.openProjectToolStripMenuItem_Click);
            // 
            // saveProjectToolStripMenuItem
            // 
            this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.saveProjectToolStripMenuItem.Text = "Save Project";
            this.saveProjectToolStripMenuItem.Click += new System.EventHandler(this.saveProjectToolStripMenuItem_Click);
            // 
            // saveProjectAsToolStripMenuItem
            // 
            this.saveProjectAsToolStripMenuItem.Name = "saveProjectAsToolStripMenuItem";
            this.saveProjectAsToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.saveProjectAsToolStripMenuItem.Text = "Save Project &As...";
            this.saveProjectAsToolStripMenuItem.Click += new System.EventHandler(this.saveProjectAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(173, 6);
            // 
            // exitWithoutSavingToolStripMenuItem
            // 
            this.exitWithoutSavingToolStripMenuItem.Name = "exitWithoutSavingToolStripMenuItem";
            this.exitWithoutSavingToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.exitWithoutSavingToolStripMenuItem.Text = "Exit without saving";
            this.exitWithoutSavingToolStripMenuItem.Click += new System.EventHandler(this.exitWithoutSavingToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectBrowserToolStripMenuItem,
            this.favouritesToolStripMenuItem,
            this.historyToolStripMenuItem,
            this.logViewToolStripMenuItem,
            this.toolStripMenuItem1,
            this.goBackToolStripMenuItem,
            this.goForwardToolStripMenuItem,
            this.toolStripMenuItem3,
            this.fullScreenF11ToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // projectBrowserToolStripMenuItem
            // 
            this.projectBrowserToolStripMenuItem.Name = "projectBrowserToolStripMenuItem";
            this.projectBrowserToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.projectBrowserToolStripMenuItem.Text = "Project Browser [F12]";
            this.projectBrowserToolStripMenuItem.Click += new System.EventHandler(this.projectBrowserToolStripMenuItem_Click);
            // 
            // favouritesToolStripMenuItem
            // 
            this.favouritesToolStripMenuItem.Name = "favouritesToolStripMenuItem";
            this.favouritesToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.favouritesToolStripMenuItem.Text = "Favourites";
            this.favouritesToolStripMenuItem.Click += new System.EventHandler(this.favouritesToolStripMenuItem_Click);
            // 
            // historyToolStripMenuItem
            // 
            this.historyToolStripMenuItem.Name = "historyToolStripMenuItem";
            this.historyToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.historyToolStripMenuItem.Text = "History";
            this.historyToolStripMenuItem.Click += new System.EventHandler(this.historyToolStripMenuItem_Click);
            // 
            // logViewToolStripMenuItem
            // 
            this.logViewToolStripMenuItem.Name = "logViewToolStripMenuItem";
            this.logViewToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.logViewToolStripMenuItem.Text = "Log View";
            this.logViewToolStripMenuItem.Click += new System.EventHandler(this.logViewToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(167, 6);
            // 
            // goBackToolStripMenuItem
            // 
            this.goBackToolStripMenuItem.Name = "goBackToolStripMenuItem";
            this.goBackToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.goBackToolStripMenuItem.Text = "Go Back";
            this.goBackToolStripMenuItem.Click += new System.EventHandler(this.goBackToolStripMenuItem_Click);
            // 
            // goForwardToolStripMenuItem
            // 
            this.goForwardToolStripMenuItem.Name = "goForwardToolStripMenuItem";
            this.goForwardToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.goForwardToolStripMenuItem.Text = "Go Forward";
            this.goForwardToolStripMenuItem.Click += new System.EventHandler(this.goForwardToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(167, 6);
            // 
            // fullScreenF11ToolStripMenuItem
            // 
            this.fullScreenF11ToolStripMenuItem.Name = "fullScreenF11ToolStripMenuItem";
            this.fullScreenF11ToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.fullScreenF11ToolStripMenuItem.Text = "Full Screen [F11]";
            this.fullScreenF11ToolStripMenuItem.Click += new System.EventHandler(this.fullScreenF11ToolStripMenuItem_Click);
            // 
            // menuItemTools
            // 
            this.menuItemTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lockLayoutToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuItemTools.MergeIndex = 2;
            this.menuItemTools.Name = "menuItemTools";
            this.menuItemTools.Size = new System.Drawing.Size(44, 20);
            this.menuItemTools.Text = "&Tools";
            // 
            // lockLayoutToolStripMenuItem
            // 
            this.lockLayoutToolStripMenuItem.Name = "lockLayoutToolStripMenuItem";
            this.lockLayoutToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.lockLayoutToolStripMenuItem.Text = "&Lock Layout";
            this.lockLayoutToolStripMenuItem.Click += new System.EventHandler(this.lockLayoutToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.optionsToolStripMenuItem.Text = "Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 460);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(689, 22);
            this.statusBar.TabIndex = 9;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(689, 482);
            this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.toolBar);
            this.Controls.Add(this.mainMenu);
            this.Controls.Add(this.statusBar);
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "Refractor 0.3";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.MainForm_PreviewKeyDown);
            this.toolBar.ResumeLayout(false);
            this.toolBar.PerformLayout();
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.ToolStripMenuItem menuItemHelp;
        private System.Windows.Forms.ToolStripMenuItem menuItemNewWindow;
        private System.Windows.Forms.ToolStripMenuItem menuItemWindow;
        private System.Windows.Forms.ToolStripButton toolBarButtonLogWindow;
        private System.Windows.Forms.ToolStripSeparator toolBarButtonSeparator2;
        private System.Windows.Forms.ToolStripButton toolBarButtonOpen;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolStripButton toolBarButtonPropertyWindow;
        private System.Windows.Forms.ToolStripButton toolBarButtonProjectBrowser;
        private System.Windows.Forms.ToolStripSeparator toolBarButtonSeparator1;
        private System.Windows.Forms.ToolStripSeparator menuItem4;
        private System.Windows.Forms.ToolStripMenuItem menuItemExit;
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        private System.Windows.Forms.ToolStrip toolBar;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem menuItemFile;
        private System.Windows.Forms.ToolStripMenuItem menuItemOpen;
        private System.Windows.Forms.ToolStripMenuItem menuItemTools;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem projectBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem favouritesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolBarButtonNew;
        private System.Windows.Forms.ToolStripMenuItem saveProjectAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolBarButtonOpenProject;
        private System.Windows.Forms.ToolStripButton toolBarButtonFavourites;
        private System.Windows.Forms.ToolStripButton toolBarButtonSaveProject;
        private System.Windows.Forms.ToolStripMenuItem historyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goBackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goForwardToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolBarButtonGoBack;
        private System.Windows.Forms.ToolStripButton toolBarButtonGoForward;
        private System.Windows.Forms.ToolStripMenuItem lockLayoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitWithoutSavingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem fullScreenF11ToolStripMenuItem;


    }
}


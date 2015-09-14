using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

namespace Refractor
{
    internal partial class MainForm : Form
	{
		public MainForm()
		{
            // Make sure the SDIL static class is initialized.
            // This is an issue with the pulgin architecture.
            SDILReader.Globals.LoadOpCodes();

            InitializeComponent(); 

            // Start up the window mediator.
            _windowManager = new WindowManager(this, this.dockPanel, this.statusBar, this.mainMenu);
        }

        private DeserializeDockContent _deserializeDockContent;
        private WindowManager _windowManager;
        private Timer _timer;
        private string _initialProjectFilename;
        private bool _noSave = false;
        private bool _cancelActivatePlugins = false;

        private void MainForm_Load(object sender, EventArgs e)
        {
            _windowManager.Init();

            ApplyMainWindowSettings();
            LoadDockingConfig();

            // Make sure we're visible.
            this.Show();
            _windowManager.ProjectBrowser.Show(_windowManager.DockPanel);
            
            // Kick off the project load. 
            if (_windowManager.AppOptions.LoadLastProjectOnStartup)
            {
                LoadLastProject();
            }
        }
        
        private void LoadLastProject()
        {
            try
            {
                // Load default/previous project.
                if (string.IsNullOrEmpty(_initialProjectFilename))
                {
                    _windowManager.ProjectBrowser.NewProject();
                }
                else
                {
                    if (File.Exists(_initialProjectFilename))
                    {
                        _windowManager.ProjectBrowser.LoadProject(_initialProjectFilename, _cancelActivatePlugins);
                    }
                    else
                    {
                        _windowManager.Logger.LogStr("Warning : Failed to find project file : " + _initialProjectFilename);
                        _windowManager.ProjectBrowser.NewProject();
                    }
                }
            }
            catch (IOException exc)
            {
                _windowManager.Logger.LogExcStr(exc, "Failed on IO loading project");
            }
            catch (Exception exc)
            {
                if (_windowManager.Logger.LogCatchAll(exc, "Failed loading project")) throw;
            }
        }
        
        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = false;

            if (_noSave) return;

            if (_windowManager.AppOptions.SaveProjectOnExit)
            {                
                try
                {
                    // Save current project.
                    _windowManager.ProjectBrowser.SaveProject(null);
                }
                catch (InvalidOperationException exc)
                {
                    _windowManager.Logger.LogExcStr(exc, "Failed saving project");
                }
                catch (Exception exc)
                {
                    if (_windowManager.Logger.LogCatchAll(exc, "Failed saving project")) throw;
                }
            }

            StashMainWindowSettings();

            SaveDockingConfig();
            
            _windowManager.Finish();
        }

        private void MainForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // This does not fire for me. KeyPress fires, but does not trap function 
            // keys. KeyUp seems to fire and catch function keys.
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                // Avoid possibly slow inital load.
                _cancelActivatePlugins = true;
            }
            else if (e.KeyCode == Keys.F11)
            {
                fullScreenF11ToolStripMenuItem_Click(null, null);
            }
            else if (e.KeyCode == Keys.F12)
            {
                projectBrowserToolStripMenuItem_Click(null, null);
            }
        }


        private void logViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_windowManager.Logger.Visible)
                _windowManager.Logger.Hide();
            else 
                _windowManager.Logger.Show(dockPanel);
        }

        private void projectBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_windowManager.ProjectBrowser.Visible)
                _windowManager.ProjectBrowser.Hide();
            else
                _windowManager.ProjectBrowser.Show(dockPanel);
        }

        private void favouritesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_windowManager.Favourites.Visible)
                _windowManager.Favourites.Hide();
            else
                _windowManager.Favourites.Show(dockPanel);                        
        }

        private void historyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_windowManager.History.Visible)
                _windowManager.History.Hide();
            else 
                _windowManager.History.Show(dockPanel);
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OptionsBrowser form = new OptionsBrowser(_windowManager);
            form.ShowDialog(this);
        }
        
        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _windowManager.ProjectBrowser.NewProject();
        }

        private void menuItemOpen_Click(object sender, EventArgs e)
        {
            _windowManager.OpenFilesDialog();
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _windowManager.OpenProjectDialog();
        }

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_windowManager.ProjectBrowser.ProjectFilename))
            {
                _windowManager.SaveProjectDialog();
            }
            else
            {
                _windowManager.ProjectBrowser.SaveProject(null);
            }
        }

        private void saveProjectAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _windowManager.SaveProjectDialog();
        }
        
        private void menuItemExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void exitWithoutSavingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _noSave = true;
        }

        private void goBackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _windowManager.History.GoBack();
        }

        private void goForwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _windowManager.History.GoForward();
        }

        private FormWindowState _prevFormState;
        private FormBorderStyle _prevFormBorder;
        private bool _prevMenuVisible;
        private bool _prevToolbarVisible;

        private void fullScreenF11ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SuspendLayout();

            try
            {
                fullScreenF11ToolStripMenuItem.Checked = !fullScreenF11ToolStripMenuItem.Checked;

                if (fullScreenF11ToolStripMenuItem.Checked)
                {
                    _prevFormState = this.WindowState;
                    _prevFormBorder = this.FormBorderStyle;
                    _prevMenuVisible = mainMenu.Visible;
                    _prevToolbarVisible = toolBar.Visible;

                    if (this.WindowState == FormWindowState.Maximized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }

                    this.FormBorderStyle = FormBorderStyle.None;
                    this.WindowState = FormWindowState.Maximized;
                    mainMenu.Visible = false;
                }
                else
                {
                    mainMenu.Visible = _prevMenuVisible;
                    this.WindowState = _prevFormState;
                    this.FormBorderStyle = _prevFormBorder;
                }
            }
            finally
            {
                this.ResumeLayout();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm form = new AboutForm();
            form.ShowDialog(this);
        }

        private void lockLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lockLayoutToolStripMenuItem.Checked)
            {
                lockLayoutToolStripMenuItem.Checked = false;
                dockPanel.AllowEndUserDocking = true;
            }
            else
            {
                lockLayoutToolStripMenuItem.Checked = true;
                dockPanel.AllowEndUserDocking = false;
            }
        }


        private void StashMainWindowSettings()
        {
            try
            {
                // Update application options.
                if (this.WindowState == FormWindowState.Maximized)
                {
                    _windowManager.AppOptions.Left = Math.Max(0, this.RestoreBounds.Left);
                    _windowManager.AppOptions.Top = Math.Max(0, this.RestoreBounds.Top);
                    _windowManager.AppOptions.Width = Math.Max(20, this.RestoreBounds.Width);
                    _windowManager.AppOptions.Height = Math.Max(20, this.RestoreBounds.Height);
                    _windowManager.AppOptions.Maximized = true;
                }
                else
                {
                    _windowManager.AppOptions.Left = Math.Max(0, this.Location.X);
                    _windowManager.AppOptions.Top = Math.Max(0, this.Location.Y);
                    _windowManager.AppOptions.Width = Math.Max(20, this.Size.Width);
                    _windowManager.AppOptions.Height = Math.Max(20, this.Size.Height);
                    _windowManager.AppOptions.Maximized = false;
                }

                _windowManager.AppOptions.LayoutLocked = lockLayoutToolStripMenuItem.Checked;
            }
            catch (IOException exc)
            {
                _windowManager.Logger.LogExcStr(exc, "Failed saving application profile");
            }
            catch (Exception exc)
            {
                if (_windowManager.Logger.LogCatchAll(exc, "Failed saving application profile")) throw;
            }
        }

        private void ApplyMainWindowSettings()
        {
            int maxLeft = 100; int minLeft = 0; 
            int maxTop = 100; int minTop = 0; 
            int maxWidth = 100; int minWidth = 20;
            int maxHeight = 100; int minHeight = 20;
            try
            {
                if (Screen.AllScreens.Length == 1)
                {
                    maxLeft = Screen.PrimaryScreen.Bounds.Width - 100;
                    maxTop = Screen.PrimaryScreen.Bounds.Height - 100;
                    maxWidth = Screen.PrimaryScreen.Bounds.Width;
                    maxHeight = Screen.PrimaryScreen.Bounds.Height;
                }
                else
                {
                    // Get the second monitor screen
                    maxLeft = Screen.PrimaryScreen.Bounds.Width + Screen.AllScreens[1].Bounds.Width - 100;
                    maxTop = Screen.PrimaryScreen.Bounds.Height - 100;
                    maxWidth = Screen.PrimaryScreen.Bounds.Width + Screen.AllScreens[1].Bounds.Width;
                    maxHeight = Screen.PrimaryScreen.Bounds.Height;
                }
            }
            catch (Exception exc)
            {
                if (_windowManager.Logger.LogCatchAll(exc, "Failed getting screen bounds")) throw;
            }

            try
            {
                this.Left = Math.Min(maxLeft, Math.Max(minLeft, _windowManager.AppOptions.Left));
                this.Top = Math.Min(maxTop, Math.Max(minTop, _windowManager.AppOptions.Top));
                this.Width = Math.Min(maxWidth, Math.Max(minWidth, _windowManager.AppOptions.Width));
                this.Height = Math.Min(maxHeight, Math.Max(minHeight, _windowManager.AppOptions.Height));
                if (_windowManager.AppOptions.Maximized)
                    this.WindowState = FormWindowState.Maximized;
                else this.WindowState = FormWindowState.Normal;

                _initialProjectFilename = _windowManager.AppOptions.ProjectFilename;

                lockLayoutToolStripMenuItem.Checked = _windowManager.AppOptions.LayoutLocked;
                
                if (lockLayoutToolStripMenuItem.Checked)
                {
                    dockPanel.AllowEndUserDocking = false; 
                }
            }
            catch (IOException exc)
            {
                _windowManager.Logger.LogExcStr(exc, "Failed loading application config");
            }
            catch (Exception exc)
            {
                if (_windowManager.Logger.LogCatchAll(exc, "Failed loading application config")) throw;
            }
        }

        private void LoadDockingConfig()
        {
            try
            {
                _deserializeDockContent = new DeserializeDockContent(
                    _windowManager.GetContentFromPersistString);

                // Load docking config.
                string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Refractor.dock.xml");
                if (!File.Exists(configFile))
                {
                    // Write a default file from resources. Dock library needs unicode.
                    string fileText = Common.ResourceHelper.GetFileText("dockpanel");
                    File.WriteAllText(configFile, fileText, Encoding.Unicode);
                }
                dockPanel.LoadFromXml(configFile, _deserializeDockContent);
            }
            catch (IOException exc)
            {
                _windowManager.Logger.LogExcStr(exc, "Failed loading docking config");
            }
            catch (Exception exc)
            {
                if (_windowManager.Logger.LogCatchAll(exc, "Failed loading docking config")) throw;
            }
        }

        private void SaveDockingConfig()
        {
            try
            {
                // Save docking config.
                string path = Path.GetDirectoryName(Application.ExecutablePath);
                string configFile = Path.Combine(path, "Refractor.dock.xml");
                dockPanel.SaveAsXml(configFile);
            }
            catch (IOException exc)
            {
                _windowManager.Logger.LogExcStr(exc, "Failed saving docking config");
            }
            catch (Exception exc)
            {
                if (_windowManager.Logger.LogCatchAll(exc, "Failed saving docking config")) throw;
            }
        }


	}

}
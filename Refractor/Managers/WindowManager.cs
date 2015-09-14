using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using Refractor.Common;
using WeifenLuo.WinFormsUI.Docking;

namespace Refractor
{
    public class WindowManager : IServiceProvider, IWindowManager 
    {
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(WindowManager))
                return this;

            if (serviceType == typeof(IWindowManager))
                return this as IWindowManager;

            if (serviceType == typeof(IStatusManager))
                return _statusManager as IStatusManager;

            if (serviceType == typeof(ILogView))
                return _logView as ILogView;

            if (serviceType == typeof(IProjectBrowser))
                return _projectBrowser as IProjectBrowser;

            if (serviceType == typeof(IMenuManager))
                return _menuManager as IMenuManager;

            _logView.LogStr("ERROR : GetService failed on type : " + serviceType.Name);

            return null;
        }

        public void SetActiveItem(BaseItem item, object sender)
        {
            // todo? get rid of this, passthrough to project manager, and then back.
            // better to have pm control it all anyhow?
            _projectBrowser.SetActiveItem(item, sender);
        }

        public PluginOptions GetPluginOptions(string pluginID)
        {
            return _pluginManager.GetPluginOptions(pluginID);
        }

        
        private LogView _logView;
        internal LogView Logger { get { return _logView; } }
        
        private DockPanel _dockPanel;
        internal DockPanel DockPanel { get { return _dockPanel; } }

        private ProjectBrowser _projectBrowser;
        internal ProjectBrowser ProjectBrowser { get { return _projectBrowser; } }

        private StatusManager _statusManager;
        internal IStatusManager StatusManager { get { return _statusManager; } }

        private MenuManager _menuManager;
        internal MenuManager MenuManager { get { return _menuManager; } }

        private Dictionary<string, DockContent> _windows = new Dictionary<string, DockContent>();
        internal Dictionary<string, DockContent> Windows { get { return _windows; } }

        private FavouritesBrowser _favouritesBrowser;
        internal FavouritesBrowser Favourites { get { return _favouritesBrowser; } }

        private HistoryBrowser _historyBrowser;
        internal HistoryBrowser History { get { return _historyBrowser; } }

        private PluginManager _pluginManager;
        internal PluginManager PluginManager { get { return _pluginManager; } }

        internal ApplicationOptions AppOptions;


        internal WindowManager(MainForm form, DockPanel dockPanel, StatusStrip statusStrip, MenuStrip menuStrip)
        {
            _mainForm = form;

            _dockPanel = dockPanel;

            _statusManager = new StatusManager(this, statusStrip);
            _menuManager = new MenuManager(this, menuStrip);

            _logView = new LogView(this);
            _projectBrowser = new ProjectBrowser(this);
            _projectBrowser.OnFilesChanged += new EventHandler(_projectBrowser_OnFilesChanged);
            _favouritesBrowser = new FavouritesBrowser(this);
            _historyBrowser = new HistoryBrowser(this);
        }

        internal void Init()
        {
            _logView.Clear();
            _logView.LogStr("Refractor starts at " + DateTime.Now.ToString());

            // Update our resource helper.
            Common.ResourceHelper.CheckAssembly(Assembly.GetExecutingAssembly());

            // Load plugins.
            _pluginManager = new PluginManager(this);
            _pluginManager.LoadPlugins();

            // Set File Dialogs.
            _ofdFiles = new OpenFileDialog();
            _ofdFiles.InitialDirectory = Application.ExecutablePath;

            _ofdFiles.Filter = GetFilterString();

            _ofdFiles.FilterIndex = 1;
            _ofdFiles.RestoreDirectory = false;
            _ofdFiles.Multiselect = true;

            _ofdProject = new OpenFileDialog();
            _ofdProject.InitialDirectory = Application.ExecutablePath;
            _ofdProject.Filter = "Project files (*.rfp)|*.rfp|All files (*.*)|*.*";
            _ofdProject.FilterIndex = 1;
            _ofdProject.RestoreDirectory = false;

            _sfd = new SaveFileDialog();
            _sfd.InitialDirectory = Application.ExecutablePath;
            _sfd.DefaultExt = ".rpf";
            _sfd.RestoreDirectory = true;

            // Load main app options.
            LoadApplicationOptions();
        }

        internal void Finish()
        {
            SaveApplicationOptions();

            _logView.LogStr("Refractor ends at " + DateTime.Now.ToString());
        }

        internal IDockContent GetContentFromPersistString(string persistString)
        {
            if (persistString == _projectBrowser.GetType().ToString())
                return _projectBrowser;

            if (persistString == _logView.GetType().ToString())
                return _logView;

            if (persistString == _favouritesBrowser.GetType().ToString())
                return _favouritesBrowser;

            if (persistString == _historyBrowser.GetType().ToString())
                return _historyBrowser;

            foreach (KeyValuePair<string, DockContent> pair in _windows)
            {
                if (persistString == pair.Value.GetType().ToString())
                    return pair.Value;
            }

            return null;
        }

        internal void OpenFilesDialog()
        {
            if (_ofdFiles.ShowDialog() == DialogResult.OK)
            {
                _projectBrowser.Show(this.DockPanel);
                _projectBrowser.AddFiles(_ofdFiles.FileNames);
            }
        }

        internal void OpenProjectDialog()
        {
            if (_ofdProject.ShowDialog() == DialogResult.OK)
            {
                _projectBrowser.LoadProject(_ofdProject.FileName, false);
            }
        }

        internal void SaveProjectDialog()
        {
            if (_sfd.ShowDialog() == DialogResult.OK)
            {
                _projectBrowser.SaveProject(_sfd.FileName);
            }
        }


        internal void AddPlugin(string id, DockContent control, bool isDefault)
        {
            if (_windows.ContainsKey(id))
            {
                _logView.LogStr("ERROR : Failed to get unique window id");
                return;
            }

            _windows.Add(id, control);

            // Attach the on visible handler, which may be responsible for
            // calling the update.
            control.VisibleChanged += new EventHandler(Window_VisibleChanged);
        }

        internal void UpdateWindow(IWindowPlugin plugin)
        {
            string id = plugin.GetID();

            if (!_windows.ContainsKey(id))
            {
                _logView.LogStr("ERROR : UpdatePlugin failed to find plugin : " + id);
                return;
            }

            _windows[id] = (DockContent)plugin;
        }

        internal void RemovePlugin(string id)
        {
            if (!_windows.ContainsKey(id)) return;

            DockContent window = _windows[id];

            _windows.Remove(id);
        }

        internal void UpdatePluginItem(IWindowPlugin plugin, BaseItem item)
        {
            CachedSet(plugin as DockContent, item, false);            
        }

        internal void BroadcastItemChange(object sender, BaseItem item)
        {
            foreach (KeyValuePair<string, DockContent> pair in _windows)
            {
                if (pair.Value is IActiveItemViewer)
                {
                    if (pair.Value is IWindowPlugin)
                    {
                        List<Type> types = (pair.Value as IWindowPlugin).GetHandledTypes();

                        if (item == null)
                        {
                            CachedSet(pair.Value, item, true);
                        }
                        else if (types != null && types.Contains(item.GetType()))
                        {
                            BaseItem currentItem = (pair.Value as IActiveItemViewer).GetActiveItem();

                            if (item != currentItem)
                            {
                                CachedSet(pair.Value, item, false);
                            }
                        }                        
                    }
                }
            }
        }

        internal void BroadcastRefresh(object sender, BaseItem item)
        {
            foreach (KeyValuePair<string, DockContent> pair in _windows)
            {
                if (pair.Value is IActiveItemViewer)
                {
                    if (pair.Value is IWindowPlugin)
                    {
                        CachedSetRefresh(pair.Value, item);
                    }
                }
            }
        }

        internal void SetWindowItems(List<string> windowNames, List<string> itemIds)
        {
            // Set current items for all the windows.
            foreach (KeyValuePair<string, DockContent> pair in _windows)
            {
                // These are dealt with by the project browser files changed event.
                if ((pair.Value as IWindowPlugin).GetKind() == WindowPluginKind.MainWindow)
                    continue;

                int idx = windowNames.IndexOf(pair.Key);

                if (idx > -1)
                {
                    BaseItem item = _projectBrowser.Lookup(itemIds[idx]);

                    CachedSet(pair.Value, item, false);
                }
            }
        }

        internal void GetWindowItems(List<string> windowNames, List<string> itemIds)
        {
            // Get the current items for all the windows.
            windowNames.Clear();
            itemIds.Clear();


            foreach (KeyValuePair<string, DockContent> pair in _windows)
            {
                try
                {
                    if (pair.Value is IActiveItemViewer)
                    {
                        BaseItem item = (pair.Value as IActiveItemViewer).GetActiveItem();

                        if (item != null)
                        {
                            windowNames.Add(pair.Key);
                            itemIds.Add(item.GetID());
                        }
                    }
                }
                catch (Exception exc)
                {
                    if (_logView.LogCatchAll(exc, "Unexpected plugin error")) throw;
                }
            }
        }

        internal void SetCaption(string caption)
        {
            if (string.IsNullOrEmpty(caption))
                _mainForm.Text = "Refractor";
            else
                _mainForm.Text = "Refractor [" + caption + "]";
        }


        private MainForm _mainForm;
        private OpenFileDialog _ofdFiles;
        private OpenFileDialog _ofdProject;
        private SaveFileDialog _sfd;
        private Dictionary<DockContent, BaseItem> _activeItems = new Dictionary<DockContent, BaseItem>();
        private Dictionary<DockContent, BaseItem> _activeItemsRefresh = new Dictionary<DockContent, BaseItem>();
        private Dictionary<Type, DockContent> _defaults = new Dictionary<Type, DockContent>();
        private object _lock = new object();

        private void Window_VisibleChanged(object sender, EventArgs e)
        {
            if (!(sender is IActiveItemViewer)) return;
            if (!(sender is DockContent)) return;

            DockContent window = (sender as DockContent);
            if (!window.Visible) return;

            if (!_activeItems.ContainsKey(sender as DockContent)) return;                            

            SetItemInternal(window, _activeItems[window]);
        }

        private void CachedSet(DockContent window, BaseItem item, bool force)
        {
            // Add or get from local storage.
            if (!_activeItems.ContainsKey(window))
            {
                _activeItems.Add(window, item);
            }
            else
            {
                _activeItems[window] = item;
            }

            // Set immediately if visible or force, otherwise set in VisibleChanged.
            if (force || window.Visible)
            {
                SetItemInternal(window, item);
            }
        }

        private void CachedSetRefresh(DockContent window, BaseItem item)
        {
            // Add or get from local storage.
            if (!_activeItemsRefresh.ContainsKey(window))
            {
                _activeItemsRefresh.Add(window, item);
            }
            else
            {
                _activeItemsRefresh[window] = item;
            }

            // Set immediately if visible, otherwise set in VisibleChanged.
            if (window.Visible)
            {
                SetItemRefreshInternal(window, item);
            }
        }

        private void SetItemInternal(DockContent window, BaseItem item)
        {
            try
            {
                IActiveItemViewer viewer = window as IActiveItemViewer;
                IWindowPlugin plugin = window as IWindowPlugin;

                // Always set a main window plugin, but only set others if they've changed.
                if (plugin.GetKind() == WindowPluginKind.MainWindow ||
                    viewer.GetActiveItem() != item)
                {
                    viewer.SetActiveItem(item);
                }
            }
            catch (Exception exc)
            {
                if (_logView.LogCatchAll(exc, "Unexpected plugin error")) throw;
            }                        
        }

        private void SetItemRefreshInternal(DockContent window, BaseItem item)
        {
            try
            {
                IActiveItemViewer viewer = window as IActiveItemViewer;
                IWindowPlugin plugin = window as IWindowPlugin;

                viewer.SetRefresh(item);
            }
            catch (Exception exc)
            {
                if (_logView.LogCatchAll(exc, "Unexpected plugin error")) throw;
            }
        }

        
        private void _projectBrowser_OnFilesChanged(object sender, EventArgs e)
        {
            BaseItem item = sender as RootItem;
            if (item == null) return;

            // Set current items for all the windows, MainWindow plugins only.
            foreach (KeyValuePair<string, DockContent> pair in _windows)
            {
                if ((pair.Value as IWindowPlugin).GetKind() != WindowPluginKind.MainWindow)
                    continue;

                CachedSet(pair.Value, item, false);
            }
        }
        
        private string GetFilterString()
        {
            StringBuilder sb = new StringBuilder("All files (*.*)|*.*|");
            List<string> allHandled = new List<string>();
            foreach (IParserPlugin plugin in _pluginManager.ParserPlugins)
            {
                try
                {
                    List<string> exts = plugin.HandlesExtensions();
                    
                    sb.Append(plugin.GetID());
                    
                    sb.Append(" (");
                    for (int i = 0; i < exts.Count; i++)
                    {
                        sb.Append("*");
                        sb.Append(exts[i]);
                        if (i < exts.Count - 1) sb.Append(";");
                    }
                    sb.Append(")|");
                    for (int i = 0; i < exts.Count; i++)
                    {
                        sb.Append("*");
                        sb.Append(exts[i]);
                        if (i < exts.Count - 1) sb.Append(";");
                    }
                    sb.Append("|");

                    for (int i = 0; i < exts.Count; i++)
                    {
                        if (!allHandled.Contains(exts[i])) 
                            allHandled.Add(exts[i]);
                    }
                }
                catch (Exception exc)
                {
                    if (_logView.LogCatchAll(exc, "Unexpected plugin error")) throw;
                }
            }

            sb.Append("All handled types (");
            for (int i = 0; i < allHandled.Count; i++)
            {
                sb.Append("*");
                sb.Append(allHandled[i]);
                if (i < allHandled.Count - 1) sb.Append(";");
            }
            sb.Append(")|");
            for (int i = 0; i < allHandled.Count; i++)
            {
                sb.Append("*");
                sb.Append(allHandled[i]);
                if (i < allHandled.Count - 1) sb.Append(";");
            }

            string result = sb.ToString();
            return result;
        }


        private void LoadApplicationOptions()
        {
            string appConfigFilename = Application.ExecutablePath + ".xml";
            if (File.Exists(appConfigFilename))
            {
                try
                {
                    string s = File.ReadAllText(Application.ExecutablePath + ".xml");
                    Hwd.Serialization.Decoder decoder = new Hwd.Serialization.Decoder();
                    AppOptions = (ApplicationOptions)decoder.Decode(s);
                }
                catch (Hwd.Serialization.SerializationException exc)
                {
                    _logView.LogExcStr(exc, "Failed deserializing application options");
                }
            }
            else
            {
                AppOptions = new ApplicationOptions();
            }

            if (AppOptions == null)
            {
                AppOptions = new ApplicationOptions();
            }

            // Apply UI settings to project browser.
            ProjectBrowser.TreeView.Columns[0].Width = AppOptions.ProjectCol0Width;
            ProjectBrowser.TreeView.Columns[1].Width = AppOptions.ProjectCol1Width;
            ProjectBrowser.TreeView.Columns[2].Width = AppOptions.ProjectCol2Width;
            ProjectBrowser.TreeView.Columns[3].Width = AppOptions.ProjectCol3Width;

            // Set the plugin options.
            PluginManager.SetAllOptions(
                AppOptions.PluginOptions,
                AppOptions.PluginParserOptions);
        }

        private void SaveApplicationOptions()
        {
            // Update the options from the UI.
            AppOptions.ProjectFilename = ProjectBrowser.ProjectFilename;
            AppOptions.ProjectCol0Width = ProjectBrowser.TreeView.Columns[0].Width;
            AppOptions.ProjectCol1Width = ProjectBrowser.TreeView.Columns[1].Width;
            AppOptions.ProjectCol2Width = ProjectBrowser.TreeView.Columns[2].Width;
            AppOptions.ProjectCol3Width = ProjectBrowser.TreeView.Columns[3].Width;

            // Update the options from the plugins.
            PluginManager.GetAllOptions(
                AppOptions.PluginOptions,
                AppOptions.PluginParserOptions);

            
            string appConfigFilename = Application.ExecutablePath + ".xml";
            try
            {
                Hwd.Serialization.Encoder encoder = new Hwd.Serialization.Encoder();
                string s = encoder.Encode(AppOptions);
                File.WriteAllText(appConfigFilename, s);
            }
            catch (Hwd.Serialization.SerializationException exc)
            {
                _logView.LogExcStr(exc, "Failed serializing application options");
            }
        }


    }
}

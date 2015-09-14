using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Xml;

using Hwd.Serialization;
using Refractor.Common;
using WeifenLuo.WinFormsUI.Docking;
using Aga.Controls.Tree.NodeControls;
using Aga.Controls.Tree;

namespace Refractor
{
    /// <summary>
    /// Based on the aga tree demo.
    /// Project Browser implements the hierarchy:
    /// File/Assembly name
    ///   Namespace
    ///     Type
    ///       Method
    /// </summary>
    internal partial class ProjectBrowser : DockContent, IProjectBrowser
	{
        private RootItem _rootItem = new RootItem();
        public RootItem RootItem { get { return _rootItem; } }

        private string _projectFilename = null;
        public string ProjectFilename { get { return _projectFilename; } }

        public Dictionary<string, BaseItem> Files { get { return _model == null ? null : _model.Files; } }

        public event EventHandler OnFilesChanged;

        public ProjectBrowser(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            _defaultFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            _windowManager = (WindowManager)serviceProvider.GetService(typeof(WindowManager));
            _statusManager = (IStatusManager)_windowManager.GetService(typeof(IStatusManager));

            _lookup = new LookupHelper(_windowManager);

            // The below shadow copy technique does *not* stop the assemblies being locked.
            //AppDomain.CurrentDomain.SetShadowCopyFiles();
            //AppDomain.CurrentDomain.SetShadowCopyPath("c:\\temp");
            // SetupInformation is used only at start of domain.
            // Loading them into another domain still locks them.
            // Only thing that seems to prevent locking is bytecopy load.
            // Bytecopy load has problems finding referenced assemblies.
            // This can be got around with CurrentDomain_AssemblyResolve?
        }

        public void NewProject()
        {
            try
            {
                string path = _defaultFolder;
                int i = 1;
                do
                {
                    _projectFilename = Path.Combine(path, "NewProject" + i++.ToString() + ".rfp");
                }
                while (File.Exists(_projectFilename));

                int previousCount = _rootItem.Files.Count;
                _project.Filenames.Clear();
                _model.Clear();
                _lookup.Clear();
                _watcher.Clear();
                _rootItem.Files.Clear();

                RefreshModel();
                SetCaption();

                _windowManager.Favourites.Clear();
                _windowManager.SetCaption(_projectFilename);
                _windowManager.BroadcastItemChange(this, null);

                if (previousCount != _rootItem.Files.Count)
                {
                    if (OnFilesChanged != null)
                        OnFilesChanged(_rootItem, new EventArgs());
                }

                _watcher.Start();
            }
            catch (IOException exc)
            {
                _windowManager.Logger.LogExcStr(exc, "New project failed");
            }
        }

        public void LoadProject(string filename, bool noPlugins)
        {
            try
            {
                _projectFilename = filename;

                _project.Filenames.Clear();
                _model.Clear();
                _lookup.Clear();
                _watcher.Clear();
                _rootItem.Files.Clear();

                if (!File.Exists(filename))
                {
                    _windowManager.Logger.LogStr("Project file was not found:" + filename);
                    return;
                }

                // Load the project file.
                string s = File.ReadAllText(_projectFilename);
                Hwd.Serialization.Decoder decoder = new Hwd.Serialization.Decoder();
                _project = (ProjectDefinition)decoder.Decode(s);

                if (_project == null)
                {
                    _windowManager.Logger.LogStr("Failed to deserialize project file:" + _projectFilename);
                    _project = new ProjectDefinition();
                    return;
                }

                // Check the filename list.
                List<string> remove = new List<string>();
                foreach (string name in _project.Filenames)
                {
                    if (!File.Exists(name))
                    {
                        remove.Add(name);
                    }
                    else
                    {
                        MatchFileToModel(name);
                    }
                }
                foreach (string name in remove)
                {
                    _project.Filenames.Remove(name);
                    _windowManager.Logger.LogStr("Warning : Project contains a file that was not found:" + name);
                }
                
                // Load item structure in advance, so we can "goto" items.
                foreach (KeyValuePair<string, BaseItem> pair in _model.Files)
                {
                    ReadItem(pair.Value, false);
                    _watcher.AddWatch(pair.Value);
                }

                // This must be done before we set current item in the tree.
                RefreshModel();
                SetCaption();

                // Load favourites, global ignores, and plugin options as part of the project.
                _windowManager.Favourites.AddAll(_project.Favourites);
                _windowManager.SetCaption(_projectFilename);

                // Load settings for windows.
                if (!noPlugins)
                    _windowManager.SetWindowItems(_project.Windows, _project.Items);

                // Load the project item. We avoid selecting it here because each
                // window will load it's own current item anyway, above.
                BaseItem item = Lookup(_project.ProjectItem);
                try
                {
                    _ignoreSelect = true;
                    SetActiveItem(item, this);
                }
                finally
                {
                    _ignoreSelect = false;
                }

                // Notify files changed.
                if (!noPlugins)
                    if (OnFilesChanged != null)
                        OnFilesChanged(_rootItem, new EventArgs());

                _watcher.Start();
            }
            catch (SerializationException exc)
            {
                _windowManager.Logger.LogExcStr(exc, "Deserializing project failed");
            }
            catch (IOException exc)
            {
                _windowManager.Logger.LogExcStr(exc, "Load project failed");
            }
        }

        public void SaveProject(string filename)
        {
            try
            {
                if (filename != null) _projectFilename = filename;

                if (_projectFilename == null) return;

                // Save current item settings for windows.
                _windowManager.GetWindowItems(_project.Windows, _project.Items);
                if (_currentItem != null)
                    _project.ProjectItem = _currentItem.GetID();
                else
                    _project.ProjectItem = string.Empty;

                // Save the favourites and ignores along with the project.
                _windowManager.Favourites.GetAll(_project.Favourites);
                _windowManager.SetCaption(_projectFilename);

                Hwd.Serialization.Encoder encoder = new Hwd.Serialization.Encoder();
                string s = encoder.Encode(_project);
                File.WriteAllText(_projectFilename, s);
                
                SetCaption();
            }
            catch (SerializationException exc)
            {
                _windowManager.Logger.LogExcStr(exc, "Deserializing project failed");
            }
            catch (IOException exc)
            {
                _windowManager.Logger.LogExcStr(exc, "Save project failed");
            }
        }

        public void AddFiles(string[] filenames)
        {
            try
            {
                foreach (string filename in filenames)
                {
                    if (_project.Filenames.Contains(filename)) continue;

                    BaseItem item = MatchFileToModel(filename);

                    if (item == null) continue;

                    _project.Filenames.Add(filename);

                    ReadItem(item, false);
                    _watcher.AddWatch(item);
                }

                RefreshModel();

                if (OnFilesChanged != null)
                    OnFilesChanged(_rootItem, new EventArgs());
            }
            catch (IOException exc)
            {
                _windowManager.Logger.LogExcStr(exc, "Add files to project failed");
            }
        }

        public BaseItem GetActiveItem()
        {
            if (_treeView.SelectedNode == null) return null;
            if (_treeView.SelectedNode.Tag == null) return null;

            if (_treeView.SelectedNode.Tag is BaseItem)
                return _treeView.SelectedNode.Tag as BaseItem;

            return null;
        }

        public void SetActiveItem(BaseItem item, object sender)
        {
            if (item == null) return;
            if (_currentItem == item) return;

            _currentItem = item;

            // Find the treenode for this item, if available, and select it.
            // todo, goes via tags. issue with this not working? in jsfile?
            _treeView.SelectedNode = _treeView.FindNode(_model.GetPath(item)); 
                
            if (_treeView.SelectedNode == null)
            {
                _windowManager.Logger.LogStr("Item no longer present in project");
            }
            else
            {
                _treeView.ScrollTo(_treeView.SelectedNode);
            }
        }

        public BaseItem Lookup(string id)
        {
            return _lookup.Find(id);
        }

        public void AddLookup(BaseItem item, string id, bool invert)
        {
            if (invert)
                _lookup.Remove(id);
            else
                _lookup.Add(id, item);
        }

        internal TreeViewAdv TreeView { get { return _treeView; } }

        private ProjectBrowserModel _model;
        private ProjectDefinition _project;
        private ProjectWatcher _watcher;
        private WindowManager _windowManager;
        private IStatusManager _statusManager;
        private LookupHelper _lookup;

        private BaseItem _currentItem;
        private bool _timedSelect = true;
        private Timer _timerSelect;
        private bool _ignoreSelect;
        private Timer _timerRefresh;
        private string _defaultFolder;
        private string _prevFilename = string.Empty;
        private List<string> _unknownExts = new List<string>();


        private void ProjectBrowser_Load(object sender, EventArgs e)
        {
            _model = new ProjectBrowserModel(_windowManager);
            _treeView.Model = new SortedTreeModel(_model);

            // Need this to select nodes in the load on demand tree model we're using here.
            _treeView.LoadOnDemand = false;

            _project = new ProjectDefinition();
            _watcher = new ProjectWatcher(_windowManager, this, _watcher_WatchChanged);

            _timerSelect = new Timer();
            _timerSelect.Interval = 200;
            _timerSelect.Tick += new EventHandler(_timerSelect_Tick);

            _timerRefresh = new Timer();
            _timerRefresh.Interval = 1000;
            _timerRefresh.Tick += new EventHandler(_timerRefresh_Tick);
            _timerRefresh.Start();
        }

        private BaseItem MatchFileToModel(string filename)
        {
            BaseItem item = null;
            string ext = Path.GetExtension(filename);

            if (_windowManager.PluginManager.ParserPluginsByExt.ContainsKey(ext))
            {
                try
                {
                    List<IParserPlugin> list = _windowManager.PluginManager.ParserPluginsByExt[ext];

                    foreach (IParserPlugin plugin in list)
                    {
                        item = plugin.GetFileItem(filename);
                        
                        if (item != null)
                        {
                            // Add the file item to the tree model.
                            _model.Files.Add(filename, item);
                            _rootItem.Files.Add((FileItem)item);
                            break;
                        }

                        // Try another if not handled.
                    }
                }
                catch (Exception exc)
                {
                    if (_windowManager.Logger.LogCatchAll(exc, "Unexpected plugin error")) throw;
                }
            }
            else
            {
                if (!_unknownExts.Contains(ext))
                {
                    _unknownExts.Add(ext);
                    _windowManager.Logger.LogStr(string.Format("No parser found for file extension : {0}", ext));
                }
            }

            return item;
        }

        private void ReadItem(BaseItem item, bool unload)
        {
            string ext = Path.GetExtension(item.Name);

            if (_windowManager.PluginManager.ParserPluginsByExt.ContainsKey(ext))
            {
                try
                {
                    List<IParserPlugin> list = _windowManager.PluginManager.ParserPluginsByExt[ext];

                    foreach (IParserPlugin plugin in list)
                    {
                        bool ok = plugin.ReadItem(item, unload);

                        // Try another until we get a result.
                        if (!ok) break;
                    }
                }
                catch (Exception exc)
                {
                    if (_windowManager.Logger.LogCatchAll(exc, "Unexpected plugin error")) throw;
                }
            }
            else _windowManager.Logger.LogStr(string.Format("Error : accepted file, but no parser found for {0}", ext));
        }

        private void SetCaption()
        {
            // This is not used, it seems. DockPanel TabText is used for both the caption
            // and the tab text, annoyingly, and we don't want long, changing tab captions.
            this.Text = string.Format("Project Browser [{0}]",
                Path.GetFileNameWithoutExtension(_projectFilename));
        }

        private void RefreshModel()
        {
            _treeView.Model = null;
            _treeView.Model = new SortedTreeModel(_model);
        }

        private void ActivateItem(BaseItem item)
        {
            if (item == null)
            {
                _windowManager.Logger.LogStr("Warning : ActivateItem : item is null");
                return;
            }

            _currentItem = item;
            _windowManager.BroadcastItemChange(this, _currentItem);
            _statusManager.SetLeftCaption(_currentItem.GetID());

            _model.Prioritise(_treeView.SelectedNode.Tag as BaseItem);
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (_treeView.SelectedNode == null) return;

            BaseItem item = (BaseItem)_treeView.SelectedNode.Tag;

            _windowManager.MenuManager.UpdateContextMenu(contextMenuStrip, item);

            workerEnabledToolStripMenuItem.Checked = _windowManager.AppOptions.RunProjectWorker;
        }

        
        private void _treeView_SelectionChanged(object sender, EventArgs e)
        {
            if (_ignoreSelect) return;

            // Use the select timer to avoid kicking off lots of threads and immediately 
            // cancelling them when we run down the tree holding the arrow keys.
            if (_timedSelect)
            {
                _timerSelect.Stop();
                _timerSelect.Start();
            }
            else
            {
                _timerSelect_Tick(null, null);
            }
        }

        private void _timerSelect_Tick(object sender, EventArgs e)
        {
            _timerSelect.Stop();
            if (_treeView.SelectedNode == null) return;

            BaseItem selectedItem = _treeView.SelectedNode.Tag as BaseItem;
            if (selectedItem == null) return;

            ActivateItem(selectedItem);
            _windowManager.History.Add(selectedItem);
        }

        private void _timerRefresh_Tick(object sender, EventArgs e)
        {
            _treeView.Invalidate();
        }


        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _windowManager.OpenFilesDialog();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelected();
        }

        private void workerEnabledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (workerEnabledToolStripMenuItem.Checked)
            {
                workerEnabledToolStripMenuItem.Checked = false;
                _model.PauseWorker();
            }
            else
            {
                workerEnabledToolStripMenuItem.Checked = true;
                _model.RunWorker();
            }
        }

        private void RemoveSelected()
        {
            if (_treeView.SelectedNode == null) return;

            if (!(_treeView.SelectedNode.Tag is FileItem)) return;

            // Get the underlying BaseItem.
            BaseItem item = _treeView.SelectedNode.Tag as BaseItem;
            int idx = _project.Filenames.IndexOf(item.ItemPath);

            // Update our local and model storage.
            _project.Filenames.Remove(item.ItemPath);
            _model.Files.Remove(item.ItemPath);
            _rootItem.Files.Remove((FileItem)item);
            _watcher.RemoveWatch((FileItem)item);

            // Call the plugin readitem again to remove the items from the lookup.
            ReadItem(item, true);

            RefreshModel();

            // Find the item to select afterwards.
            idx = Math.Min(idx, _project.Filenames.Count - 1);
            if (idx >= 0)
            {
                item = _model.Files[_project.Filenames[idx]];
                SetActiveItem(item, this);
            }

            // Notify files changed.
            if (OnFilesChanged != null) 
                OnFilesChanged(_rootItem, new EventArgs());
        }


        private void _watcher_WatchChanged(string filename)
        {
            try
            {
                if (_model.Files.ContainsKey(filename))
                {
                    string selectedId = string.Empty;
                    if (_treeView.SelectedNode != null)
                    {
                        selectedId = (_treeView.SelectedNode.Tag as BaseItem).GetID();
                    }

                    BaseItem item = _model.Files[filename];

                    // Reload the file.
                    ReadItem(item, false);

                    RefreshModel();

                    // Reselect by id.
                    if (selectedId != string.Empty)
                    {
                        BaseItem selectedItem = Lookup(selectedId);
                        if (item != null)
                        {
                            SetActiveItem(selectedItem, this);
                        }
                    }
                    else
                    {
                        SetActiveItem(item, this);
                    }

                    _prevFilename = filename;
                }
            }
            catch (Exception exc)
            {
                if (_windowManager.Logger.LogCatchAll(exc, "Unexpected file watcher error")) throw;
            }
        }
                
        
        private void _treeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // Taken from the demo project.
            _treeView.DoDragDropSelectedNodes(DragDropEffects.Link);
        }

        private void _treeView_Click(object sender, EventArgs e)
        {

        }

        private void _treeView_ColumnReordered(object sender, TreeColumnEventArgs e)
        {
        }

        private void _treeView_ColumnClicked(object sender, TreeColumnEventArgs e)
        {
            if (e == null || e.Column == null) return;

            TreeColumn clicked = e.Column;

            if (clicked.SortOrder == SortOrder.Ascending)
                clicked.SortOrder = SortOrder.Descending;
            else
                clicked.SortOrder = SortOrder.Ascending;

            (_treeView.Model as SortedTreeModel).Comparer = new FolderItemSorter(clicked.Header, clicked.SortOrder);

        }

        private void _treeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                RemoveSelected();
            }
        }

        private void _treeView_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
            if (e.Data.GetDataPresent(DataFormats.Text) ||
               e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void _treeView_DragDrop(object sender, DragEventArgs e)
        {
            string[] filenames = new string[1];
            bool isProject = false;
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                filenames[0] = (string)e.Data.GetData(DataFormats.Text);

                isProject = (Path.GetExtension(filenames[0]).ToLower() == ".rfp");
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                filenames = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (filenames.Length == 1)
                    isProject = (Path.GetExtension(filenames[0]).ToLower() == ".rfp");
            }

            if (isProject)
            {
                LoadProject(filenames[0], false);
            }
            else
            {
                AddFiles(filenames);
            }
        }


        private void checkLookupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _windowManager.Logger.LogStr("Lookup count=" +_lookup.GetCount());
            _windowManager.Logger.LogStr("Lookup count short=" + _lookup.GetCountShort());
        }

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _treeView.ExpandAll();
        }

        private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _treeView.CollapseAll();
        }

    }
}

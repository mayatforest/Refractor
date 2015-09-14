using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using Refractor.Common;
using Aga.Controls.Tree;

namespace Refractor
{
    internal class ProjectBrowserModel : ITreeModel
	{
        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
        public event EventHandler<TreePathEventArgs> StructureChanged;

        public ProjectBrowserModel()
        {
            NodesChanged += new EventHandler<TreeModelEventArgs>(ProjectBrowserModel_NodesChanged);
            NodesInserted += new EventHandler<TreeModelEventArgs>(ProjectBrowserModel_NodesInserted);
            NodesRemoved += new EventHandler<TreeModelEventArgs>(ProjectBrowserModel_NodesRemoved);
            StructureChanged += new EventHandler<TreePathEventArgs>(ProjectBrowserModel_StructureChanged);
        }

        public IEnumerable GetChildren(TreePath treePath)
        {
            List<BaseItem> items = null;

            if (treePath.IsEmpty())
            {
                items = new List<BaseItem>();
                foreach (KeyValuePair<string, BaseItem> pair in _files)
                {
                    items.Add(pair.Value);
                }
            }
            else
            {
                BaseItem parent = treePath.LastNode as BaseItem;
                if (parent != null)
                {
                    Type type = parent.GetType();
                    if (_plugins.ContainsKey(type))
                    {
                        IParserPlugin plugin = _plugins[type];
                        items = new List<BaseItem>();
                        try
                        {
                            plugin.FindChildren(parent, items);
                        }
                        catch (Exception exc)
                        {
                            if (_windowManager.Logger.LogCatchAll(exc, "Unexpected plugin error")) throw;
                        }
                    }
                    else
                    {
                        _windowManager.Logger.LogStr("Warning : No plugin for type : " + type.Name);
                    }
                }
            }

            if (items.Count > 0)
            {
                // Sort the items based on their names.
                Comparison<BaseItem> comparison = new Comparison<BaseItem>(Comparer);
                items.Sort(comparison);

                lock (_workerLock)
                {
                    // Add to the worker queue.
                    _itemsToRead.AddRange(items);
                }

                if (!_worker.IsBusy && _windowManager.AppOptions.RunProjectWorker)
                {
                    _worker.RunWorkerAsync();
                }
            }

            return items;
        }

        public bool IsLeaf(TreePath treePath)
        {
            bool result = false;
            
            BaseItem item = treePath.LastNode as BaseItem;
            Type type = item.GetType();
            
            if (_plugins.ContainsKey(type))
            {
                IParserPlugin plugin = _plugins[type];
                try
                {
                    result = plugin.IsLeaf(item);
                }
                catch (Exception exc)
                {
                    if (_windowManager.Logger.LogCatchAll(exc, "Unexpected plugin error")) throw;
                }
            }
            else
            {
                _windowManager.Logger.LogStr("Warning : No plugin for type : " + type.Name);
            }

            return result;
        }


        private Dictionary<string, BaseItem> _files = new Dictionary<string, BaseItem>();
        internal Dictionary<string, BaseItem> Files { get { return _files; } set { _files = value; } }

        internal ProjectBrowserModel(WindowManager windowManager)
		{
            _windowManager = windowManager;
			_itemsToRead = new List<BaseItem>();
            
            foreach (IParserPlugin plugin in _windowManager.PluginManager.ParserPlugins)
            {
                try
                {
                    List<Type> types = plugin.HandlesItems();
                    foreach (Type type in types)
                    {
                        if (!_plugins.ContainsKey(type))
                        {
                            _plugins.Add(type, plugin);
                        }
                        else 
                        {
                            _windowManager.Logger.LogStr("Warning : multiple plugins for :" + type.Name);
                        }
                    }
                }
                catch (Exception exc)
                {
                    if (_windowManager.Logger.LogCatchAll(exc, "Unexpected plugin error")) throw;
                }
            }

            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += new DoWorkEventHandler(_worker_ReadItemsProperties);
            _worker.ProgressChanged += new ProgressChangedEventHandler(_worker_ProgressChanged);
		}

        internal void Clear()
        {
            _files.Clear();
            _itemsToRead.Clear();
        }

        internal TreePath GetPath(BaseItem item)
        {
            if (item == null)
            {
                return TreePath.Empty;
            }
            else
            {
                Stack<object> stack = new Stack<object>();
                while (item != null)
                {
                    stack.Push(item);
                    item = item.Parent;
                }
                return new TreePath(stack.ToArray());
            }
        }
        
        internal void Prioritise(BaseItem item)
        {
            lock (_workerLock)
            {
                if (_itemsToRead.Count == 0) return;

                if (item.Parent != null)
                {
                    item = item.Parent;
                }

                int idx = _itemsToRead.IndexOf(item);

                // If greater than zero - if zero, then already at head of queue.
                if (idx > 0)
                {
                    // Move the items infront of this items parent to the back.
                    List<BaseItem> list = new List<BaseItem>();
                    for (int i = 0; i < idx; i++)
                    {
                        list.Add(_itemsToRead[i]);
                    }
                    _itemsToRead.RemoveRange(0, idx - 1);
                    _itemsToRead.AddRange(list);
                }

                if (!_worker.IsBusy && _windowManager.AppOptions.RunProjectWorker)
                {
                    _worker.RunWorkerAsync();
                }
            }
        }

        internal void RunWorker()
        {
            _windowManager.AppOptions.RunProjectWorker = true;
            if (!_worker.IsBusy)
            {
                _worker.RunWorkerAsync();
            }
        }

        internal void PauseWorker()
        {
            _windowManager.AppOptions.RunProjectWorker = false;
        }

        private BackgroundWorker _worker;
        private List<BaseItem> _itemsToRead;
        private WindowManager _windowManager;
        private Dictionary<Type, IParserPlugin> _plugins = new Dictionary<Type, IParserPlugin>();
        private FileItem _workerItem = new FileItem("Browser", null);
        private int _progress = 0;
        private object _workerLock = new object();

        private void _worker_ReadItemsProperties(object sender, DoWorkEventArgs e)
        {
            try
            {
                BaseItem item = null;
                int count = 0;
                while (true)
                {
                    if (!_windowManager.AppOptions.RunProjectWorker) break;

                    item = null;
                    lock (_workerLock)
                    {
                        count = _itemsToRead.Count;
                        if (count > 0)
                        {
                            item = _itemsToRead[0];
                            _itemsToRead.RemoveAt(0);
                        }
                    }
                    if (count == 0) break;
                    if (item == null) break;

                    Thread.Sleep(50);

                    if (_plugins.ContainsKey(item.GetType()))
                    {
                        try
                        {
                            _plugins[item.GetType()].CalcMetrics(item);
                        }
                        catch (Exception exc)
                        {
                            if (_windowManager.Logger.LogCatchAll(exc, "Unexpected plugin error")) throw;
                        }
                    }

                    _worker.ReportProgress(_progress++, item);
                }
            }
            catch (Exception exc)
            {
                if (_windowManager.Logger.LogCatchAll(exc, "Unexpected worker failure")) throw;
            }
        }

        private void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
            //
            // NodesChanged chronically slows down opening of large projects.
            // 
        }

        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //_windowManager.WorkersBrowser.WorkerFinished((BackgroundWorker)sender);
            }
            catch (Exception exc)
            {
                if (_windowManager.Logger.LogCatchAll(exc, "Unexpected worker failure")) throw;
            }
        }

        private int Comparer(BaseItem item1, BaseItem item2)
        {
            return StringLogicalComparer.Compare(item1.Name, item2.Name);
        }


        private void ProjectBrowserModel_NodesChanged<TreeModelEventArgs>(object sender, TreeModelEventArgs e)
        {
        }
        private void ProjectBrowserModel_NodesInserted<TreeModelEventArgs>(object sender, TreeModelEventArgs e)
        {
        }
        private void ProjectBrowserModel_NodesRemoved<TreeModelEventArgs>(object sender, TreeModelEventArgs e)
        {
        }
        private void ProjectBrowserModel_StructureChanged<TreePathEventArgs>(object sender, TreePathEventArgs e)
        {
        }
	}


    internal class FolderItemSorter : IComparer
    {
        private string _mode;
        private SortOrder _order;

        public FolderItemSorter(string mode, SortOrder order)
        {
            _mode = mode;
            _order = order;
        }

        public int Compare(object x, object y)
        {
            BaseItem a = x as BaseItem;
            BaseItem b = y as BaseItem;
            int res = 0;

            //if (_mode == "Date")
            //    res = DateTime.Compare(a.Date, b.Date);
            //else 
            if (_mode == "Count")
            {
                if (a.Count < b.Count)
                    res = -1;
                else if (a.Count > b.Count)
                    res = 1;
            }
            else if (_mode == "Total")
            {
                if (a.Total < b.Total)
                    res = -1;
                else if (a.Total > b.Total)
                    res = 1;
            }
            else if (_mode == "Kind")
            {
                res = string.Compare(a.Kind, b.Kind);
            }
            else
                res = string.Compare(a.Name, b.Name);

            if (_order == SortOrder.Ascending)
                return -res;
            else
                return res;
        }

        private string GetData(object x)
        {
            return (x as BaseItem).Name;
        }
    }
}

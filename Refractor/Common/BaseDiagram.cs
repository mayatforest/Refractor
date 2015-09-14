using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using WF = System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;
using GD=Microsoft.Glee.Drawing;
using GV=Microsoft.Glee.GraphViewerGdi;

namespace Refractor.Common
{
    /// <summary>
    /// A base class for diagrams using Glee.
    /// CreateBuilder must be overridden, to supply a matching builder.
    /// </summary>
    public partial class BaseDiagram : DockContent, IActiveItemViewer, IWindowPlugin
    {
        public BaseDiagram()
        {
            InitializeComponent();

            _options = new BaseDiagramOptions(this.GetID());

            _sidePanel = new SidePanel();
            _sidePanel.RefreshDiagram = new DRefreshDiagram(ForceRefresh);
            _sidePanel.HidePane = new DHidePane(HidePane);
            panel2.Controls.Add(_sidePanel.MainPanel);
            panel2.Visible = false;
            splitter1.Visible = false;

            // Using a single instance of the viewer UI control.
            _viewer = new GV.GViewer();
            _viewer.AsyncLayout = false;
            _viewer.Dock = WF.DockStyle.Fill;
            _viewer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            _viewer.SelectionChanged += new EventHandler(viewer_SelectionChanged);
            _viewer.MouseClick += new WF.MouseEventHandler(viewer_MouseClick);
            _viewer.MouseDown += new System.Windows.Forms.MouseEventHandler(viewer_MouseDown);
            _viewer.MouseUp += new System.Windows.Forms.MouseEventHandler(viewer_MouseUp);
            _viewer.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(viewer_MouseDoubleClick);
            _viewer.MouseMove += new System.Windows.Forms.MouseEventHandler(viewer_MouseMove);
            _viewer.OutsideAreaBrush = new Pen(Color.White).Brush;
            _viewer.Visible = false;
            _viewer.Font = new Font("Arial", 24, FontStyle.Bold);

            panel1.Controls.Add(_viewer);
            _viewer.BringToFront();

            lblCaption.BringToFront();
            lblCaption.Text = string.Empty;
            lblCaption.Top = 6;
            lblCaption.Left = 165;

            lblPleaseWait.Text = string.Empty;

            _timer = new Timer(_timer_Callback, null, Timeout.Infinite, 100);

            _clickTimer.Tick += new EventHandler(_clickTimer_Tick);
            _clickTimer.Interval = 200;

            this.TabText = GetID();
        }

        public virtual string GetID()
        {
            return string.Empty;
        }

        public virtual List<Type> GetHandledTypes()
        {
            return null;
        }

        public virtual WindowPluginKind GetKind()
        {
            return WindowPluginKind.ProjectItem;
        }

        public virtual void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _windowManager = (IWindowManager)serviceProvider.GetService(typeof(IWindowManager));
            _projectBrowser = (IProjectBrowser)serviceProvider.GetService(typeof(IProjectBrowser));
            _logView = (ILogView)serviceProvider.GetService(typeof(ILogView));
            _menuManager = (IMenuManager)serviceProvider.GetService(typeof(IMenuManager));

            _sidePanel.SetServiceProvider(_serviceProvider);
        }

        public virtual void BaseDiagram_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if (_thread != null && _thread.ThreadState != ThreadState.Stopped)
            {
                _timer.Change(Timeout.Infinite, 100);
                _thread.Abort();
                _thread = null;
            }
        }
        
        public virtual void SetActiveItem(BaseItem item)
        {
            if (item == null)
            {
                lblCaption.Text = string.Empty;
                lblPleaseWait.Text = string.Empty;
            }

            if (GetKind() == WindowPluginKind.ProjectItem)
            {
                // Use SetRefresh to refresh, so ignore calls with same item.
                if (item == _activeItem) return;
            }

            RunTranslate(item);
        }

        public void SetRefresh(BaseItem item)
        {
            BaseItem baseItem = GetActiveItem();
            if (baseItem == null) return;

            // We need access to the builder here :/
            if (baseItem != null && _builders.ContainsKey(baseItem))
            {
                BaseGraphBuilder builder = _builders[baseItem];
                if (item != null)
                {
                    string id = item.GetID();

                    // TODO - short id - added nodes do not have full ids.
                    string[] bits = id.Split('?');
                    if (bits.Length > 1)
                    {
                        id = bits[1];
                    }

                    if (builder.AddedNodes.ContainsKey(id))
                    {
                        // We know we added this node, so we do a refresh
                        ForceRefresh();
                    }
                }
            }
        }

        public BaseItem GetActiveItem()
        {
            return _activeItem;
        }

        public virtual PluginOptions GetOptions()
        {
            if (_options == null)
            {
                _options = new BaseDiagramOptions(this.GetID());
            }

            _options.PanelWidth = panel2.Width;
            _options.AutoRefresh = _sidePanel.AutoRefreshCheckBox.Checked;

            return _options;
        }

        public virtual void SetOptions(PluginOptions options)
        {
            if (options == null)
            {
                BaseDiagramOptions o = new BaseDiagramOptions(this.GetID());
                options = (PluginOptions)o;
            }
            
            if (options is BaseDiagramOptions)
            {
                _options = (BaseDiagramOptions)options;
                showMultipleEdgesToolStripMenuItem.Checked = _options.MultipleEdges;
                showMultipleEgdesAsThickLinesToolStripMenuItem.Checked = _options.ThickenEdges;
                useLeftToRightLayoutToolStripMenuItem.Checked = _options.LeftToRight;
                showOrphanNodesToolStripMenuItem.Checked = _options.ShowOrphans;

                if (_options.PanelWidth <= 0 || _options.PanelWidth > 500) _options.PanelWidth = 100;

                panel2.Width = _options.PanelWidth;
                _sidePanel.AutoRefreshCheckBox.Checked = _options.AutoRefresh;

            }
            else
            {
                _logView.LogStr("Warning : SetOptions reports invalid options type : " + options.GetType());
            }
        }

        protected IServiceProvider _serviceProvider;
        protected IWindowManager _windowManager;
        protected IProjectBrowser _projectBrowser;
        protected ILogView _logView;
        protected IMenuManager _menuManager;
        protected BaseItem _activeItem;
        protected BaseDiagramOptions _options;
        protected PluginOptions _parserOptions;
        protected SidePanel _sidePanel;

        protected virtual BaseGraphBuilder CreateBuilder()
        {
            // Subclasses create specific instance of builder.
            return null;
        }

        protected void RunTranslate(BaseItem item)
        {
            try
            {
                _activeItem = item;
                _failed = false;
                _viewer.Visible = false;
                _builders.Clear();

                lblCaption.Text = string.Empty;
                btnRefresh.Visible = false;
                if (item == null)
                {
                    lblPleaseWait.Text = string.Empty;
                }
                else
                {
                    lblPleaseWait.Text = "Please wait...";
                }

                if (item != null && !GetHandledTypes().Contains(item.GetType())) return;

                // We're on the main thread. We're using proper threads instead of
                // BackgroundWorkers so we can safely abort them. (The async mode
                // of GViewer does not abort quickly enough)
                if (_thread != null && _thread.ThreadState != ThreadState.Stopped)
                {
                    _timer.Change(Timeout.Infinite, 100);
                    _thread.Abort();
                    _thread = null;
                }

                if (item != null)
                {
                    _thread = new Thread(new ParameterizedThreadStart(thread_doWork));
                    _thread.IsBackground = true;
                    _thread.Priority = ThreadPriority.AboveNormal;

                    _logView.Debug("New thread:" + _thread.ManagedThreadId);

                    _thread.Start(item);
                    _timer.Change(0, 100);
                }
            }
            catch (Exception exc)
            {
                if (_logView.LogCatchAll(exc, "Unexpected plugin error")) throw;
            }
        }

        private GV.GViewer _viewer;
        private Thread _thread;
        private Timer _timer;
        private WF.Timer _clickTimer = new WF.Timer();
        private int _progress;
        private Dictionary<BaseItem, BaseGraphBuilder> _builders = new Dictionary<BaseItem, BaseGraphBuilder>();
        private const int _toolheight = 27;
        private bool _firstTime = true;
        private GD.Node _menuSelectedNode;
        private Dictionary<GD.Node, AffectedNode> _hiddenNodes = new Dictionary<Microsoft.Glee.Drawing.Node, AffectedNode>();
        private Dictionary<GD.Node, AffectedNode> _hiliteNodes = new Dictionary<Microsoft.Glee.Drawing.Node, AffectedNode>();
        private int _downX;
        private int _downY;
        private float _defaultZoom;
        private delegate void DSetGraph(GD.Graph graph);
        private delegate void DSetVisible(object flag);
        private bool _cancelClick;
        private GD.Node _prevNode;
        private int _prevIdx = 0;
        private bool _failed = false;
        private bool _mouseDown = false;

        private void _timer_Callback(object state)
        {
            // Don't deafen the listener. 
            if (_thread != null && _thread.ThreadState != ThreadState.Stopped)
            {
                // Progress update is more effort than all the rest of the threading issues.
                //_workersBrowser.UpdateThreadProgress(_thread, _progress);
            }
        }

        private void thread_doProgress(object sender)
        {
            _progress = (int)sender;
        }

        private void thread_doWork(object sender)
        {
            try
            {
                BaseItem item = sender as BaseItem;
                _progress = 0;

                // Do our side of the work, using the builder subclass.
                BaseGraphBuilder builder = CreateBuilder();
                float ratio = _viewer.Width / (float)(_viewer.Height - _toolheight);
                builder.SetServiceProvider(_serviceProvider,
                    new DThreadProgress(thread_doProgress), _sidePanel,
                    item, GetID(), ratio, _options);

                // Hang onto this for ignores.
                if (_builders.ContainsKey(item))
                    _builders[item] = builder;
                else
                    _builders.Add(item, builder);

                try
                {
                    try
                    {
                        builder.BeforeTranslate();
                        builder.Translate();
                    }
                    finally
                    {
                        builder.AfterTranslate();
                    }
                }
                catch (ArgumentException exc)
                {
                    _logView.LogExcStr(exc, "Failed during translate");
                    _failed = true;
                }
                catch (InvalidOperationException)
                {
                    // Collection modified, fail gracefully, don't restart
                    // automatically, as it could be a slow operation.
                    _logView.LogStr("Failed during translate of:" + item.Name);
                    _failed = true;
                }

                _logView.Debug("Visible false on UI thread");
                try
                {
                    if (this.IsHandleCreated)
                    {
                        this.Invoke((DSetVisible)SetViewerVisible, new object[] { false });
                    }
                }
                catch
                {

                }

                try
                {
                    _logView.Debug("Layout on this thread");

                    // This fails a lot inside the viewer component.
                    //_viewer.SetCalculatedLayout(layout);

                    // More stable, still sometimes fails.
                    object layout = _viewer.CalculateLayout(builder.Graph); 
                }
                catch (InvalidOperationException)
                {
                    _logView.LogStr("GLEE Layout error, try again with different aspect ratio");
                    _failed = true;
                }

                // This can take time itself, seconds for large graphs, and has to run on the 
                // UI thread. 
                _logView.Debug("Invoking SetGraph()");
                if (this.IsHandleCreated)
                {
                    this.Invoke((DSetGraph)SetGraph, new object[] { builder.Graph });
                }

                // Stop the progress timer.
                _timer.Change(Timeout.Infinite, 100);

                _logView.Debug("Thread finished");
            }
            catch (System.Threading.ThreadAbortException)
            {
                _failed = true;
            }
            catch (Exception exc)
            {
                if (_logView.LogCatchAll(exc, "Unexpected plugin error")) throw;
                _failed = true;
            }
        }

        private float olddefaultzoom = 1;
        private int oldnodecount = -1;
        private void SetGraph(GD.Graph graph)
        {
            try
            {
                string activeItemName = graph.UserData as string;

                // Can't work out how to set the caption, without also setting 
                // the tab text, which we do not want. This at least tells us.
                lblCaption.Text = activeItemName;

                this.ToolTipText = string.Format("{0} [{1}]",
                    GetID(), activeItemName);



                // This can fail with 'trim a spline' error.
                _viewer.Graph = graph;

                _viewer.Visible = true;

                _viewer.ZoomFraction = 0.1;
                //_viewer.ZoomMode

                // Heuristic adjustment.
                _defaultZoom = 1.0f;
                if (graph.NodeCount <= 1)
                {
                    _defaultZoom = 0.35f;
                }
                else if (graph.NodeCount <= 2)
                {
                    _defaultZoom = 0.45f;
                }
                else if (graph.NodeCount <= 3)
                {
                    _defaultZoom = 0.6f;
                }
                else if (graph.NodeCount <= 4)
                {
                    _defaultZoom = 0.8f;
                }
                if (oldnodecount == graph.NodeCount)
                {
                    _defaultZoom = olddefaultzoom;
                }

                _viewer.ZoomF = _defaultZoom;

                if (1==1)
                {
                    olddefaultzoom = (float)_viewer.ZoomF;
                    oldnodecount = _viewer.Graph.NodeCount;
                }
            }
            catch (InvalidOperationException)
            {
                _logView.LogStr("GLEE Layout error, try again with different aspect ratio");
                _failed = true;
            }

            // Show these, incase we've failed. Will be hidden if all ok.
            if (_failed)
            {
                lblPleaseWait.Text = "Layout failed";
                btnRefresh.Visible = true;
            }
        }
        
        private void SetViewerVisible(object flag)
        {
            // Used to hide only.
            try
            {
                if ((bool)flag == false)
                {
                    _viewer.Visible = false;
                }
            }
            catch (InvalidOperationException exc)
            {
                _logView.LogExc(exc);
            }
        }


        private void viewer_MouseClick(object sender, WF.MouseEventArgs e)
        {
            _clickTimer.Tag = e;
            _clickTimer.Start();

            
            if (UnHilite())
            {
                _viewer.DrawingPanel.Invalidate();
            }
        }

        private void viewer_SelectionChanged(object sender, EventArgs e)
        {
            // This happens even when the mouse is just moving over a node.
        }

        private void viewer_MouseMove(object sender, WF.MouseEventArgs e)
        {
            if (_mouseDown) return;
            if (!_options.HiliteNode) return;

            GD.Node node = GetObjectAt(e.X, e.Y) as GD.Node;

            bool refresh = false;

            if (!_options.HiliteLazy)
            {
                if (UnHilite()) 
                    refresh = true;
            }

            if (node != null)
            {
                if (_options.HiliteLazy)
                {
                    if (UnHilite()) 
                        refresh = true;
                }

                if (Hilite(node))
                    refresh = true;
            }

            if (refresh)
            {
                _viewer.DrawingPanel.Invalidate();
            }
        }

        private void viewer_MouseUp(object sender, WF.MouseEventArgs e)
        {
            if (_firstTime)
            {
                viewer_MouseClick(sender, e);
                _firstTime = false;
            }
            _mouseDown = false;
        }

        private void viewer_MouseDown(object sender, WF.MouseEventArgs e)
        {
            _downX = e.X;
            _downY = e.Y;
            _mouseDown = true;
        }

        private void viewer_MouseDoubleClick(object sender, WF.MouseEventArgs e)
        {
            _cancelClick = true;

            GD.Node node = GetObjectAt(e.X, e.Y) as GD.Node;
            BaseItem item = GetItem(node);

            if (item == null)
            {
                // Use double click on whitespace to toggle ignores visible.
                hideShowLocalIgnoresToolStripMenuItem_Click(null, null);
            }
            else
            {
                // Use double click to toggle ignore status.
                List<BaseItem> list = GetItemList(node);
                _sidePanel.ToggleList(list);
                if (_sidePanel.AutoRefreshCheckBox.Checked)
                {
                    ForceRefresh();
                }
            }
        }

        private void _clickTimer_Tick(object sender, EventArgs e)
        {
            _clickTimer.Stop();

            WF.MouseEventArgs me = _clickTimer.Tag as WF.MouseEventArgs;

            if (_cancelClick)
            {
                _cancelClick = false;
                return;
            }

            if (_activeItem == null) return;

            GD.Node node = GetObjectAt(me.X, me.Y) as GD.Node;
            BaseItem item = GetItem(node);

            if (me.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (item != null)
                {
                    ShowContextMenu(me.X, me.Y);
                }
                else
                {
                    if (_viewer.ZoomF == _defaultZoom)
                    {
                        // If we're fully zoomed out, then wherever we are, show the context menu.
                        ShowContextMenu(me.X, me.Y);
                    }
                    else
                    {
                        // Reset zoom. Would be nice to go "back" instead, but can't see how.
                        _viewer.ZoomF = _defaultZoom;
                    }
                }
            }
            else if (Math.Abs(_downX - me.X) > 2 || Math.Abs(_downY - me.Y) > 2)
            {
                // It was a zoom operation, don't select the item.
            }
            else
            {
                if (WF.Control.ModifierKeys == WF.Keys.Control)
                {
                    // Use control to handle local ignores.
                    List<BaseItem> list = GetItemList(node);
                    _sidePanel.ToggleList(list);

                    if (_sidePanel.AutoRefreshCheckBox.Checked)
                    {
                        ForceRefresh();
                    }
                }
                else
                {
                    _windowManager.SetActiveItem(item, this);
                }
            }
        }

        private bool Hilite(GD.Node node)
        {
            bool result = false;

            if (_hiliteNodes.ContainsKey(node)) return result;

            _hiliteNodes.Add(node, new AffectedNode(node.Attr.Color, node.Attr.Fillcolor, node.Attr.Fontcolor));

            Color color = _options.HiliteColor;
            GD.Color gcolor = new GD.Color(color.A, color.R, color.G, color.B);
            node.Attr.Fillcolor = gcolor;

            color = _options.HiliteLineColor;
            gcolor = new GD.Color(color.A, color.R, color.G, color.B);

            // We need access to the builder here :/
            if (_activeItem != null && _builders.ContainsKey(_activeItem))
            {
                BaseGraphBuilder builder = _builders[_activeItem];
                foreach (KeyValuePair<string, List<object>> pair in builder.AddedEdges)
                {
                    foreach (GD.Edge edge in pair.Value)
                    {
                        if (edge.Source == node.Id || edge.Target == node.Id)
                        {
                            // Abuse the userdata to hang onto the old color.
                            edge.UserData = new AffectedEdge(edge.Attr.Color);
                            edge.Attr.Color = gcolor;
                        }
                    }
                }
                result = true;
            }
            return result;
        }

        private bool UnHilite()
        {
            bool result = false;

            // Check for any to clear.
            if (_hiliteNodes.Count > 0)
            {
                foreach (KeyValuePair<GD.Node, AffectedNode> pair in _hiliteNodes)
                {
                    GD.Node node = pair.Key;
                    AffectedNode hidden = pair.Value;
                    node.Attr.Color = hidden.Color;
                    node.Attr.Fillcolor = hidden.FillColor;
                    node.Attr.Fontcolor = hidden.FontColor;
                }

                // Again, we need access to the builder here :/ 
                if (_activeItem != null && _builders.ContainsKey(_activeItem))
                {
                    BaseGraphBuilder builder = _builders[_activeItem];
                    foreach (KeyValuePair<string, List<object>> pair in builder.AddedEdges)
                    {
                        foreach (GD.Edge edge in pair.Value)
                        {
                            AffectedEdge hidden = edge.UserData as AffectedEdge;

                            if (hidden != null)
                            {
                                edge.Attr.Color = hidden.Color;
                                edge.UserData = null;
                            }
                        }
                    }

                    _hiliteNodes.Clear();
                    result = true;
                }
            }

            return result;
        }

        private object GetObjectAt(int x, int y)
        {
            object result = null;
            try
            {
                result = _viewer.GetObjectAt(x, y);
            }
            catch (NullReferenceException)
            {
                // Just ignore this, user will click again.
                //_logView.LogExcStr(exc, "Glee viewer reports null reference");
            }
            return result;
        }

        private BaseItem GetItem(GD.Node node)
        {
            BaseItem item = null;

            if (node != null && node.UserData != null)
            {
                if (node.UserData is BaseItem)
                {
                    item = node.UserData as BaseItem;
                }
                else if (node.UserData is List<BaseItem>)
                {
                    // Iterate over nodes with multiple items.
                    if (_prevNode == node) _prevIdx++;
                    else _prevIdx = 0;

                    List<BaseItem> list = node.UserData as List<BaseItem>;
                    item = list[_prevIdx % list.Count];
                }
                else _logView.Debug("GetItem reports invalid UserData");
                _prevNode = node;
            }

            return item;
        }

        private List<BaseItem> GetItemList(GD.Node node)
        {
            List<BaseItem> list = new List<BaseItem>();

            if (node != null && node.UserData != null)
            {
                if (node.UserData is BaseItem)
                {
                    BaseItem item = node.UserData as BaseItem;
                    list.Add(item);
                }
                else if (node.UserData is List<BaseItem>)
                {
                    list = node.UserData as List<BaseItem>;
                }
                else _logView.Debug("GetItemList reports invalid UserData");
            }

            return list;
        }

        private void ShowContextMenu(int x, int y)
        {
            _menuSelectedNode = GetObjectAt(x, y) as GD.Node;

            contextMenuStrip1.Show(this, x, y + _toolheight);
        }
        
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            WF.ContextMenuStrip menu = (WF.ContextMenuStrip)sender;
            menu.Items.Clear();

            BaseItem item = GetItem(_menuSelectedNode);


            if (item != null)
            {
                // Update the menu depending on the type of the item before we show it.
                _menuManager.UpdateContextMenu(menu, item);
                menu.Items.Add(new WF.ToolStripSeparator());
            }


            // Always provide the general options.
            menu.Items.Add(refreshToolStripMenuItem);
            menu.Items.Add(diagramOptionsToolStripMenuItem);

            menu.Items.Add(new WF.ToolStripSeparator());
            menu.Items.Add(showAllHiddenToolStripMenuItem);
            if (item != null) menu.Items.Add(hideNodeToolStripMenuItem);

            menu.Items.Add(hideShowLocalIgnoresToolStripMenuItem);
            if (item != null) menu.Items.Add(toggleLocalIgnoreToolStripMenuItem);

            e.Cancel = false;
        }

        private void ForceRefresh()
        {
            BaseItem item = _activeItem;
            _activeItem = null;
            
            RunTranslate(item);
        }

        private void HidePane()
        {
            panel2.Visible = false;
            splitter1.Visible = false;
            hideShowLocalIgnoresToolStripMenuItem.Checked = false;
        }


        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ForceRefresh();
        }

        private void showMultipleEdgesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showMultipleEdgesToolStripMenuItem.Checked = 
                !showMultipleEdgesToolStripMenuItem.Checked;
            _options.MultipleEdges = showMultipleEdgesToolStripMenuItem.Checked;
            ForceRefresh();
        }

        private void showMultipleEgdesAsThickLinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showMultipleEgdesAsThickLinesToolStripMenuItem.Checked = 
                !showMultipleEgdesAsThickLinesToolStripMenuItem.Checked;
            _options.ThickenEdges = showMultipleEgdesAsThickLinesToolStripMenuItem.Checked;
            ForceRefresh();
        }

        private void useLeftToRightLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            useLeftToRightLayoutToolStripMenuItem.Checked = 
                !useLeftToRightLayoutToolStripMenuItem.Checked;
            _options.LeftToRight = useLeftToRightLayoutToolStripMenuItem.Checked;
            ForceRefresh();
        }

        private void showOrphanNodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showOrphanNodesToolStripMenuItem.Checked = 
                !showOrphanNodesToolStripMenuItem.Checked;
            _options.ShowOrphans = showOrphanNodesToolStripMenuItem.Checked;
            ForceRefresh();
        }        

        private void hideNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GD.Node node = _menuSelectedNode;
            if (node == null) return;
            if (_hiddenNodes.ContainsKey(node)) return;

            // Hiding means the layout is still affected by the node,
            // but we don't have to recalculate the layout.            
            // NB Setting Style.Invis doesn't work, this is a workaround.
            _hiddenNodes.Add(node, new AffectedNode(node.Attr.Color, node.Attr.Fillcolor, node.Attr.Fontcolor));

            node.Attr.Color = GD.Color.Transparent; 
            node.Attr.Fillcolor = GD.Color.Transparent;
            node.Attr.Fontcolor = GD.Color.Transparent;

            // We need access to the builder here :/
            if (_activeItem != null && _builders.ContainsKey(_activeItem))
            {
                BaseGraphBuilder builder = _builders[_activeItem];
                foreach (KeyValuePair<string, List<object>> pair in builder.AddedEdges)
                {
                    foreach (GD.Edge edge in pair.Value)
                    {
                        if (edge.Source == node.Id || edge.Target == node.Id)
                        {
                            // Abuse the userdata to hang onto the old color.
                            edge.UserData = new AffectedEdge(edge.Attr.Color);
                            edge.Attr.Color = GD.Color.Transparent;
                        }
                    }
                }
            }
            _viewer.Refresh();
        }

        private void showAllHiddenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Grim workaround for not being able to invisible the nodes.
            foreach (KeyValuePair<GD.Node, AffectedNode> pair in _hiddenNodes)
            {
                GD.Node node = pair.Key;
                AffectedNode hidden = pair.Value;
                node.Attr.Color = hidden.Color;
                node.Attr.Fillcolor = hidden.FillColor;
                node.Attr.Fontcolor = hidden.FontColor;
            }

            // We need access to the builder here :/
            if (_activeItem != null && _builders.ContainsKey(_activeItem))
            {
                BaseGraphBuilder builder = _builders[_activeItem];
                foreach (KeyValuePair<string, List<object>> pair in builder.AddedEdges)
                {
                    foreach (GD.Edge edge in pair.Value)
                    {
                        AffectedEdge hidden = edge.UserData as AffectedEdge;

                        if (hidden != null)
                        {
                            edge.Attr.Color = hidden.Color;
                            edge.UserData = null;
                        }
                    }
                }
            }

            _hiddenNodes.Clear();
            _viewer.Refresh();
        }

        private void hideShowLocalIgnoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hideShowLocalIgnoresToolStripMenuItem.Checked = !hideShowLocalIgnoresToolStripMenuItem.Checked;
            panel2.Visible = hideShowLocalIgnoresToolStripMenuItem.Checked;
            splitter1.Visible = panel2.Visible;
        }

        private void toggleSidePanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BaseItem item = GetItem(_menuSelectedNode);
            if (item == null) return;

            List<BaseItem> list = GetItemList(_menuSelectedNode);
            _sidePanel.ToggleList(list);

            if (_sidePanel.AutoRefreshCheckBox.Checked)
            {
                ForceRefresh();
            }
        }


        private void btnRefresh_Click(object sender, EventArgs e)
        {
            // If we failed, this button at least should be present, as the right click
            // menu will not be available.
            ForceRefresh();
        }

        private void lblCaption_Click(object sender, EventArgs e)
        {
            _windowManager.SetActiveItem(_activeItem, this);
        }

        private void panel1_DoubleClick(object sender, EventArgs e)
        {
            hideShowLocalIgnoresToolStripMenuItem_Click(null, null);
        }

        private class AffectedNode
        {
            internal GD.Color Color;
            internal GD.Color FillColor;
            internal GD.Color FontColor;
            internal AffectedNode(GD.Color color, GD.Color fillColor, GD.Color fontColor)
            {
                Color = color;
                FillColor = fillColor;
                FontColor = fontColor;
            }
        }
        
        private class AffectedEdge
        {
            internal GD.Color Color;
            internal AffectedEdge(GD.Color color)
            {
                Color = color;
            }
        }
    }

    public delegate void DThreadProgress(object sender);
    public delegate void DRefreshDiagram();
    public delegate void DHidePane();

}

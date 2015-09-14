using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.ComponentModel;
using System.Drawing;

using Refractor.Common;
using GD = Microsoft.Glee.Drawing;
using MG = Microsoft.Glee;

namespace Refractor.Common
{        
    /// <summary>
    /// Graph builder base class, abstracting away the graph implementation.
    /// Single hit - no clear.
    /// </summary>
    public class BaseGraphBuilder
    {        
        public virtual void SetServiceProvider(IServiceProvider serviceProvider,
            DThreadProgress threadProgress, SidePanel sidePanel, 
            BaseItem item, string id, float ratio,
            BaseDiagramOptions options)
        {
            _windowManager = (IWindowManager)serviceProvider.GetService(typeof(IWindowManager));
            _logView = (ILogView)serviceProvider.GetService(typeof(ILogView));
            _projectBrowser = (IProjectBrowser)serviceProvider.GetService(typeof(IProjectBrowser));
            _threadProgress = threadProgress;
            _sidePanel = sidePanel;
            _activeItem = item;
            _id = id;
            _ratio = ratio;
            _options = options;
        }

        public virtual void BeforeTranslate()
        {
        }

        public virtual void Translate()
        {
        }

        public virtual void AfterTranslate()
        {
            // Post translate now builds the actual graph.
            BuildGraph();

            if (_addedNodes.Count == 0)
            {
                GD.Node node = _graph.AddNode("default");
                string text = GetEmptyText();
                node.Attr.Label = text;
                node.Attr.Color = GD.Color.Transparent;
            }
        }

        public Dictionary<string, object> AddedNodes { get { return _addedNodes; } }
        public Dictionary<string, List<object>> AddedEdges { get { return _addedEdges; } }
        internal GD.Graph Graph { get { return _graph; } }
        protected IWindowManager _windowManager;
        protected IProjectBrowser _projectBrowser;
        protected ILogView _logView;
        protected BackgroundWorker _worker;
        protected Dictionary<string, object> _addedNodes = new Dictionary<string, object>();
        protected Dictionary<string, List<object>> _addedEdges = new Dictionary<string, List<object>>();    
        protected Dictionary<string, object> _notOrphans = new Dictionary<string, object>();
        protected int _progress;
        protected BaseItem _activeItem;
        protected string _id;
        protected BaseDiagramOptions _options;
        protected bool _hasOrphans;
        protected SidePanel _sidePanel;
        protected bool _ignoreSidePanelItems = true;

        protected virtual string GetEmptyText()
        {
            string result = " Empty Graph ";
            if (_sidePanel.Count > 0)
            {
                if (_hasOrphans)
                    result = " Empty Graph \n (all ignored) \n (has orphans) ";
                else
                    result = " Empty Graph \n (all ignored) ";
            }
            else
            {
                if (_hasOrphans)
                    result = " Empty Graph \n (has orphans)";
                else
                    result = " Empty Graph ";
            }
            return result;
        }

        protected object AddNode(string id, string caption, Color color, BaseItem item)
        {
            return AddNode(id, caption, color, item, false);   
        }

        protected object AddNode(string id, string caption, Color color, BaseItem item, bool allowDuplicates)
        {
            if (_ignoreSidePanelItems && _sidePanel.Lookup(id)) return null;

            GD.Node node = null;
            
            if (_addedNodes.ContainsKey(id))
            {
                if (allowDuplicates)
                {
                    node = (GD.Node)_addedNodes[id];

                    if (node.UserData is BaseItem)
                    {
                        List<BaseItem> list = new List<BaseItem>();
                        list.Add((BaseItem)node.UserData);
                        list.Add(item);
                        node.UserData = list;
                    }
                    else if (node.UserData is List<BaseItem>)
                    {
                        List<BaseItem> list = node.UserData as List<BaseItem>;
                        list.Add(item);
                    }
                    else _logView.Debug("AddNode reports invalid UserData");
                }
            }
            else
            {
                // NB Don't add it to the graph yet.
                node = new GD.Node(id);

                node.UserData = (object)item;
                node.Attr.Label = caption;

                GD.Color gcolor = new GD.Color(color.A, color.R, color.G, color.B);
                node.Attr.Fillcolor = gcolor;

                _addedNodes.Add(id, node);
            }   
            
            return node;
        }

        protected object AddEdge(string idFrom, string idTo, EdgeStyle edgeStyle)
        {
            if (_ignoreSidePanelItems && _sidePanel.Lookup(idFrom)) return null;
            if (_ignoreSidePanelItems && _sidePanel.Lookup(idTo)) return null;

            string key = idFrom + "?" + idTo;
            bool edgePresent = _addedEdges.ContainsKey(key);

            if (!_options.MultipleEdges && edgePresent) return null;

            // Don't add it to the graph yet.
            GD.Edge edge = new GD.Edge(idFrom, string.Empty, idTo);

            if (edgeStyle == EdgeStyle.NormalArrow)
                edge.Attr.ArrowHeadAtTarget = GD.ArrowStyle.Normal;
            else
                edge.Attr.ArrowHeadAtTarget = GD.ArrowStyle.None;
            
            if (_options.ShowOrphans)
            {
                // Things are easy if we're showing everything.
                if (!edgePresent) _addedEdges.Add(key, new List<object>());
                _addedEdges[key].Add(edge);
            }
            else
            {
                // Nodes linked to themselves still count as orphans.
                if (idFrom != idTo)
                {
                    // Update non orphan list.
                    if (!_notOrphans.ContainsKey(idFrom)) _notOrphans.Add(idFrom, true);
                    if (!_notOrphans.ContainsKey(idTo)) _notOrphans.Add(idTo, true);

                    if (!edgePresent) _addedEdges.Add(key, new List<object>());
                    _addedEdges[key].Add(edge);
                }
                else
                {
                    // We still want to show cyclic, as long as their not orphans.
                    if (_notOrphans.ContainsKey(idFrom) || _notOrphans.ContainsKey(idTo))
                    {
                        if (!edgePresent) _addedEdges.Add(key, new List<object>());
                        _addedEdges[key].Add(edge);
                    }
                }
            }

            return edge;
        }

        protected object AddEdge(string idFrom, string idTo, EdgeStyle edgeStyle, Color color)
        {
            GD.Edge edge = (GD.Edge)AddEdge(idFrom, idTo, edgeStyle);
            if (edge == null) return null;

            GD.Color gcolor = new GD.Color(color.A, color.R, color.G, color.B);
            edge.Attr.Color = gcolor;

            return edge;
        }

        protected void AppendNodeCaption(object node, string text)
        {
            if (node == null) return;

            ((GD.Node)node).Attr.Label += text;
        }

        protected void UpdateNodeColor(object node, Color color)
        {
            if (node == null) return;

            GD.Color gcolor = new GD.Color(color.A, color.R, color.G, color.B);
            ((GD.Node)node).Attr.Fillcolor = gcolor;
        }

        protected bool CheckWorker()
        {
            _threadProgress(_progress);

            return false; //
        }

        private GD.Graph _graph;
        private DThreadProgress _threadProgress;
        private float _ratio = 2.0f;

        private GD.Graph CreateGraph()
        {
            GD.Graph result = new GD.Graph(_id);

            // Hang onto the name of the active item for SetGraph.
            if (_activeItem != null)
            {
                result.UserData = _activeItem.Name;
            }

            result.GraphAttr.NodeAttr.FontName = "Microsoft Sans Serif";
            result.GraphAttr.NodeAttr.Fontsize = 8;
            result.GraphAttr.NodeAttr.Shape = GD.Shape.Box;
            result.GraphAttr.NodeAttr.Fillcolor = GD.Color.WhiteSmoke;

            result.GraphAttr.EdgeAttr.FontName = "Tahoma";
            result.GraphAttr.EdgeAttr.Fontsize = 8;

            result.GraphAttr.AspectRatio = _ratio;

            if (_options.LeftToRight)
            {
                result.GraphAttr.LayerDirection = GD.LayerDirection.LR;
            }
            else
            {
                result.GraphAttr.LayerDirection = GD.LayerDirection.TB;
            }

            return result;
        }

        private void BuildGraph()
        {
            // The only way I can see to do this is to create another graph, and 
            // copy what we want to the new graph, which is slow. Grim, we need
            // to rebuild addedNodes and edges as well.
            
            // Considering options, it's easier to do this each time anyway,
            // and don't build the initial graph at all. TODO, split this.

            GD.Graph newGraph = CreateGraph();
            Dictionary<string, object> newAddedNodes = new Dictionary<string, object>();
            Dictionary<string, List<object>> newAddedEdges = new Dictionary<string, List<object>>();

            foreach (KeyValuePair<string, object> pair in _addedNodes)
            {
                GD.Node node = pair.Value as GD.Node;
                
                if (!_options.ShowOrphans && !_notOrphans.ContainsKey(pair.Key)) continue;

                GD.Node newNode = newGraph.AddNode(node.Id);
                newNode.Attr = node.Attr.Clone();
                newNode.Attr.LabelMargin = _options.TextSpacing;

                newNode.UserData = node.UserData;
                newAddedNodes.Add(node.Id, newNode);
            }

            foreach (KeyValuePair<string, List<object>> pair in _addedEdges)
            {
                if (pair.Value.Count == 0) return;

                string idFrom = (pair.Value[0] as GD.Edge).Source;
                string idTo = (pair.Value[0] as GD.Edge).Target;

                if (_options.MultipleEdges)
                {
                    int count = _addedEdges[pair.Key].Count;

                    // Perhaps thicken edges. 
                    if (_options.ThickenEdges)
                    {
                        // Interesting, but any greater than 2 looks bad.
                        int thickness = 1;
                        if (count > 1) thickness = 2;

                        // Create a thickened edge. Take the properties of the first one.
                        GD.Edge edge = pair.Value[0] as GD.Edge;
                        GD.Edge newEdge = newGraph.AddEdge(idFrom, idTo);
                        newEdge.Attr = edge.Attr.Clone();
                        newEdge.Attr.LineWidth = thickness;
                        newAddedEdges.Add(pair.Key, new List<object>() { newEdge } );
                    }
                    else
                    {
                        // Show multiple edges.
                        newAddedEdges.Add(pair.Key, new List<object>());
                        foreach (GD.Edge edge in pair.Value)
                        {
                            GD.Edge newEdge = newGraph.AddEdge(idFrom, idTo);
                            newEdge.Attr = edge.Attr.Clone();
                            newAddedEdges[pair.Key].Add(newEdge);
                        }
                    }
                }
                else
                {
                    // Show single edges. Take the properties of the first one.
                    GD.Edge edge = pair.Value[0] as GD.Edge;
                    GD.Edge newEdge = newGraph.AddEdge(idFrom, idTo);
                    newEdge.Attr = edge.Attr.Clone();
                    newAddedEdges.Add(pair.Key, new List<object> { newEdge });
                }
            }

            _hasOrphans = _notOrphans.Count < _addedNodes.Count;

            _graph = newGraph;
            _addedEdges = newAddedEdges;
            _addedNodes = newAddedNodes;
        }        

    }

    public enum EdgeStyle { NormalArrow, None }
}

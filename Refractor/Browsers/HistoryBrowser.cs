using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

using Refractor.Common;
using WeifenLuo.WinFormsUI.Docking;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

namespace Refractor
{
    /// <summary>
    /// A list of BaseItems that can be persisted.
    /// </summary>
    internal partial class HistoryBrowser : DockContent
    {
        public HistoryBrowser(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            _windowManager = (WindowManager)serviceProvider.GetService(typeof(WindowManager));

            _nodeTextBox = new NodeTextBox();
            _nodeTextBox.ToolTipProvider = new ToolTipProvider();
            _nodeTextBox.DataPropertyName = "Text";
            _nodeTextBox.EditEnabled = true;
            _treeView.NodeControls.Add(this._nodeTextBox);

            _model = new TreeModel();
            _treeView.Model = _model;        
        }

        public void Add(BaseItem item)
        {
            if (_currentItem == item) return;

            Node node = new Node(item.Name);
            _items.Add(node, item);

            if (_currentNode != null)
            {
                if (_currentNode.NextNode == null)
                {
                    Node parent = _currentNode.Parent;

                    if (parent != null)
                    {
                        parent.Nodes.Add(node);
                        node.Parent = parent;
                    }
                    else
                    {
                        _model.Nodes.Add(node);
                    }
                }
                else
                {
                    Node parent = _currentNode;
                    parent.Nodes.Add(node);
                    node.Parent = parent;
                }
            }
            else
            {
                _model.Nodes.Add(node);
            }

            _currentItem = item;
            _currentNode = node;

            try
            {
                _ignoreSelect = true;
                _treeView.SelectedNode = _treeView.FindNode(_model.GetPath(node));
            }
            finally
            {
                _ignoreSelect = false;
            }

            SetCaption();
        }

        public BaseItem GoBack()
        {
            Node node = null;

            if (_currentNode != null)
            {
                Node parent = _currentNode.Parent;

                if (parent != null)
                {
                    // Find the previous sibling.
                    int idx = parent.Nodes.IndexOf(_currentNode);

                    if (idx > 0)
                        node = parent.Nodes[idx - 1];
                    else
                        node = parent;
                }
            }

            if ((node != null) && (node.Parent != null))
            {
                _currentNode = node;
                _currentItem = _items[node];
                _treeView.SelectedNode = _treeView.FindNode(_model.GetPath(node));
                SetCaption();
                _windowManager.ProjectBrowser.SetActiveItem(_currentItem, this);
            }

            return _currentItem;
        }

        public BaseItem GoForward()
        {
            Node node = null;

            if (_currentNode != null)
            {
                Node parent = _currentNode.Parent;

                if (parent != null)
                {
                    // Find the next sibling.
                    int idx = parent.Nodes.IndexOf(_currentNode);

                    if (idx < parent.Nodes.Count - 1)
                    {
                        node = parent.Nodes[idx + 1];
                    }
                }
            }

            if ((node != null) && (node.Parent != null))
            {
                _currentNode = node;
                _currentItem = _items[node];
                _treeView.SelectedNode = _treeView.FindNode(_model.GetPath(node));
                SetCaption();
                _windowManager.ProjectBrowser.SetActiveItem(_currentItem, this);
            }

            return _currentItem;
        }

        public void Delete(BaseItem item)
        {
            SetCaption();
        }

        public void Clear()
        {
            _model.Nodes.Clear();
            _items.Clear();
        }


        private WindowManager _windowManager;
        private TreeModel _model;
        private NodeTextBox _nodeTextBox;
        private BaseItem _currentItem;
        private Node _currentNode;
        private Dictionary<Node, BaseItem> _items = new Dictionary<Node, BaseItem>();
        private bool _ignoreSelect;

        private class ToolTipProvider : IToolTipProvider
        {
            public string GetToolTip(TreeNodeAdv node, NodeControl nodeControl)
            {
                // TODO
                return string.Empty; 
            }
        }

        private void SetCaption()
        {
            this.Text = string.Format("History [{0}]",
                _currentItem.Name);
        }

        private void goBackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GoBack();
        }

        private void goForwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GoForward();
        }

        private void _treeView_SelectionChanged(object sender, EventArgs e)
        {
            if (_ignoreSelect) return;

            if (_treeView.SelectedNode == null) return;

            _currentNode = _treeView.SelectedNode.Tag as Node;
            _currentItem = _items[_currentNode];
                        
            _windowManager.SetActiveItem(_currentItem, this);
        }

    }



}

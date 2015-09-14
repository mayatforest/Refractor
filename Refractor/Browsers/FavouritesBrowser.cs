using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// A list of favourite BaseItems. These may or may not be present in 
    /// current project, and must persist so are referenced by id rather 
    /// than instance.
    /// </summary>
    internal partial class FavouritesBrowser : DockContent
    {
        internal FavouritesBrowser(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            _windowManager = (WindowManager)serviceProvider.GetService(typeof(WindowManager));
            
            // We're using a static Node sublcassed model for this aga tree.
            _nodeTextBox.ToolTipProvider = new FavouritesToolTipProvider();
            _nodeTextBox.DrawText += new EventHandler<DrawEventArgs>(_nodeTextBox_DrawText);            
            _treeModel = new TreeModel();
            _tree.NodeMouseClick += new EventHandler<TreeNodeAdvMouseEventArgs>(_tree_NodeMouseClick);
            _tree.Model = _treeModel;
            _tree.BackColor = Color.LightGoldenrodYellow;

            _windowManager.ProjectBrowser.OnFilesChanged += new EventHandler(ProjectBrowser_OnFilesChanged);
        }

        internal virtual void Clear()
        {
            _items.Clear();

            _tree.BeginUpdate();
            _treeModel.Nodes.Clear();
            _tree.EndUpdate();

            _addedPaths.Clear();
        }

        internal virtual void Add(BaseItem item)
        {
            Node parent = null;
            if (_tree.SelectedNode != null) 
                parent = _tree.SelectedNode.Tag as Node;

            AddWithParent(item, parent, -1);
        }

        internal virtual void Remove(BaseItem item)
        {
            string id = item.GetID();

            if (!_items.ContainsKey(id))
            {
                _windowManager.Logger.LogStr("Warning : Favourite not found to remove : " + id);
                return;
            }

            RemoveInternal(id);
        }

        internal void AddAll(List<FavouritesItem> items)
        {
            _tree.BeginUpdate();

            // Add from a list of items, from definition.
            foreach (FavouritesItem fave in items)
            {
                if (fave.Id != null)
                {
                    if (_items.ContainsKey(fave.Id)) continue;

                    BaseItem item = _windowManager.ProjectBrowser.Lookup(fave.Id);

                    FavouritesNode node;
                    if (item == null)
                    {
                        node = new FavouritesNode(fave.Id, fave.Id, false, null);
                    }
                    else
                    {
                        node = new FavouritesNode(item.Name, fave.Id, false, item);
                    }

                    // Update the folder structure.
                    AddNodeAndPath(fave.Path, node);

                    // And the items list.
                    AddInternal(fave.Id, node);
                }
            }

            _tree.EndUpdate();
        }

        internal void GetAll(List<FavouritesItem> items)
        {
            // Create a list of FavouritesItem to persist.
            items.Clear();
            foreach (TreeNodeAdv tnode in _tree.AllNodes)
            {
                FavouritesNode node = tnode.Tag as FavouritesNode;
                if (node == null) continue;
                if (node.IsFolder) continue;

                TreePath treePath = _treeModel.GetPath(node);
                string path = string.Empty;

                string id = node.Id;

                int idx = 1;
                foreach (FavouritesNode pathNode in treePath.FullPath)
                {
                    if (idx == treePath.FullPath.Length && !pathNode.IsFolder) break;
                    idx++;
                    path = path + pathNode.Text + "/";
                }

                // Don't add the folders themselves.
                if (id != null)
                {
                    items.Add(new FavouritesItem(id, path));
                }
            }
        }

        protected WindowManager _windowManager;
        private Dictionary<string, FavouritesNode> _items = new Dictionary<string, FavouritesNode>();
        private TreeModel _treeModel;
        private Dictionary<string, Node> _addedPaths = new Dictionary<string, Node>();

        protected virtual void AddInternal(string id, FavouritesNode node)
        {
            _items.Add(id, node);
        }

        protected virtual void RemoveInternal(string id)
        {
            FavouritesNode node = _items[id];

            if (node.Parent == null)
            {
                _treeModel.Nodes.Remove(node);
            }
            else
            {
                node.Parent.Nodes.Remove(node);
            }
            _items.Remove(id);
        }

        private void AddWithParent(BaseItem item, Node parent, int idx)
        {
            string id = item.GetID();

            if (_items.ContainsKey(id)) return;

            Node node = null;
            if (parent != null)
            {
                if (parent is FavouritesNode && (parent as FavouritesNode).IsFolder)
                {
                    // Standard.
                    node = new FavouritesNode(item.Name, id, false, item);
                    if (idx == -1) parent.Nodes.Add(node);
                    else parent.Nodes.Insert(idx, node);
                }
                else
                {
                    // May be the "root" Node.
                    node = new FavouritesNode(item.Name, id, false, item);

                    if (parent.Parent != null)
                    {
                        if (idx == -1) parent.Parent.Nodes.Add(node);
                        else parent.Parent.Nodes.Insert(idx, node);
                    }
                    else
                    {
                        if (idx == -1) _treeModel.Nodes.Add(node);
                        else _treeModel.Nodes.Insert(idx, node);
                    }
                }
            }
            else
            {
                node = new FavouritesNode(item.Name, id, false, item);
                _treeModel.Nodes.Add(node);
            }

            AddInternal(id, (FavouritesNode)node);
        }

        private void AddNodeAndPath(string path, FavouritesNode node)
        {
            if (path == string.Empty)
            {
                // No folder.
                _treeModel.Nodes.Add(node);
                return;
            }

            if (_addedPaths.ContainsKey(path))
            {
                // Folder already added.
                _addedPaths[path].Nodes.Add(node);
                return;
            }

            // Folder not added yet, so add it.
            string[] bits = path.Split('/');
            string partial = string.Empty;
            Collection<Node> nodes = _treeModel.Nodes;
            FavouritesNode folderNode = null;
            foreach (string bit in bits)
            {
                if (string.IsNullOrEmpty(bit)) continue;

                // Forward slash folder seperater.
                partial += bit + "/";

                if (_addedPaths.ContainsKey(partial))
                {
                    nodes = _addedPaths[partial].Nodes;
                    continue;
                }

                folderNode = new FavouritesNode(bit, partial, true, null);
                nodes.Add(folderNode);
                nodes = folderNode.Nodes;
                
                // Keep a note of the folders.
                _addedPaths.Add(partial, folderNode);
            }

            if (folderNode == null) return;

            folderNode.Nodes.Add(node);
        }

        private void SelectNode()
        {
            if (_tree.SelectedNode == null) return;

            FavouritesNode node = _tree.SelectedNode.Tag as FavouritesNode;

            if (node == null) return;
            if (node.IsFolder) return;
            
            // Look it up each time, incase it's changed, file reload etc.
            BaseItem item = _windowManager.ProjectBrowser.Lookup(node.Id);

            // And reset it.
            node.Item = item;

            if (item != null)
            {
                _windowManager.SetActiveItem(item, this);
            }
        }

        private void ProjectBrowser_OnFilesChanged(object sender, EventArgs e)
        {
           // Make sure the items are up to date, or null if no longer present.
           foreach (KeyValuePair<string, FavouritesNode> pair in _items)
           {
               if ((pair.Value) == null) continue;
               pair.Value.Item = _windowManager.ProjectBrowser.Lookup(pair.Key);
           }
        }

        
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void newFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string partial = string.Empty;

            FavouritesNode folderNode = new FavouritesNode("New Folder", partial, true, null);
            
            if (_tree.SelectedNode != null)        
            {
                FavouritesNode selectedNode = _tree.SelectedNode.Tag as FavouritesNode;

                if (selectedNode.IsFolder)
                {
                    selectedNode.Nodes.Add(folderNode);
                }
                else
                {
                    int insertIdx;
                    if (_tree.SelectedNode.Parent == null)
                    {
                        // Insert at root level.
                        insertIdx = _treeModel.Nodes.IndexOf(selectedNode);
                        _treeModel.Nodes.Insert(insertIdx, folderNode);
                    }
                    else
                    {
                        // Insert at node level.
                        insertIdx = selectedNode.Parent.Nodes.IndexOf(selectedNode);
                        selectedNode.Parent.Nodes.Insert(insertIdx, folderNode);
                    }
                }
            }
            else
            {
                _treeModel.Nodes.Add(folderNode);
            }
        }

        private void removeItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_tree.SelectedNode == null) return;

            FavouritesNode node = _tree.SelectedNode.Tag as FavouritesNode;
            if (node == null) return;

            if (node.IsFolder)
            {
                // Remove path if empty.
                if (_tree.SelectedNode.Children.Count == 0)
                {
                    if (node.Parent == null)
                        _treeModel.Nodes.Remove(node);
                    else
                        node.Parent.Nodes.Remove(node);

                    string path = node.Id;

                    _addedPaths.Remove(path);
                }
            }
            else
            {
                if (node.Item != null)
                {
                    // Standard remove.
                    Remove(node.Item);
                }
                else
                {
                    // No associated item.
                    RemoveInternal(node.Id);
                }
            }
        }


        private void _nodeTextBox_DrawText(object sender, DrawEventArgs e)
        {
            // Grey out ids that are present that no longer have an item.
            FavouritesNode node = (e.Node.Tag as FavouritesNode);
            if (node == null) return;

            if (!node.IsFolder && node.Item == null)
            {
                e.TextColor = Color.Gray;
            }
        }

        private void _tree_NodeMouseClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            //
        }
 
        private void _tree_SelectionChanged(object sender, EventArgs e)
        {
            //
        }

        private void _tree_DoubleClick(object sender, EventArgs e)
        {
            SelectNode();
        }


        private void _tree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            _tree.DoDragDropSelectedNodes(DragDropEffects.Move);
        }

        private bool FoundParent(TreeNodeAdv parent, TreeNodeAdv node)
        {
            while (parent != null)
            {
                if (node == parent)
                    return true;
                else
                    parent = parent.Parent;
            }
            return false;
        }

        private void _tree_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
            if (!e.Data.GetDataPresent(typeof(TreeNodeAdv[]))) return;

            TreeNodeAdv[] nodes = e.Data.GetData(typeof(TreeNodeAdv[])) as TreeNodeAdv[];
            if (nodes.Length == 0) return;

            DropPosition dp = _tree.DropPosition;
            if (dp.Node != null)
            {
                if (dp.Node.Tag is FavouritesNode)
                {
                    if ((dp.Position == NodePosition.Inside && (dp.Node.Tag as FavouritesNode).IsFolder) ||
                        (dp.Position != NodePosition.Inside))
                    {
                        e.Effect = e.AllowedEffect;
                    }
                }
            }
            else
            {
                e.Effect = e.AllowedEffect;
            }
        }

        private void _tree_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(TreeNodeAdv[]))) return;

            TreeNodeAdv[] nodes = e.Data.GetData(typeof(TreeNodeAdv[])) as TreeNodeAdv[];
            if (nodes.Length == 0) return;

            _tree.BeginUpdate();

            DropPosition dp = _tree.DropPosition;
            if (dp.Node != null)
            {
                // We have a drop node to consider.
                if (dp.Node.Tag is FavouritesNode)
                {
                    Node dropNode = _tree.DropPosition.Node.Tag as Node;
                    if (_tree.DropPosition.Position == NodePosition.Inside)
                    {
                        foreach (TreeNodeAdv n in nodes)
                        {
                            if (n.Tag is FavouritesNode)
                            {
                                // Reset the parent of each node we've dropped.
                                (n.Tag as Node).Parent = dropNode;
                            }
                            else if (n.Tag is BaseItem)
                            {
                                // Add a new fave node for each item dropped, with parent.
                                AddWithParent(n.Tag as BaseItem, dropNode, -1);
                            }
                        }
                        _tree.DropPosition.Node.IsExpanded = true;
                    }
                    else
                    {
                        Node parent = dropNode.Parent;
                        Node nextItem = dropNode;
                        if (_tree.DropPosition.Position == NodePosition.After) nextItem = dropNode.NextNode;

                        foreach (TreeNodeAdv n in nodes)
                        {
                            if (n.Tag is FavouritesNode)
                            {
                                (n.Tag as Node).Parent = null;
                            }
                            else if (n.Tag is BaseItem)
                            {
                                //
                            }
                        }

                        int index = parent.Nodes.IndexOf(nextItem);
                        foreach (TreeNodeAdv n in nodes)
                        {
                            if (n.Tag is FavouritesNode)
                            {
                                Node item = n.Tag as Node;
                                if (index == -1)
                                {
                                    parent.Nodes.Add(item);
                                }
                                else
                                {
                                    parent.Nodes.Insert(index, item);
                                    index++;
                                }
                            }
                            else if (n.Tag is BaseItem)
                            {
                                if (index == -1)
                                {
                                    AddWithParent(n.Tag as BaseItem, parent, -1);
                                }
                                else
                                {
                                    AddWithParent(n.Tag as BaseItem, parent, index);
                                    index++;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // We have no drop node.
                foreach (TreeNodeAdv n in nodes)
                {
                    if (n.Tag is FavouritesNode)
                    {
                    }
                    else if (n.Tag is BaseItem)
                    {
                        AddWithParent(n.Tag as BaseItem, null, -1);
                    }
                }
            }

            _tree.EndUpdate();
        }


        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            BaseItem item = null;

            if (_tree.SelectedNode != null)
            {
                // If we have a selected node, find the corresponding item.
                FavouritesNode node = _tree.SelectedNode.Tag as FavouritesNode;
                if (node != null)
                {
                    item = node.Item;
                }
            }

            _windowManager.MenuManager.UpdateContextMenu(sender as ContextMenuStrip, item);
        }
    }

}

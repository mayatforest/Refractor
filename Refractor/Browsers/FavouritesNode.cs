using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Refractor.Common;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

namespace Refractor
{
    internal class FavouritesNode : Node
    {
        public string Id;
        public bool IsFolder;

        private Image _icon;
        public Image Icon { get { return _icon; } set { _icon = value; } }

        private BaseItem _item;
        public BaseItem Item { get { return _item; } set { _item = value; } }

        public FavouritesNode(string text, string id, bool isFolder, BaseItem item)
            : base(text)
        {
            Id = id;
            IsFolder = isFolder;

            _item = item;

            if (item != null)
            {
                _icon = item.Icon;
            }
        }

        public override bool IsLeaf
        {
            get
            {
                return !IsFolder;
            }
        }
    }

    internal class FavouritesToolTipProvider : IToolTipProvider
    {
        public string GetToolTip(TreeNodeAdv node, NodeControl nodeControl)
        {
            return "Drag&Drop nodes to move them";
        }
    }

    [Serializable]
    public class FavouritesItem
    {
        public string Id;
        public string Path;

        public FavouritesItem()
        {
            // Serialization requires default ctor.
        }

        public FavouritesItem(string id, string path)
        {
            Id = id;
            Path = path;
        }
    }

}

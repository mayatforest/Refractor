using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using System.Reflection;
using System.IO;

namespace Refractor.Common
{
	public abstract class BaseItem
	{
        private string _itemPath = string.Empty; 
		public string ItemPath { get { return _itemPath; } set { _itemPath = value; } }

        private Image _icon;
		public Image Icon 
        { 
            get 
            {                 
                if (_icon == null) 
                {
                    try
                    {
                                                
                        _icon = ResourceHelper.GetBitmap(this.GetType().Name);
                    }
                    catch (ArgumentException exc)
                    {
                        exc.ToString();
                    }

                }
                return _icon; 
            } 
            set 
            { 
                _icon = value; 
            } 
        }

		private string _size = string.Empty; 
        public string Size { get { return _size; } set { _size = value; } }

		private string _complexity = "";
		public string Complexity { get { return _complexity; } set { _complexity = value; } }

        private int _count = 0;
        public int Count { get { return _count; } set { _count = value; } }

        private int _total = 0;
        public int Total { get { return _total; } set { _total = value; } }

        private string _kind = string.Empty;
        public string Kind { get { return _kind; } set { _kind = value; } }
                
        private BaseItem _parent; 
        public BaseItem Parent { get { return _parent; } set { _parent = value; } }
        
        public abstract string Name { get; set; }

        public abstract string GetID();

        public override string ToString()
        {
            return Name;
        }

        public virtual string GetShortID()
        {
            string[] bits = GetID().Split(FileSeperator);
            if (bits.Length > 1)
                return bits[1];
            else
                return bits[0];
        }

        public static char FileSeperator = '?';

    }
  
    public class FolderItem : BaseItem
    {
        public FolderItem(string itemPath, BaseItem parent)
        {
            ItemPath = itemPath;
            Parent = parent;
        }

        public override string Name
        {
            get
            {
                // e.g. windows
                return Path.GetFileName(ItemPath);
            }
            set
            {
                ItemPath = Path.Combine(Path.GetDirectoryName(ItemPath), value);
            }
        }

        public override string GetID()
        {
            return ItemPath;
        }
    }

    public class RootItem : BaseItem
    {
        public RootItem()
        {
            ItemPath = "Root";
            Parent = null;
        }

        public override string Name
        {
            get
            {
                return ItemPath;
            }
            set
            {
                ItemPath = value;
            }
        }

        public override string GetID()
        {
            return ItemPath;
        }

        public List<FileItem> Files = new List<FileItem>();
    }


	public class FileItem : BaseItem
	{
        public FileItem(string itemPath, BaseItem parent)
        {
            ItemPath = itemPath;
            Parent = parent;
        }
        
        public override string Name
		{
			get
			{
                // e.g. setup.dll
				return Path.GetFileName(ItemPath);
			}
            set
            {
                ItemPath = Path.Combine(Path.GetDirectoryName(ItemPath), value);
            }
        }

        public override string GetID()
        {
            return ItemPath;
        }

        public string PureName()
        {
            return Path.GetFileNameWithoutExtension(ItemPath);
        }
    }


}

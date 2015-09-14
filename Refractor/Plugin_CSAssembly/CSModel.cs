using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

using Refractor.Common;


namespace Refractor.Plugins
{
    public class AssemblyItem : FileItem
    {
        public AssemblyItem(string itemPath, BaseItem parent)
            : base(itemPath, parent)
        {
        }

        public List<NamespaceItem> NameSpaces = new List<NamespaceItem>();
        public List<string> ReferencedAssemblies = new List<string>();
        public Assembly AsmRef;
        public bool Loaded = false;

        public override string GetID()
        {
            return ItemPath; 
        }
        //
        public string GetShortPath()
        {
            String filename=Path.GetFileName(ItemPath);
            return filename;
        }

        public int GetCount()
        {
            return NameSpaces.Count;
        }

    }


    public class NamespaceItem : BaseItem
    {
        public override string Name { get { return _Name; } set { _Name = value; } }

        public NamespaceItem(string itemPath, BaseItem parent)
        {
            ItemPath = itemPath;
            Parent = parent;
            _Name = itemPath;
        }

        private string _Name = string.Empty;

        public override string GetID()
        {
            return Parent.GetID() + "?" + _Name;
        }

        public int GetCount()
        {
            return Types.Count;
        }

        public List<TypeItem> Types = new List<TypeItem>();
    }


    public class TypeItem : BaseItem
    {
        private string _Name = string.Empty;
        public override string Name { get { return _Name; } set { _Name = value; } }

        private string _FullName = string.Empty;
        public string FullName { get { return _FullName; } set { _FullName = value; } }

        public TypeItem(string itemPath, string fullName, Type t, BaseItem parent)
        {
            ItemPath = itemPath;
            Parent = parent;
            _Name = itemPath;
            _FullName = fullName;
            TypeRef = t;
        }

        public Type TypeRef;

        public List<MethodItem> Methods = new List<MethodItem>();

        // These may hold references to items not present in the project.
        public List<string> Implements;

        public string InheritsFrom;


        public override string GetID()
        {
            return Parent.GetID() + "." + _Name;
        }

        public string GetParentAssemblyID()
        {
            BaseItem Parentfor = Parent;
            while (Parentfor != null)
            {
                if (Parentfor is AssemblyItem)
                {
                    AssemblyItem ai=(AssemblyItem)Parentfor;
                    return ai.GetShortPath();
                }
                Parentfor = Parentfor.Parent;
            }
            return "";
        }


        public int GetCount()
        {
            return Methods.Count;
        }
    }

    public class MethodItem : BaseItem
    {
        private string _Name = string.Empty;
        public override string Name { get { return _Name; } set { _Name = value; } }

        public MethodItem(string name, MethodInfo mi, BaseItem parent)
        {
            ItemPath = name;     //?
            Parent = parent;
            _Name = name;
            MethodInfo = mi;
        }

        private MethodInfo _methodInfo;
        public MethodInfo MethodInfo { get { return _methodInfo; } set { _methodInfo = value; } }

        public override string GetID()
        {
            return Parent.GetID() + " " + RefHelp.GetNameWithParameterList(_methodInfo);
        }

        public int GetCount()
        {
            return 0;
        }
    }


}

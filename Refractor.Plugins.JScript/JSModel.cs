using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Refractor.Common;
using Refractor.Plugins;
using Microsoft.JScript; // Mono implementation.

namespace Refractor.Plugins.JScript
{
    public class JSFileItem : FileItem
    {
        public JSFileItem(string itemPath, BaseItem parent)
            : base(itemPath, parent)
        {
        }

        public List<JSClassItem> Classes = new List<JSClassItem>();
        public List<JSMethodItem> Methods = new List<JSMethodItem>();

        //public bool Loaded = false;
        public ScriptBlock Block;

        public int GetCount()
        {
            return Classes.Count;
        }
    }

    public class JSClassItem : BaseItem
    {
        private string _Name = string.Empty;
        public override string Name { get { return _Name; } set { _Name = value; } }

        private string _FullName = string.Empty;
        public string FullName { get { return _FullName; } set { _FullName = value; } }

        public JSClassItem(string itemPath, string fullName, BaseItem parent)
        {
            ItemPath = itemPath;     //?
            Parent = parent;
            _Name = itemPath;
            _FullName = fullName;
        }

        public List<JSMethodItem> Methods = new List<JSMethodItem>();

        public override string GetID()
        {
            return Parent.GetID() + "." + _Name;
        }
    }

    public class JSMethodItem : BaseItem
    {
        private string _Name = string.Empty;
        public override string Name { get { return _Name; } set { _Name = value; } }

        private AST _MethodInfo;
        public AST MethodInfo { get { return _MethodInfo; } set { _MethodInfo = value; } }

        public JSMethodItem(string name, BaseItem parent)
        {
            ItemPath = name;     //?
            Parent = parent;
            _Name = name;
        }

        public override string GetID()
        {
            return Parent.GetID() + " " + _Name; // params? todo RefHelp.GetNameWithParameterList(_methodInfo);
        }

        public int GetCount()
        {
            return 0;
        }
    }
}

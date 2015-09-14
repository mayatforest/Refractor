using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;

using Refractor.Common;
using Microsoft.JScript; // Mono implementation of.

namespace Refractor.Plugins.JScript
{
    [Serializable]
    public class JSParserOptions : PluginOptions
    {
        public JSParserOptions(string id) : base(id) { }
        public JSParserOptions() { }

        private Color _fileColor = Color.LightBlue;
        public Color FileColor { get { return _fileColor; } set { _fileColor = value; } }

        private Color _classColor = Color.Green;
        public Color ClassColor { get { return _classColor; } set { _classColor = value; } }

        private Color _methodColor = Color.White;
        public Color MethodColor { get { return _methodColor; } set { _methodColor = value; } }
    }
    
    public class JSParserPlugin : IParserPlugin
    {
        public JSParserPlugin()
        {
            _parser = new Parser();

            ResourceHelper.CheckAssembly(Assembly.GetExecutingAssembly());
        }

        public string GetID()
        {
            return "JS Parser Plugin";
        }

        public List<string> HandlesExtensions()
        {
            return new List<string>() { ".js" };
        }

        public List<Type> HandlesItems()
        {
            return new List<Type>() { 
                typeof(JSFileItem), 
                typeof(JSClassItem), 
                typeof(JSMethodItem)
            };
        }


        public BaseItem GetFileItem(string filename)
        {
            // A factory method to return a file item for a specific file,
            // for this plugin, which will be held as a reference in the UI.
            JSFileItem item = new JSFileItem(filename, null);

            return item;
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _projectBrowser = (IProjectBrowser)serviceProvider.GetService(typeof(IProjectBrowser));
            _logView = (ILogView)serviceProvider.GetService(typeof(ILogView));
        }

        public bool ReadItem(BaseItem item, bool unload)
        {
            lock (_lockObj)
            {
                JSFileItem jsFileItem = (JSFileItem)item;

                ReadJSFileItem(jsFileItem, unload);
            }
            return true;
        }

        public bool FindChildren(BaseItem parent, List<BaseItem> items)
        {
            bool result = true;
           
            if (parent is JSFileItem)
            {
                foreach (BaseItem item in (parent as JSFileItem).Classes)
                    items.Add(item);
                foreach (BaseItem item in (parent as JSFileItem).Methods)
                    items.Add(item);
            }
            else if (parent is JSClassItem)
            {
                foreach (BaseItem item in (parent as JSClassItem).Methods)
                    items.Add(item);
            }
            else result = false;

            return result;
        }

        public bool IsLeaf(BaseItem item)
        {
            return (item is JSMethodItem);
        }

        public void CalcMetrics(BaseItem item)
        {
            if (item is JSFileItem)
            {
                int count = 0;

                foreach (JSClassItem classItem in (item as JSFileItem).Classes)
                {
                    count += classItem.Methods.Count;
                }

                item.Size = count.ToString();
                item.Complexity = ((item as JSFileItem).Classes.Count * count).ToString();

            }
            else if (item is JSClassItem)
            {
                item.Size = ((item as JSClassItem).Methods.Count).ToString();
                item.Complexity = "0";
            }
        }

        public PluginOptions GetOptions()
        {
            if (_options == null)
            {
                _options = new JSParserOptions(this.GetID());
            }

            return _options;
        }

        public void SetOptions(PluginOptions options)
        {
            if (options == null) return;
            if (!(options is JSParserOptions)) return;

            _options = options as JSParserOptions;
        }

        //public List<Type> GetSerializationTypes()
        //{
        //    return new List<Type>() { typeof(JSParserOptions) };
        //}

        private IProjectBrowser _projectBrowser;
        private ILogView _logView;
        private Parser _parser;
        private Hashtable _classLookup = new Hashtable();
        private JSFileItem _fileItem;
        private bool _invert;
        private object _lockObj = new object();
        private JSParserOptions _options;


        private void ReadJSFileItem(JSFileItem item, bool unload)
        {
            try
            {
                // Check if we've already read this file.
                if (item.Block != null)
                {
                    // Clear the caches.
                    item.Classes.Clear();
                    item.Methods.Clear();
                    _classLookup.Clear();
                    _currentMethodStack.Clear();

                    // Remove structure from the IProjectBrowser lookup dictionary.
                    _invert = true;
                    ParseAstList(item.Block.src_elems.elems, string.Empty);
                }

                if (!unload)
                {
                    //todo common
                    // Clear the caches.
                    item.Classes.Clear();
                    item.Methods.Clear();
                    _classLookup.Clear();
                    _currentMethodStack.Clear();

                    // Create the AST using our JS parser.
                    string filename = item.ItemPath;
                    string text = File.ReadAllText(filename);
                    ScriptBlock block = (ScriptBlock)_parser.Parse(text, filename, 0);

                    // Add structure to the IProjectBrowser lookup dictionary.
                    _fileItem = item;
                    _invert = false;
                    ParseAstList(block.src_elems.elems, string.Empty);

                    _fileItem.Block = block;
                }
            }
            catch (Exception exc)
            {
                _logView.LogExcStr(exc, "ReadJSFileItem failed :" + item.ItemPath);
            }
        }


        private List<string> _currentMethodStack = new List<string>();

        private void AddMethod(string className, string methodName, Assign ass)
        {
            JSClassItem classItem = _classLookup[className] as JSClassItem;
            if (classItem == null)
            {
                // We haven't defined this type yet, do so here.
                classItem = new JSClassItem(className, className, _fileItem);
                //classItem.Icon = Properties.Resources.JSClassItem;
                _fileItem.Classes.Add(classItem);

                _classLookup.Add(className, classItem);
                _projectBrowser.AddLookup(classItem, classItem.GetID(), _invert);
            }

// TODO methods need parent method ids in their ids, e.g. Hierarchy.js is breaking

            JSMethodItem methodItem = new JSMethodItem(methodName + "()", classItem);
            //methodItem.Icon = Properties.Resources.JSMethodItem;
            methodItem.MethodInfo = ass.right;
            classItem.Methods.Add(methodItem);
            _projectBrowser.AddLookup(methodItem, methodItem.GetID(), _invert);
        }


        private void ParseAstList(ArrayList astItems, string sp)
        {
            foreach (AST ast in astItems)
            {
                ParseAst(ast, sp);
            }
        }

        private void ParseAst(AST ast, string sp)
        {
            if (ast == null) return;

            //_logView.LogStr("JS->" + sp + ast.ToString() + "\t\t" + ast.GetType().Name);

            if (ast is Function)
            {
                Function func = ast as Function;

                JSMethodItem methodItem = new JSMethodItem(func.func_obj.name + "()", _fileItem);
                //methodItem.Icon = Properties.Resources.JSMethodItem;
                methodItem.MethodInfo = ast;
                
                _fileItem.Methods.Add(methodItem);
                _projectBrowser.AddLookup(methodItem, methodItem.GetID(), _invert);

                _currentMethodStack.Insert(0, func.func_obj.name);

                ParseAstList(func.func_obj.body.elems, sp + "  ");

                _currentMethodStack.RemoveAt(0);
            }
            else if (ast is Assign)
            {
                Assign ass = ast as Assign;

                if ((ass.left is Binary) && (ass.right is FunctionExpression))
                {
                    // Can't see useful properties on this, except ToString().
                    string[] parts = ass.left.ToString().Split('.');
                    if (parts.Length > 2 && parts[1] == "prototype")
                    {
                        // We've found a class method, prototype style.
                        string className = parts[0];
                        string methodName = parts[2];

                        AddMethod(className, methodName, ass);
                    }
                    else if (parts.Length > 2 && parts[2] == "This")
                    {
                        string className = _currentMethodStack[0];
                        string methodName = parts[3];

                        AddMethod(className, methodName, ass);
                    }

                }
            }
            else if (ast is Expression)
            {
                Expression expr = ast as Expression;
                ParseAstList(expr.exprs, sp + "  ");
            }
            else if (ast is FunctionExpression)
            {
            }
            else if (ast is If)
            {
                If expr = ast as If;
                ParseAst(expr.true_stm, sp + "t  ");
                ParseAst(expr.false_stm, sp + "f  ");
            }
            else if (ast is Block)
            {
                Block block = ast as Block;
                ParseAstList(block.elems, sp + "  ");
            }
        }

    }

}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Reflection;

using Refractor.Common;
using SDILReader;

namespace Refractor.Plugins
{
    public class CSParserPlugin : IParserPlugin
    {
        public CSParserPlugin()
        {
            if (_loadByteCopy)
            {
                // http://www.devcity.net/Articles/254/1/article.aspx
                AppDomain.CurrentDomain.AssemblyResolve +=
                    new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            }
        }

        public string GetID()
        {
            return "IL Parser Plugin";
        }

        public List<string> HandlesExtensions()
        {
            return new List<string>() { ".exe", ".dll" };
        }

        public List<Type> HandlesItems()
        {
            return new List<Type>() { 
                typeof(AssemblyItem), 
                typeof(NamespaceItem), 
                typeof(TypeItem), 
                typeof(MethodItem)
            };
        }
        
        public BaseItem GetFileItem(string name)
        {
            AssemblyItem item = new AssemblyItem(name, null);
            return item;
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _projectBrowser = (IProjectBrowser)serviceProvider.GetService(typeof(IProjectBrowser));
            _logView = (ILogView)serviceProvider.GetService(typeof(ILogView));
        }

        public bool ReadItem(BaseItem item, bool unload)
        {
            if (!DotNetObject.isDotNetAssembly(item.ItemPath)) return false;
            
            AssemblyItem assemblyItem = (AssemblyItem)item;

            if (assemblyItem.Loaded == true)
            {
                // Run over the same assembly that was loaded, removing the items.
                ReflectAssembly(assemblyItem, assemblyItem.AsmRef, true);

                assemblyItem.NameSpaces.Clear();
            }

            if (!unload)
            {
                ReadAssemblyItem(assemblyItem);
                assemblyItem.Loaded = true;
            }

            return true;
        }

        public bool FindChildren(BaseItem parent, List<BaseItem> items)
        {
            bool result = true;
           
            if (parent is AssemblyItem)
            {
                foreach (BaseItem item in (parent as AssemblyItem).NameSpaces)
                    items.Add(item);
            }
            else if (parent is NamespaceItem)
            {
                foreach (BaseItem item in (parent as NamespaceItem).Types)
                    items.Add(item);
            }
            else if (parent is TypeItem)
            {
                foreach (BaseItem item in (parent as TypeItem).Methods)
                    items.Add(item);
            }
            else result = false;

            return result;
        }

        public bool IsLeaf(BaseItem item)
        {
            return (item is MethodItem);
        }

        public void CalcMetrics(BaseItem item)
        {
            if (item is AssemblyItem)
            {
                item.Count = (item as AssemblyItem).NameSpaces.Count;
                RippleCount(item);
            }
            else if (item is NamespaceItem)
            {
                item.Count = (item as NamespaceItem).Types.Count;
                RippleCount(item);
            }
            else if (item is TypeItem)
            {
                item.Count = (item as TypeItem).Methods.Count;
                RippleCount(item);

                Type t = (item as TypeItem).TypeRef;

                string s = string.Empty;
                if (t.IsClass) s += "/Class";
                if (t.IsInterface) s += "/Interface";
                if (t.IsValueType) s += "/IsValueType";
                if (t.IsSerializable) s += "/IsSerializable";
                if (t.IsCOMObject) s += "/COMObject";
                if (t.IsSealed) s += "/IsSealed";
                if (t.IsContextful) s += "/Contextful";
                if (t.IsEnum) s += "/Enum";
                if (t.IsExplicitLayout) s += "/ExplicitLayout";
                if (t.IsGenericParameter) s += "/GenericParameter";
                if (t.IsGenericType) s += "/GenericType";
                if (t.IsGenericTypeDefinition) s += "/GenericTypeDefinition";
                if (t.IsImport) s += "/Import";
                if (t.IsLayoutSequential) s += "/LayoutSequential";
                if (t.IsMarshalByRef) s += "/MarshalByRef";
                if (t.IsNested) s += "/Nested";
                if (t.IsNestedAssembly) s += "/NestedAssembly";
                if (t.IsNestedFamANDAssem) s += "/NestedFamANDAssem";
                if (t.IsNestedFamily) s += "/NestedFamily";
                if (t.IsNestedFamORAssem) s += "/NestedFamORAssem";
                if (t.IsNestedPrivate) s += "/NestedPrivate";
                if (t.IsNestedPublic) s += "/NestedPublic";
                if (t.IsNotPublic) s += "/IsNotPublic";
                if (t.IsPointer) s += "/IsPointer";
                if (t.IsPrimitive) s += "/IsPrimitive";
                if (t.IsPublic) s += "/IsPublic";
                if (t.IsSpecialName) s += "/IsSpecialName";
                if (t.IsUnicodeClass) s += "/IsUnicodeClass";
                if (t.IsVisible) s += "/IsVisible";
                item.Kind = s.Trim('/');
            }
            else if (item is MethodItem)
            {
                item.Count = 0;
                MethodInfo mi = (item as MethodItem).MethodInfo;

                MethodBody mb = mi.GetMethodBody();
                if (mb != null)
                {
                    item.Count = mb.GetILAsByteArray().Length;
                    RippleCount(item);
                }

                string s = string.Empty;
                if (mi.IsStatic) s += "/Static";
                if (mi.IsAbstract) s += "/IsAbstract";
                if (mi.IsConstructor) s += "/IsConstructor";
                if (mi.IsAssembly) s += "/IsAssembly";
                if (mi.IsFamily) s += "/IsFamily";
                if (mi.IsFamilyAndAssembly) s += "/IsFamilyAndAssembly";
                if (mi.IsFamilyOrAssembly) s += "/IsFamilyOrAssembly";
                if (mi.IsFinal) s += "/IsFinal";
                if (mi.IsGenericMethod) s += "/IsGenericMethod";
                if (mi.IsGenericMethodDefinition) s += "/IsGenericMethodDefinition";
                if (mi.IsHideBySig) s += "/IsHideBySig";
                if (mi.IsSpecialName) s += "/IsSpecialName";
                if (mi.IsPrivate) s += "/IsPrivate";
                if (mi.IsPublic) s += "/IsPublic";
                if (mi.IsVirtual) s += "/IsVirtual";
                item.Kind = s.Trim('/');
            }

        }

        public PluginOptions GetOptions()
        {
            if (_options == null)
            {
                _options = new CSParserOptions(this.GetID());
            }

            return _options;
        }

        public void SetOptions(PluginOptions options)
        {
            if (options == null) return;
            if (!(options is CSParserOptions)) return;

            _options = options as CSParserOptions;
        }

       
        private IProjectBrowser _projectBrowser;
        private ILogView _logView;
        private string _asmSearchPath = string.Empty;        
        private bool _loadByteCopy = true;
        private List<string> _asmWarnedAlready = new List<string>();
        private List<string> _miWarnedAlready = new List<string>();
        private CSParserOptions _options;

        private void RippleCount(BaseItem item)
        {
            item.Total += item.Count;
            BaseItem parent = item.Parent;
            while (parent != null)
            {
                if (!(parent is AssemblyItem || parent is NamespaceItem ||
                    parent is TypeItem || parent is MethodItem)) break;

                parent.Total = parent.Total + item.Count;
                parent = parent.Parent;
            }
        }


        private void ReadAssemblyItem(AssemblyItem assemblyItem)
        {
            // Create NamesSpace, Type, and method nodes all here, once,
            // so they'll be there when the tree needs them.
            try
            {
                Assembly asm = LoadAssembly(assemblyItem.ItemPath);

                assemblyItem.AsmRef = asm;

                // Fill Referenced Assemblies list.
                AssemblyName[] referenced = asm.GetReferencedAssemblies();
                foreach (AssemblyName an in referenced)
                {
                    assemblyItem.ReferencedAssemblies.Add(an.Name); //
                }

                ReflectAssembly(assemblyItem, asm, false);

            }
            catch (Exception exc)
            {
                _logView.LogExcStr(exc, "ReadAssemblyItem failed (possible missing referenced assembly):" + assemblyItem.ItemPath);
            }
        }

        private void ReflectAssembly(AssemblyItem assemblyItem, Assembly asm, bool invert)
        {
            string ns;
            NamespaceItem nsItem;
            Dictionary<string, NamespaceItem> nsItems = new Dictionary<string, NamespaceItem>();

            // Add the assembly item itself to the lookup.
            _projectBrowser.AddLookup(assemblyItem, assemblyItem.GetID(), invert);

            try
            {
                Type[] types = asm.GetTypes();
            }
            catch (ReflectionTypeLoadException exc)
            {
                _logView.LogExcStr(exc, "Failed to call GetTypes on " + asm.FullName);
                return;
            }

            foreach (Type t in asm.GetTypes())
            {
                ns = t.Namespace;
                if (string.IsNullOrEmpty(ns)) ns = "(none)";

                if (!nsItems.ContainsKey(ns))
                {
                    nsItem = new NamespaceItem(ns, assemblyItem);
                    nsItems.Add(ns, nsItem);
                    assemblyItem.NameSpaces.Add(nsItem);
                    _projectBrowser.AddLookup(nsItem, nsItem.GetID(), invert);
                }
                else
                {
                    nsItem = nsItems[ns];
                }

                // Flatten nested types.
                string name = t.Name;
                if (t.IsNested)
                {
                    // Flat with .id'd name.
                    Type parentType = t.DeclaringType;
                    while (parentType != null)
                    {
                        name = parentType.Name + "." + name;
                        parentType = parentType.DeclaringType;
                    }
                }

                try
                {
                    TypeItem typeItem = new TypeItem(name, t.FullName, t, nsItem);

                    nsItem.Types.Add(typeItem);
                    _projectBrowser.AddLookup(typeItem, typeItem.GetID(), invert);

                    foreach (MethodInfo mi in GetMethods(typeItem))
                    {
                        MethodItem methodItem = new MethodItem(mi.Name, mi, typeItem);
                        typeItem.Methods.Add(methodItem);

                        _projectBrowser.AddLookup(methodItem, methodItem.GetID(), invert);
                    }

                    // Get the list of implemented interfaces.
                    typeItem.Implements = GetImplementedInterfaces(typeItem);

                    if (typeItem.TypeRef.BaseType != null)
                    {
                        typeItem.InheritsFrom = typeItem.TypeRef.BaseType.ToString();
                    }
                    else
                    {
                        typeItem.InheritsFrom = null;
                    }
                }
                catch (Exception exc)
                {
                    _logView.LogExcStr(exc, "Generic Types issue?");
                }

            }
        }


        private Assembly LoadAssembly(string path)
        {
            Assembly asm;

            if (_loadByteCopy)
            {
                _asmSearchPath = Path.GetDirectoryName(path);

                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    byte[] bytes = new byte[4096];
                    while (fileStream.Read(bytes, 0, bytes.Length) > 0)
                        memoryStream.Write(bytes, 0, bytes.Length);
                    asm = Assembly.Load(memoryStream.ToArray());
                }
                return asm;
            }
            else
            {
                asm = Assembly.LoadFrom(path);
            }

            return asm;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // For bytecopy assembly load technique.
            // Ensure we find referenced assemblies, without which GetTypes() fails.

            string[] asmName = args.Name.Split(',');
            string asmPath = Path.Combine(_asmSearchPath, asmName[0] + ".dll");

            if (!File.Exists(asmPath))
            {
                if (!_asmWarnedAlready.Contains(asmPath))
                {
                    _logView.LogStr("AssemblyResolve didn't find : " + asmPath);
                    _asmWarnedAlready.Add(asmPath);
                }

                return null;
            }
            

            // Make sure we load referenced assemblies using using bytecopy.
            // We're not interested in these assemblies unless they're in the project,
            // but they still need to be loaded to reflect over the assembly contents.
            return LoadAssembly(asmPath);
        }

        private MethodInfo[] GetMethods(TypeItem typeItem)
        {
            // better way to sort.. IComparable? todo
            Dictionary<string, MethodInfo> d = new Dictionary<string, MethodInfo>();
            List<string> l = new List<string>();

            string id;
            foreach (MethodInfo mi in typeItem.TypeRef.GetMethods(
                    BindingFlags.NonPublic | BindingFlags.Public |
                    BindingFlags.Instance | BindingFlags.Static |
                    BindingFlags.DeclaredOnly ))
            {
                if (mi.IsSpecialName) continue;

                try
                {
                    id = RefHelp.GetNameWithParameterList(mi);
                }
                catch (FileNotFoundException exc)
                {
                    if (!_miWarnedAlready.Contains(typeItem.FullName))
                    {
                        _logView.LogExcStr(exc, "Failed to get methods for " + typeItem.FullName);
                        _miWarnedAlready.Add(typeItem.FullName);
                    }
                    continue;
                }

                d.Add(id, mi);
                l.Add(id);
            }
            l.Sort();

            MethodInfo[] result = new MethodInfo[l.Count];

            for (int i = 0; i < l.Count; i++)
                result[i] = d[l[i]];

            return result;
        }



        // http://msdn2.microsoft.com/en-us/library/system.type.findinterfaces.aspx
        private static List<string> GetImplementedInterfaces(TypeItem typeItem)
        {
            List<string> result = new List<string>();

            try
            {
                TypeFilter myFilter = new TypeFilter(MyInterfaceFilter);
                Type[] myInterfaces = typeItem.TypeRef.FindInterfaces(myFilter, null);
                for (int i = 0; i < myInterfaces.Length; i++)
                {
                    result.Add(myInterfaces[i].ToString());
                }
            }
            catch (ArgumentNullException)
            {
                //Console.WriteLine("ArgumentNullException: " + e.Message);
            }
            catch (TargetInvocationException)
            {
                //Console.WriteLine("TargetInvocationException: " + e.Message);
            }

            return result;
        }

        public static bool MyInterfaceFilter(Type typeObj, Object criteriaObj)
        {
            return true;
        }


    
    }
}

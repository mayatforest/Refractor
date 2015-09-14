using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using Refractor.Common;
using WeifenLuo.WinFormsUI.Docking;

namespace Refractor
{
    internal class PluginManager
    {        
        internal Dictionary<string, List<IParserPlugin>> ParserPluginsByExt { get { return _extensions; } }

        internal PluginManager(IServiceProvider serviceProvider)
        {
            _windowManager = (WindowManager)serviceProvider.GetService(typeof(WindowManager));
        }

        internal void LoadPlugins()
        {
            // Files named as Refractor.*.dll on the exe path will be 
            // searched for plugin implementations which sublass DockContent.
            string path = Path.GetDirectoryName(Application.ExecutablePath);
            string[] filenames = Directory.GetFiles(path, "Refractor.*.dll",
                SearchOption.TopDirectoryOnly);

            _allNames.Clear();
            foreach (string filename in filenames)
            {
                LoadAllPluginsFromFile(filename);
            }

            // Also check this assembly for self hosted plugins.
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                CheckLoadPluginType("Refractor.exe", t);
            }
        }

        internal void SetAllOptions(List<PluginOptions> list, List<PluginOptions> listParserOptions)
        {
            // Set diagram plugin options.
            foreach (PluginOptions options in list)
            {
                string id = options.PluginId;
                
                try
                {
                    if (_windowPlugins.ContainsKey(id))
                    {
                        _windowPlugins[id].SetOptions(options);
                    }
                    else _windowManager.Logger.LogStr("Warning : Plugin options available but no plugin found : " + id);
                }
                catch (Exception exc)
                {
                    if (_windowManager.Logger.LogCatchAll(exc, "Failed setting plugin options :" + id)) throw;
                }
            }

            // Set parser plugin options.
            foreach (PluginOptions options in listParserOptions)
            {
                string id = options.PluginId;

                try
                {
                    if (_parserPlugins.ContainsKey(id))
                    {
                        _parserPlugins[id].SetOptions(options);
                    }
                    else _windowManager.Logger.LogStr("Warning : Plugin options available but no plugin found : " + id);
                }
                catch (Exception exc)
                {
                    if (_windowManager.Logger.LogCatchAll(exc, "Failed setting plugin options :" + id)) throw;
                }
            }
        }

        internal void GetAllOptions(List<PluginOptions> result, List<PluginOptions> resultParserOptions)
        {
            if (result == null) 
            { 
                _windowManager.Logger.LogStr("Error : empty options list"); 
                return; 
            }

            // Get options from plugins. The plugins give us a specialized options class.
            result.Clear();
            foreach (KeyValuePair<string, IWindowPlugin> pair in _windowPlugins)
            {
                string id = string.Empty;
                try
                {
                    id = pair.Value.GetID();
                    PluginOptions options = pair.Value.GetOptions();

                    if (options != null)
                    {
                        options.PluginId = id;
                        result.Add(options);
                    }
                }
                catch (Exception exc)
                {
                    if (_windowManager.Logger.LogCatchAll(exc, "Failed getting plugin options :" + id)) throw;
                }
            }

            resultParserOptions.Clear();
            foreach (KeyValuePair<string, IParserPlugin> pair in _parserPlugins)
            {
                string id = string.Empty;
                try
                {
                    id = pair.Value.GetID();
                    PluginOptions options = pair.Value.GetOptions();

                    if (options != null)
                    {
                        options.PluginId = id;
                        resultParserOptions.Add(options);
                    }
                }
                catch (Exception exc)
                {
                    if (_windowManager.Logger.LogCatchAll(exc, "Failed getting parser plugin options :" + id)) throw;
                }
            }
        }

        internal void ClearAllOptions()
        {
            foreach (KeyValuePair<string, IWindowPlugin> pair in _windowPlugins)
            {
                string id = string.Empty;
                try
                {
                    if (pair.Value != null)
                    {
                        id = pair.Value.GetID();
                        pair.Value.SetOptions(null);
                    }
                }
                catch (Exception exc)
                {
                    if (_windowManager.Logger.LogCatchAll(exc, "Failed clearing plugin options :" + id)) throw;
                }
            }

            foreach (KeyValuePair<string, IParserPlugin> pair in _parserPlugins)
            {
                string id = string.Empty;
                try
                {
                    if (pair.Value != null)
                    {
                        id = pair.Value.GetID();
                        pair.Value.SetOptions(null);
                    }
                }
                catch (Exception exc)
                {
                    if (_windowManager.Logger.LogCatchAll(exc, "Failed clearing parser plugin options :" + id)) throw;
                }
            }
        }


        internal PluginOptions GetPluginOptions(string pluginID)
        {
            PluginOptions result = null;

            if (_parserPlugins.ContainsKey(pluginID))
            {
                result = _parserPlugins[pluginID].GetOptions();
            }
            else if (_windowPlugins.ContainsKey(pluginID))
            {
                result = _windowPlugins[pluginID].GetOptions();
            }

            return result;
        }

        internal List<IParserPlugin> ParserPlugins
        {
            get
            {
                List<IParserPlugin> result = new List<IParserPlugin>();
                foreach (KeyValuePair<string, IParserPlugin> pair in _parserPlugins)
                {
                    result.Add(pair.Value);
                }
                return result;
            }
        }

        internal List<IWindowPlugin> WindowPlugins
        {
            get
            {
                List<IWindowPlugin> result = new List<IWindowPlugin>();
                foreach (KeyValuePair<string, IWindowPlugin> pair in _windowPlugins)
                {
                    result.Add(pair.Value);
                }
                return result;
            }
        }
        
        private WindowManager _windowManager;
        private Dictionary<string, List<IParserPlugin>> _extensions = new Dictionary<string, List<IParserPlugin>>();
        private List<string> _allNames = new List<string>();
        private Dictionary<string, IWindowPlugin> _windowPlugins = new Dictionary<string, IWindowPlugin>();
        private Dictionary<string, IParserPlugin> _parserPlugins = new Dictionary<string, IParserPlugin>();

        private void LoadAllPluginsFromFile(string filename)
        {
            try
            {
                // We don't mind locking these assemblies.
                // However, using LoadFrom() doesn't see to work so well with debugging?
                Assembly asm = Assembly.LoadFile(filename);
                
                foreach (Type t in asm.GetTypes())
                {
                    CheckLoadPluginType(filename, t);
                }
            }
            catch (Exception exc)
            {
                if (_windowManager.Logger.LogCatchAll(exc, string.Format("Failed loading plugin from {0}", 
                    filename))) throw;
            }
        }

        private void CheckLoadPluginType(string filename, Type t)
        {
            try
            {
                if ((t.GetInterface("IWindowPlugin") != null) &&
                    (t.IsSubclassOf(typeof(DockContent))))
                {
                    if (!_allNames.Contains(t.Name.ToLower()))
                    {
                        LoadWindowPlugin(filename, t);
                        _allNames.Add(t.Name.ToLower());
                    }
                    else
                    {
                        _windowManager.Logger.LogStr(string.Format(
                            "Warning : Ignored duplicate named window plugin {0}", t.Name));
                    }
                }
                else if (t.GetInterface("IParserPlugin") != null)
                {
                    if (!_allNames.Contains(t.Name.ToLower()))
                    {
                        LoadParserPlugin(filename, t);
                        _allNames.Add(t.Name.ToLower());
                    }
                    else
                    {
                        _windowManager.Logger.LogStr(string.Format(
                            "Warning : Ignored duplicate named parser plugin {0}", t.Name));
                    }
                }
            }
            catch (Exception exc)
            { 
                if (_windowManager.Logger.LogCatchAll(exc, string.Format(
                    "Failed attempting to load '{0}' plugin from {1}",
                    t.ToString(), filename))) throw;
            }
        }

        private void LoadParserPlugin(string filename, Type t)
        {
            try
            {
                IParserPlugin plugin = (IParserPlugin)Activator.CreateInstance(t);
                plugin.SetServiceProvider(_windowManager);

                // Ignore classes with blank ID, for example a base class.
                if (!string.IsNullOrEmpty(plugin.GetID()))
                {
                    _parserPlugins.Add(plugin.GetID(), plugin);

                    List<string> extensions = plugin.HandlesExtensions();

                    // Set up the parser plugin extension list.
                    foreach (string ext in extensions)
                    {
                        if (_extensions.ContainsKey(ext))
                        {
                            _extensions[ext].Add(plugin);
                        }
                        else
                        {
                            List<IParserPlugin> list = new List<IParserPlugin>();
                            list.Add(plugin);
                            _extensions.Add(ext, list);
                        }
                    }

                    _windowManager.Logger.LogStr(string.Format(
                        "Loaded Parser Plugin '{0}'", t.ToString()));
                }
                else _windowManager.Logger.LogStr("Ignored (blank id)");

            }
            catch (Exception exc)
            {
                if (_windowManager.Logger.LogCatchAll(exc, string.Format(
                    "Unexpected error in plugin '{0}'", t.ToString()))) throw;
            }
        }

        private void LoadWindowPlugin(string filename, Type t)
        {
            try
            {
                IWindowPlugin plugin = (IWindowPlugin)Activator.CreateInstance(t);
                plugin.SetServiceProvider(_windowManager);

                string id = plugin.GetID();

                // Ignore classes with blank ID, for example a base class.
                if (!string.IsNullOrEmpty(id))
                {
                    _windowPlugins.Add(id, plugin);
                    _windowManager.AddPlugin(id, plugin as DockContent, true);

                    // If kind is MainWindow, then we have a main menu option, otherwise
                    // the project browser will build a context sensitive right click menu.
                    // Whatever, the menu option for the plugin will hold the reference to 
                    // that plugin, and duplicates should not matter.
                    if (plugin.GetKind() == WindowPluginKind.MainWindow)
                    {
                        _windowManager.MenuManager.AddMenuOption(
                            plugin,
                            "Window",
                            plugin.GetID());
                    }
                    else
                    {
                        _windowManager.MenuManager.AddContextMenuOption(
                            plugin,
                            plugin.GetHandledTypes(),
                            plugin.GetID());
                    }

                    _windowManager.Logger.LogStr(string.Format(
                        "Loaded UI Plugin '{0}'", t.Name));
                }
            }
            catch (Exception exc)
            {
                if (_windowManager.Logger.LogCatchAll(exc, string.Format(
                    "Unexpected error in plugin '{0}'", t.ToString()))) throw;
            }
        }
    }
}

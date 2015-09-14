using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Refractor.Common;
using WeifenLuo.WinFormsUI.Docking;

namespace Refractor
{
    internal class MenuInfo
    {
        public IWindowPlugin Plugin;
        public BaseItem Item;
        
        public MenuInfo(IWindowPlugin plugin, BaseItem item)
        {
            Plugin = plugin;
            Item = item;
        }
    }

    internal class MenuManager : IMenuManager
    {
        public void AddMenuOption(IWindowPlugin plugin, string mainMenuText, string subMenuText)
        {
            ToolStripMenuItem item = null;
            foreach (ToolStripMenuItem menuItem in _menuStrip.Items)
            {
                // Dubious hack for & in title. Better way to find item?
                if (menuItem.Text.Trim('&') == mainMenuText)
                {
                    item = menuItem;
                    break;
                }
            }

            if (item == null)
            {
                item = new ToolStripMenuItem(mainMenuText);
                _menuStrip.Items.Insert(_menuStrip.Items.Count - 2, item);
            }

            ToolStripItem[] subitems = item.DropDownItems.Find(subMenuText, false);
            ToolStripItem subitem = null;
            if (subitems.Length == 0)
            {
                subitem = new ToolStripMenuItem(subMenuText);
                //subitem.Tag = new MenuInfo(plugin, null);
                subitem.Tag = new MenuInfo(plugin, _windowManager.ProjectBrowser.RootItem);
                subitem.Click += plugin_MenuClick;
                item.DropDownItems.Insert(item.DropDownItems.Count, subitem);
                _pluginIndex.Add(plugin.GetID(), subitem);
            }
            else
            {
                _windowManager.Logger.LogStr("Failed to add menu item:" + mainMenuText + "|" + subMenuText);
            }
        }

        public void UpdateContextMenu(ContextMenuStrip contextMenu, BaseItem item)
        {
            // The context menu needs to be updated depending on the type of the item.
            if (_contextMenuIndex.ContainsKey(contextMenu))
            {
                // We've been here already, so clear the menu below this point.
                int defaultCount = _contextMenuIndex[contextMenu];
                for (int i = contextMenu.Items.Count - 1; i >= defaultCount; i--)
                {
                    contextMenu.Items.RemoveAt(i);
                }
            }
            else
            {
                _contextMenuIndex.Add(contextMenu, contextMenu.Items.Count);
            }

            if (item == null) return;

            // The rest of the menu is context sensitive on the underlying BaseItem.
            Type type = item.GetType();
            if (_contextOptions.ContainsKey(type))
            {
                List<ToolStripMenuItem> list = _contextOptions[type];
                if (list.Count > 0)
                {
                    if (contextMenu.Items.Count > 0) contextMenu.Items.Add(new ToolStripSeparator());
                    foreach (ToolStripMenuItem mi in list)
                    {
                        contextMenu.Items.Add(mi);

                        // Hang onto the item as well for context clicks.
                        ((MenuInfo)mi.Tag).Item = item;
                    }
                }
            }

            // Add any items that will always be at the end.
            if (contextMenu.Items.Count > 0) contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(_addToFavouritesMI);

            // Make sure the tags are up to date for any menu function to use.
            (_addToFavouritesMI.Tag as MenuInfo).Item = item;
        }


        internal MenuManager(WindowManager windowManager, MenuStrip menuStrip)
        {
            _windowManager = windowManager;
            _menuStrip = menuStrip;

            _addToFavouritesMI.Name = "_addToFavouritesMI";
            _addToFavouritesMI.Size = new System.Drawing.Size(171, 22);
            _addToFavouritesMI.Text = "Add to Favourites";
            _addToFavouritesMI.Click += new System.EventHandler(_addToFavouritesMI_Click);
            _addToFavouritesMI.Tag = new MenuInfo(null, null);
        }

        internal void AddContextMenuOption(IWindowPlugin plugin, List<Type> types, string text)
        {
            // Each plugin may deal with multiple types.
            foreach (Type t in types)
            {
                List<ToolStripMenuItem> list;
                if (_contextOptions.ContainsKey(t))
                {
                    list = _contextOptions[t];
                }
                else
                {
                    list = new List<ToolStripMenuItem>();
                    _contextOptions.Add(t, list);
                }

                ToolStripMenuItem mi = new ToolStripMenuItem();
                mi.Text = text;
                mi.Click += plugin_MenuClick;

                // Hang onto the plugin for menu events.
                mi.Tag = new MenuInfo(plugin, null);

                list.Add(mi);

                string id = plugin.GetID();
                _contextTypeIndex.Add(id + "_" + t.FullName, mi);
            }
        }

        internal void UpdateMenuOption(IWindowPlugin plugin)
        {
            string id = plugin.GetID();
            if (!_pluginIndex.ContainsKey(id))
            {
                _windowManager.Logger.LogStr("Failed to update menu item");
                return;
            }

            _pluginIndex[id].Tag = new MenuInfo(plugin, null);
        }

        internal void UpdateContextMenuOption(IWindowPlugin plugin)
        {
            string id = plugin.GetID();
            List<Type> types = plugin.GetHandledTypes();
            foreach (Type type in types)
            {
                if (_contextTypeIndex.ContainsKey(id + "_" + type.FullName))
                {
                    _contextTypeIndex[id + "_" + type.FullName].Tag = new MenuInfo(plugin, null);
                }
                else
                {
                    _windowManager.Logger.LogStr("Browser failed to update menu item");
                }
            }
        }

        private WindowManager _windowManager;
        private MenuStrip _menuStrip;
        private Dictionary<string, ToolStripItem> _pluginIndex = new Dictionary<string, ToolStripItem>();
        private Dictionary<Type, List<ToolStripMenuItem>> _contextOptions = new Dictionary<Type, List<ToolStripMenuItem>>();
        private Dictionary<string, ToolStripMenuItem> _contextTypeIndex = new Dictionary<string, ToolStripMenuItem>();
        private Dictionary<ContextMenuStrip, int> _contextMenuIndex = new Dictionary<ContextMenuStrip, int>();
        private ToolStripMenuItem _addToFavouritesMI = new ToolStripMenuItem();

        private void _addToFavouritesMI_Click(object sender, EventArgs e)
        {
            MenuInfo info = ((ToolStripMenuItem)sender).Tag as MenuInfo;
            if (info == null) return;
            if (info.Item == null) return;
            
            _windowManager.Favourites.Add(info.Item);
        }



        private void plugin_MenuClick(object sender, EventArgs e)
        {
            MenuInfo info = (sender as ToolStripMenuItem).Tag as MenuInfo;
            DockContent dock = info.Plugin as DockContent;

            // Make sure the window manager active cache is up to date. This
            // itself will update the plugin if it's visible.
            _windowManager.UpdatePluginItem(info.Plugin, info.Item);

            // If it's not, then make it visible, and the show will call the update.
            if (dock.Visible == false)
            {
                dock.Show(_windowManager.DockPanel);
            }

            if (info.Plugin.GetKind() == WindowPluginKind.ProjectItem)
            {
                // And add to history.
                _windowManager.History.Add(info.Item);
            }
        }

    }

}

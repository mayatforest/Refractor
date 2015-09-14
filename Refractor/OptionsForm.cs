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
using WeifenLuo.WinFormsUI;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;


namespace Refractor
{
    /// <summary>
    /// Options display.
    /// </summary>
    internal partial class OptionsBrowser : Form 
    {
        internal OptionsBrowser(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            _windowManager = (WindowManager)serviceProvider.GetService(typeof(WindowManager));

          
            listBox1.Items.Clear();
            listBox1.Items.Add(new ListBoxItem("", false, false, "Main", _windowManager.AppOptions));

            foreach (IParserPlugin plugin in _windowManager.PluginManager.ParserPlugins)
            {
                PluginOptions po = plugin.GetOptions();
                listBox1.Items.Add(new ListBoxItem(plugin.GetID(), true, false, plugin.GetID(), po));
            }

            foreach (IWindowPlugin plugin in _windowManager.PluginManager.WindowPlugins)
            {
                PluginOptions po = plugin.GetOptions();
                listBox1.Items.Add(new ListBoxItem(plugin.GetID(), false, true, plugin.GetID(), po));
            }

            listBox1.SelectedIndex = 0;
        }
 
        private WindowManager _windowManager;

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int count = listBox1.SelectedItems.Count;
            object[] objs = new object[count];
            for (int i = 0; i < count; i++)
            {
                // Always get the most recent.
                ListBoxItem li = (listBox1.SelectedItems[i] as ListBoxItem);
                if (li.IsParser)
                {
                    objs[i] = _windowManager.PluginManager.GetPluginOptions(li.Id);
                }
                else if (li.IsWindow)
                {
                    objs[i] = _windowManager.PluginManager.GetPluginOptions(li.Id);
                }
                else
                {
                    objs[i] = _windowManager.AppOptions;
                }
            }
            propertyGrid1.SelectedObjects = objs;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            Close();
        }

        private class ListBoxItem
        {
            public string Id;
            public bool IsParser;
            public bool IsWindow;
            public string Caption;
            public object Options;

            public ListBoxItem(string id, bool isParser, bool isWindow, string caption, object options)
            {
                Id = id;
                IsParser = isParser;
                IsWindow = isWindow;
                Caption = caption;
                Options = options;
            }

            public override string ToString()
            {
                return Caption;
            }
        }
    }

}

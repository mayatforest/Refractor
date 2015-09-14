using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI;

namespace Refractor.Common
{
    public interface IWindowManager
    {
        void SetActiveItem(BaseItem item, object sender);

        PluginOptions GetPluginOptions(string pluginID);
    }

    public interface IMenuManager
    {
        void UpdateContextMenu(ContextMenuStrip menu, BaseItem item);
    }

    public interface IStatusManager
    {
        void SetLeftCaption(string text);
        void Refresh();
    }

}

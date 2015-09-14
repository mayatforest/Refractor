using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Refractor;

namespace Refractor.Common
{
    public delegate void DNoArgsDelegate();
    public delegate void DSingleStringDelegate(string text);
    public delegate void DSingleObjectDelegate(object sender);

    public interface IActiveItemViewer 
    {
        void SetActiveItem(BaseItem item);
        void SetRefresh(BaseItem item);
        BaseItem GetActiveItem();
    }

    public enum WindowPluginKind { MainWindow, ProjectItem }

    public interface IWindowPlugin
    {
        string GetID();
        List<Type> GetHandledTypes();
        WindowPluginKind GetKind();
        void SetServiceProvider(IServiceProvider serviceProvider);
        PluginOptions GetOptions();
        void SetOptions(PluginOptions options);
    }

    public interface IParserPlugin
    {
        string GetID();
        List<string> HandlesExtensions();
        List<Type> HandlesItems();
        BaseItem GetFileItem(string name);
        bool ReadItem(BaseItem item, bool unload);
        bool FindChildren(BaseItem parent, List<BaseItem> items);
        bool IsLeaf(BaseItem item);
        void CalcMetrics(BaseItem item);
        void SetServiceProvider(IServiceProvider serviceProvider);
        PluginOptions GetOptions();
        void SetOptions(PluginOptions options);
    }
}

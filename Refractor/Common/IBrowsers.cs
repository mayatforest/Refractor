using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using WeifenLuo.WinFormsUI.Docking;

namespace Refractor.Common
{ 
    public interface IBrowser
    {
        void Show(DockPanel dockPanel);
        void Hide();
        bool Visible { get; }
    }

    public interface IProjectBrowser : IBrowser
    {
        string ProjectFilename { get; }
        Dictionary<string, BaseItem> Files { get; }
        BaseItem GetActiveItem();
        void SetActiveItem(BaseItem item, object sender);
        BaseItem Lookup(string id);
        void AddLookup(BaseItem item, string id, bool invert);
        event EventHandler OnFilesChanged;
    }

    public interface ILogView : IBrowser
    {
        void Debug(string message);
        void LogStr(string message);
        void LogExc(Exception exc);
        void LogExcStr(Exception exc, string message);
        bool LogCatchAll(Exception exc, string message);
    }

   
    

}

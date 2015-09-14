using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using System.Windows.Forms;

using Refractor.Common;

namespace Refractor.Plugins.ILDiagrams
{
    /// <summary>
    /// Show a diagram of namespace dependencies.
    /// </summary>
    public partial class AllNamespaces : BaseDiagram
    {

        public AllNamespaces()
        {
            InitializeComponent();
        }
        
        public override string GetID()
        {
            return "All Namespaces";
        }

        public override List<Type> GetHandledTypes()
        {
            return new List<Type> { typeof(RootItem) };
        }

        public override WindowPluginKind GetKind()
        {
            return WindowPluginKind.MainWindow;
        }

        public override PluginOptions GetOptions()
        {
            if (_options == null)
            {
                _options = new NamespacesOptions(this.GetID());
            }

            return _options;
        }

        public override void SetOptions(PluginOptions options)
        {
            if (options == null) return;
            if (!(options is NamespacesOptions)) return;

            _options = options as NamespacesOptions;
        }

        protected override BaseGraphBuilder CreateBuilder()
        {
            return new AllNamespacesGb();
        }

        
        //public override void SetServiceProvider(IServiceProvider serviceProvider)
        //{
        //    base.SetServiceProvider(serviceProvider);

        //    _projectBrowser.OnFilesChanged += new EventHandler(ProjectBrowser_OnFilesChanged);
        //}

        //private void ProjectBrowser_OnFilesChanged(object sender, EventArgs e)
        //{
        //    _activeItem = sender as RootItem;
        //    if (_activeItem == null) return;

        //    SetActiveItem(_activeItem);
        //}

        //private void _cbxFlatten_Click(object sender, EventArgs e)
        //{
        //    if (_options == null) return;
        //    if (!(_options is NamespacesOptions)) return;

        //    _cbxFlatten.Checked = !_cbxFlatten.Checked;
        //    (_options as NamespacesOptions).FlattenNamespaces = _cbxFlatten.Checked;
        //}


    }


    [Serializable]
    public class NamespacesOptions : BaseDiagramOptions
    {
        public NamespacesOptions(string id) : base(id) { }
        public NamespacesOptions() { }

        private bool _flattenNamespaces = false;
        public bool FlattenNamespaces { get { return _flattenNamespaces; } set { _flattenNamespaces = value; } }
    }
}

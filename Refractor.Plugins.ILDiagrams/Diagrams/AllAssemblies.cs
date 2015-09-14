using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Refractor.Common;

namespace Refractor.Plugins.ILDiagrams
{
    /// <summary>
    /// Show a diagram of assembly dependencies.
    /// </summary>
    public partial class AllAssemblies : BaseDiagram
    {
        public AllAssemblies()
        {
            InitializeComponent();
        }

        public override string GetID()
        {
            return "All Assemblies";
        }

        public override List<Type> GetHandledTypes()
        {
            return new List<Type> { typeof(RootItem) };
        }

        public override WindowPluginKind GetKind()
        {
            return WindowPluginKind.MainWindow;
        }

        protected override BaseGraphBuilder CreateBuilder()
        {
            return new AllAssembliesGb();
        }


        public override void SetServiceProvider(IServiceProvider serviceProvider)
        {
            base.SetServiceProvider(serviceProvider);

            _projectBrowser.OnFilesChanged += new EventHandler(ProjectBrowser_OnFilesChanged);
        }

        private void ProjectBrowser_OnFilesChanged(object sender, EventArgs e)
        {
            _activeItem = sender as RootItem;
            if (_activeItem == null) return;

            SetActiveItem(_activeItem);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Refractor.Common;

namespace Refractor.Plugins.ILDiagrams
{
    /// <summary>
    /// Show a diagram of all namespaces within an assembly.
    /// </summary>
    public partial class AssemblyNamespaces : BaseDiagram
    {
        public AssemblyNamespaces()
        {
            InitializeComponent();
        }

        public override string GetID()
        {
            return "Assembly Namespaces";
        }

        public override List<Type> GetHandledTypes()
        {
            return new List<Type> { typeof(AssemblyItem) };
        }

        public override WindowPluginKind GetKind()
        {
            return WindowPluginKind.ProjectItem;
        }

        protected override BaseGraphBuilder CreateBuilder()
        {
            return new AssemblyNamespacesGb();
        }
    }
}

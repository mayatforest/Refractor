using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Refractor.Common;

namespace Refractor.Plugins.ILDiagrams
{
    /// <summary>
    /// Show a diagram of the classes in a namespace.
    /// </summary>
    public partial class NamespaceClasses : BaseDiagram
    {
        public NamespaceClasses() : base()
        {
            InitializeComponent();
        }

        public override string GetID()
        {
            return "Namespace Classes";
        }

        public override List<Type> GetHandledTypes()
        {
            return new List<Type> { typeof(NamespaceItem) };
        }

        protected override BaseGraphBuilder CreateBuilder()
        {
            return new NamespaceClassesGb();
        }


    }
}

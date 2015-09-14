using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Refractor.Common;

namespace Refractor.Plugins.ILDiagrams
{
    /// <summary>
    /// Show a diagram of all assemblies that directly call or are called from the selected assembly.
    /// </summary>
    public partial class AssemblyButterfly : BaseDiagram
    {
        public AssemblyButterfly() : base()
        {
            InitializeComponent();
        }

        public override string GetID()
        {
            return "Assembly Butterfly";
        }

        public override List<Type> GetHandledTypes()
        {
            return new List<Type> { typeof(AssemblyItem) };
        }

        protected override BaseGraphBuilder CreateBuilder()
        {
            return new AssemblyButterflyGb();
        }
    }
}

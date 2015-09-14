using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Refractor.Common;

namespace Refractor.Plugins.ILDiagrams
{
    /// <summary>
    /// Show a diagram of all calls to and from a method.
    /// </summary>
    public partial class MethodButterfly : BaseDiagram
    {
        public MethodButterfly() :base()
        {
            InitializeComponent();
        }

        public override string GetID()
        {
            return "Method Call Usage Hierarchy";
        }

        public override List<Type> GetHandledTypes()
        {
            return new List<Type> { typeof(MethodItem) };
        }

        protected override BaseGraphBuilder CreateBuilder()
        {
            return new MethodButterflyGb();
        }

    }
}

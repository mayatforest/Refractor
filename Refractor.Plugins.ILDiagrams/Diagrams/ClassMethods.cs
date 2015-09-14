using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Refractor.Common;

namespace Refractor.Plugins.ILDiagrams
{
    /// <summary>
    /// Show a diagram of method calls within a class.
    /// </summary>
    public partial class ClassMethods : BaseDiagram
    {
        public ClassMethods() : base()
        {
            InitializeComponent();
        }

        public override string GetID()
        {
            return "Class Methods";
        }

        public override List<Type> GetHandledTypes()
        {
            return new List<Type> { typeof(TypeItem) };
        }

        protected override BaseGraphBuilder CreateBuilder()
        {
            return new ClassMethodsGb();
        }
    }
}

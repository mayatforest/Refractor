using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Refractor.Common;

namespace Refractor.Plugins.ILDiagrams
{
    /// <summary>
    /// Show a diagram of all classes that directly call or are called from the selected class.
    /// </summary>
    public partial class ClassButterfly : BaseDiagram
    {
        public ClassButterfly() : base()
        {
            InitializeComponent();
        }
            
        public override string GetID()
        {
            return "Class Butterfly";
        }

        public override List<Type> GetHandledTypes()
        {
            return new List<Type> { typeof(TypeItem) };
        }

        protected override BaseGraphBuilder CreateBuilder()
        {
            return new ClassButterflyGb();
        }

    }
}

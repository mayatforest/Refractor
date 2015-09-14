using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Refractor.Common;

namespace Refractor.Plugins.JScript
{
    /// <summary>
    /// Show a diagram of method calls within a class.
    /// </summary>
    public partial class JSMethods : BaseDiagram
    {
        public JSMethods() : base()
        {
            InitializeComponent();
        }

        public override string GetID()
        {
            return "JS Methods";
        }

        public override List<Type> GetHandledTypes()
        {
            return new List<Type> { typeof(JSClassItem), typeof(JSFileItem) };
        }
       
        protected override BaseGraphBuilder CreateBuilder()
        {
            return new JSMethodsGb();
        }
    }
}

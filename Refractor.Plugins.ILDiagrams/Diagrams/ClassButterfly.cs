using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Refractor.Common;

namespace Refractor.Plugins.ILDiagrams
{

    public class ClassButterflyOptions : BaseDiagramOptions
    {
        public ClassButterflyOptions(string id) : base(id) { }
        public ClassButterflyOptions() { }

        private int _outerscanlevel = 5;
        public int OuterScanLevel { get { return _outerscanlevel; } set { _outerscanlevel = value; } }
    }

    /// <summary>
    /// Show a diagram of all classes that directly call or are called from the selected class.
    /// </summary>
    public partial class ClassButterfly : BaseDiagram
    {
        ClassButterflyOptions _ClassButterflyOptions=null;
        public ClassButterfly() : base()
        {
            InitializeComponent();
        }
            
        public override string GetID()
        {
            return "Class Call Usage Hierarchy";
        }

        public override List<Type> GetHandledTypes()
        {
            return new List<Type> { typeof(TypeItem) };
        }

        protected override BaseGraphBuilder CreateBuilder()
        {
            return new ClassButterflyGb();
        }

        public override PluginOptions GetOptions()
        {
            if (_ClassButterflyOptions == null)
            {
                _ClassButterflyOptions = new ClassButterflyOptions(this.GetID());
            }
            return _ClassButterflyOptions;
        }


    }
}

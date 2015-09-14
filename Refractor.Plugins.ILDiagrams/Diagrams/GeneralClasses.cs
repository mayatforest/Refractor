using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Refractor.Common;

namespace Refractor.Plugins.ILDiagrams
{
    /// <summary>
    /// Show a diagram of general classes, via the side panel.
    /// </summary>
    public partial class GeneralClasses : BaseDiagram
    {
        public GeneralClasses()
            : base()
        {
            InitializeComponent();
            this._sidePanel.SetCaption("Include Types");
        }

        public override string GetID()
        {
            return "General Classes";
        }

        public override List<Type> GetHandledTypes()
        {
            return new List<Type> { typeof(TypeItem) };
        }

        public override void SetActiveItem(BaseItem item)
        {
            _activeItem = item;

            // For this diagram, when set active items is called, we just add
            // the item to the side panel.
            this._sidePanel.AddItem(item);

            if (_options.AutoRefresh)
            {
                this.RunTranslate(item);
            }
        }

        protected override BaseGraphBuilder CreateBuilder()
        {
            return new GeneralClassesGb();
        }
    }
}

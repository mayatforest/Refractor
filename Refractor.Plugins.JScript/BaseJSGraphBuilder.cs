using System;
using System.Text;

using Refractor.Common;


namespace Refractor.Plugins.JScript
{
    public class BaseJSGraphBuilder : BaseGraphBuilder
    {
        protected JSParserOptions _sharedOptions;

        public override void BeforeTranslate()
        {
            // This saves us casting everywhere.
            _sharedOptions = _windowManager.GetPluginOptions("JS Parser Plugin") as JSParserOptions;            
        }
    }
}

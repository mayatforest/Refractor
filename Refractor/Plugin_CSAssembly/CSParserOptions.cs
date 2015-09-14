using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Refractor.Common;

namespace Refractor.Plugins
{
    [Serializable]
    public class CSParserOptions : PluginOptions
    {
        public CSParserOptions(string id) : base(id) { }
        public CSParserOptions() { }

        private Color _assemblyColor = Color.LightBlue;
        public Color AssemblyColor { get { return _assemblyColor; } set { _assemblyColor = value; } }

        private Color _namespaceColor = Color.Silver;
        public Color NamespaceColor { get { return _namespaceColor; } set { _namespaceColor = value; } }

        private Color _typeColor = Color.Green;
        public Color TypeColor { get { return _typeColor; } set { _typeColor = value; } }

        private Color _methodColor = Color.White;
        public Color MethodColor { get { return _methodColor; } set { _methodColor = value; } }
    }
}

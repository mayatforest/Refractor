using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Refractor;

namespace Refractor.Common
{
    public class BaseDiagramOptions : PluginOptions
    {
        public BaseDiagramOptions(string id) : base (id) {}
        public BaseDiagramOptions() { }

        private bool _multipleEdges = false;
        public bool MultipleEdges { get { return _multipleEdges; } set { _multipleEdges = value; } }

        private bool _thickenEdges = false;
        public bool ThickenEdges { get { return _thickenEdges; } set { _thickenEdges = value; } }

        private bool _leftToRight = false;
        public bool LeftToRight { get { return _leftToRight; } set { _leftToRight = value; } }

        private bool _showOrphans = false;
        public bool ShowOrphans { get { return _showOrphans; } set { _showOrphans = value; } }


        private Color _backgroundColour = Color.White;
        public Color BackgroundColour { get { return _backgroundColour; } set { _backgroundColour = value; } }

        private int _panelWidth = 150;
        public int PanelWidth { get { return _panelWidth; } set { _panelWidth = value; } }

        private int _textSpacing = 2;
        public int TextSpacing { get { return _textSpacing; } set { _textSpacing = value; } }
        
        private bool _autoRefreshIgnores = true;
        public bool AutoRefresh { get { return _autoRefreshIgnores; } set { _autoRefreshIgnores = value; } }

        private bool _hiliteNode = true;
        public bool HiliteNode { get { return _hiliteNode; } set { _hiliteNode = value; } }

        private bool _hiliteLazy = true;
        public bool HiliteLazy { get { return _hiliteLazy; } set { _hiliteLazy = value; } }

        private Color _hiliteColor = Color.Yellow;
        public Color HiliteColor { get { return _hiliteColor; } set { _hiliteColor = value; } }

        private Color _hiliteLineColor = Color.Blue;
        public Color HiliteLineColor { get { return _hiliteLineColor; } set { _hiliteLineColor = value; } }
    }
}

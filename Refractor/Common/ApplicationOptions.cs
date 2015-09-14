using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Refractor.Common
{
    internal class ApplicationOptions
    {
        public int Left = 0;
        public int Top = 0;
        public int Width = 500;
        public int Height = 400; 
        public bool Maximized = false;
        public bool LayoutLocked = false;
        public int ProjectCol0Width = 300;
        public int ProjectCol1Width = 60;
        public int ProjectCol2Width = 70;
        public int ProjectCol3Width = 100;
        public string ProjectFilename = string.Empty;
        public List<PluginOptions> PluginOptions = new List<PluginOptions>();
        public List<PluginOptions> PluginParserOptions = new List<PluginOptions>();


        private bool _saveProjectOnExit = true;
        public bool SaveProjectOnExit { get { return _saveProjectOnExit; } set { _saveProjectOnExit = value; } }

        private bool _loadLastProjectOnStartup = true;
        public bool LoadLastProjectOnStartup { get { return _loadLastProjectOnStartup; } set { _loadLastProjectOnStartup = value; } }

        private bool _runProjectWorker = true;
        public bool RunProjectWorker { get { return _runProjectWorker; } set { _runProjectWorker = value; } }

    }
}

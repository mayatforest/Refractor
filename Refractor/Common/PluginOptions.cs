using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Refractor;

namespace Refractor.Common
{
    public class PluginOptions
    {
        public string PluginId = null;

        public PluginOptions()
        {
        }

        public PluginOptions(string id)
        {
            PluginId = id;
        }

    }

}

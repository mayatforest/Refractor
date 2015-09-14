using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

using Refractor.Common;

namespace Refractor
{
    internal class StatusManager : IStatusManager
    {
        public StatusManager(WindowManager windowManager, StatusStrip statusStrip)
        {
            _windowManager = windowManager;
            _statusStrip = statusStrip;

            _leftLabel = new ToolStripStatusLabel();
            _leftLabel.Alignment = ToolStripItemAlignment.Right;

            _statusStrip.Items.Add(_leftLabel);
        }

        public void SetLeftCaption(string text)
        {
            _leftLabel.Text = text;
        }

        public void Refresh()
        {
            _statusStrip.Refresh();
        }


        private WindowManager _windowManager;
        private StatusStrip _statusStrip;
        private ToolStripStatusLabel _leftLabel;
        private Dictionary<BackgroundWorker, ToolStripProgressBar> _progressBars = new Dictionary<BackgroundWorker, ToolStripProgressBar>();
    }

}

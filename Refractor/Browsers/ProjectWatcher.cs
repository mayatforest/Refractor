using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

using Refractor.Common;

namespace Refractor
{
    /// <summary>
    /// Maintain watchers for the files in the project. Originally this used 
    /// FileSystemWatcher, but when we know the individual files we want
    /// to watch, a poll was a better solution.
    /// </summary>
    internal class ProjectWatcher
    {
        internal ProjectWatcher(WindowManager windowManager, 
            Control control, DWatchChanged watchChanged)
        {
            _windowManager = windowManager;
            _control = control;
            _watchChanged = watchChanged;
            _timer.Interval = 2000;
            _timer.Tick += new EventHandler(_timer_Tick);
        }

        internal void Clear()
        {
            Stop();
            
            lock (_watchLock)
            {
                _watches.Clear();
            }
        }

        internal void Start()
        {
            _timer.Enabled = true;
        }

        internal void Stop()
        {
            _timer.Enabled = false;
        }

        internal void AddWatch(BaseItem fileItem)
        {
            try
            {
                string path = Path.GetDirectoryName(fileItem.ItemPath);

                if (!_watches.ContainsKey(fileItem.ItemPath))
                {
                    lock (_watchLock)
                    {
                        _watches.Add(fileItem.ItemPath, File.GetLastWriteTime(fileItem.ItemPath));
                    }
                }
            }
            catch (Exception exc)
            {
                if (_windowManager.Logger.LogCatchAll(exc, "Unexpected AddWatch error")) throw;
            }
        }

        internal void RemoveWatch(BaseItem fileItem)
        {
            try
            {
                string path = Path.GetDirectoryName(fileItem.ItemPath);

                if (_watches.ContainsKey(fileItem.ItemPath))
                {
                    lock (_watchLock)
                    {
                        _watches.Remove(fileItem.ItemPath);
                    }
                }
            }
            catch (Exception exc)
            {
                if (_windowManager.Logger.LogCatchAll(exc, "Unexpected RemoveWatch error")) throw;
            }
        }

        private WindowManager _windowManager;
        private Control _control;
        private DWatchChanged _watchChanged;
        private Timer _timer = new Timer();
        private Dictionary<string, DateTime> _watches = new Dictionary<string, DateTime>();
        private object _watchLock = new object();

        private void _timer_Tick(object sender, EventArgs e)
        {
            lock (_watchLock)
            {
                Dictionary<string, DateTime> modified = new Dictionary<string, DateTime>();

                foreach (KeyValuePair<string, DateTime> pair in _watches)
                {
                    DateTime lwt = File.GetLastWriteTime(pair.Key);

                    // Check if the last write time has changed.
                    if (pair.Value != lwt)
                    {
                        // Invoke the watch changed delegate on the UI thread.
                        _control.Invoke(_watchChanged, new object[1] { pair.Key });
                        
                        modified.Add(pair.Key, lwt);
                    }
                }

                // Update the watches.
                foreach (KeyValuePair<string, DateTime> pair in modified)
                {
                    _watches[pair.Key] = pair.Value;
                }
            }

        }
    }

    internal delegate void DWatchChanged(string filename);
}

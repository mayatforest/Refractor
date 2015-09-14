using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Refractor.Common;
using WeifenLuo.WinFormsUI.Docking;

namespace Refractor
{
    /// <summary>
    /// A logging window, dealing with threaded logging, to screen and to file.
    /// </summary>
    internal partial class LogView : DockContent, ILogView
    {
        static LogView()
        {
            if (File.Exists(LogFilename))
            {
                FileInfo fi = new FileInfo(LogFilename);
                if (fi.Length > 10000)
                {
                    File.Delete(LogFilename);
                }
            }
        }

        public static void LogToFile(string message)
        {
            File.Delete(StaticLogFilename);
            using (StreamWriter sw = File.AppendText(StaticLogFilename))
            {
                sw.WriteLine(message);
            }
        }

        public LogView(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _onClear = ClearInternal;
            _onLogStr = LogStrInternal;
            _onLogExc = LogExcInternal;
            _onLogExcStr = LogExcStrInternal;
        }

        public void Clear()
        {
            if (!rtbLog.IsDisposed && rtbLog.InvokeRequired)
            {
                rtbLog.Invoke(_onClear);
            }
            else ClearInternal();
        }

        public void Debug(string message)
        {
            #if DEBUG
            LogStr(message);
            #endif
        }
                

        public void LogStr(string message)
        {
            message = GetText(message);

            if (!rtbLog.IsDisposed && rtbLog.InvokeRequired)
            {
                rtbLog.Invoke(_onLogStr, new object[1] { message });
            }
            else LogStrInternal(message);
        }

        public void LogExc(Exception exc)
        {
            if (!rtbLog.IsDisposed && rtbLog.InvokeRequired)
            {
                rtbLog.Invoke(_onLogExc, new object[1] { exc });
            }
            else LogExcInternal(exc);
        }

        public void LogExcStr(Exception exc, string message)
        {
            message = GetText(message);

            if (!rtbLog.IsDisposed && rtbLog.InvokeRequired)
            {
                rtbLog.Invoke(_onLogExcStr, new object[2] { exc, message });
            }
            else LogExcStrInternal(exc, message);
        }

        public bool LogCatchAll(Exception exc, string message)
        {
            // For a plugin architecture, we can't know what errors may be thrown, and we want
            // to catch them all, except for a list of fatal errors, which we need to throw.
            if (exc is OutOfMemoryException || exc is StackOverflowException || exc is OverflowException)
                return true;

            message = GetText(message);

            if (!rtbLog.IsDisposed && rtbLog.InvokeRequired)
            {
                rtbLog.Invoke(_onLogExcStr, new object[2] { exc, message });
            }
            else LogExcStrInternal(exc, message);

            return false;
        }

        private delegate void DOnClear();
        private delegate void DOnLogStr(string message);
        private delegate void DOnLogExc(Exception exc);
        private delegate void DOnLogExcStr(Exception exc, string message);
        private DOnClear _onClear;
        private DOnLogStr _onLogStr;
        private DOnLogExc _onLogExc;
        private DOnLogExcStr _onLogExcStr;
        private object _lock = new object();
        private static string LogFilename = Application.ExecutablePath + ".log";
        private static string StaticLogFilename = Application.ExecutablePath + ".static.log";

        private string GetText(string message)
        {
            if (Thread.CurrentThread.ManagedThreadId == 1)
                return message;
            else
                return Thread.CurrentThread.ManagedThreadId + ":" + message;
        }

        private void AppendText(string s, bool scroll)
        {
            if (!rtbLog.IsDisposed)
            {
                rtbLog.AppendText(s);
                if (scroll) rtbLog.ScrollToCaret();
            }
        }

        private void ClearInternal()
        {
            if (!rtbLog.IsDisposed)
            {
                rtbLog.Clear();
            }

            ToFileClear();
        }

        private void LogStrInternal(string s)
        {
            AppendText(s + "\n", true);

            ToFile(s);
        }

        private void LogExcInternal(Exception exc)
        {
            if (exc is ReflectionTypeLoadException)
            {
                ToFile("  ReflectionTypeLoadException :");

                AppendText("  ReflectionTypeLoadException :\n", false);
                List<string> shown = new List<string>();
                foreach (Exception excLoad in (exc as ReflectionTypeLoadException).LoaderExceptions)
                {
                    if (shown.Contains(excLoad.Message)) continue;

                    AppendText("    " + excLoad.Message + "\n", false);

                    ToFile("    " + excLoad.Message);

                    shown.Add(excLoad.Message);
                }
            }
            else
            {
                ToFile("  " + exc.GetType() + " : " + exc.Message + "\n" + exc.StackTrace + "\n");

                AppendText("  " + exc.GetType() + " : " + exc.Message + "\n", false);

                if (exc.InnerException != null)
                {
                    ToFile("  InnerException:" + exc.InnerException.Message + "\n");

                    AppendText("  InnerException:" + exc.InnerException.Message + "\n", false);
                }
            }
        }

        private void LogExcStrInternal(Exception exc, string message)
        {
            ToFile(message);

            AppendText(message + "\n", false); 
            LogExcInternal(exc);
        }


        private void ToFile(string s)
        {
            lock (_lock)
            {
                using (StreamWriter sw = File.AppendText(LogFilename))
                {
                    sw.WriteLine(s);
                }
            }
        }

        private void ToFileClear()
        {
            lock (_lock)
            {
                using (StreamWriter sw = File.AppendText(LogFilename))
                {
                    sw.WriteLine();
                    sw.WriteLine();
                }
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbLog.Clear();
        }
    }


}

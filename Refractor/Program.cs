using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

using Refractor.Common;

namespace Refractor
{
	static class Program
	{
        /// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
            Application.EnableVisualStyles();
		    Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException +=
                new ThreadExceptionEventHandler(Application_ThreadException);

            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(AppDomain_UnhandledException);  

            // For manual positioning of the form.
            MainForm form = new MainForm();
            form.StartPosition = FormStartPosition.Manual;

            try
            {
                Application.Run(form);
            }
            catch (Exception exc)
            {
                LogView.LogToFile("Unhandled Run Exception:\n" + exc.ToString());

                DialogResult result = MessageBox.Show(
                    "Unhandled Run Exception:\n" + exc.ToString() +
                    "\nApplication will exit..",
                    "Fatal Application Exception",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            LogView.LogToFile("Unhandled Thread Exception:\n" + e.Exception.ToString());

            DialogResult result = MessageBox.Show(
                "Unhandled Thread exception:\n" + e.Exception.ToString() + 
                "\nAttempt to continue?",
                "Application Thread Exception",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation);

            if (result == DialogResult.No)
                Application.Exit();
        }

        static void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogView.LogToFile("Unhandled Exception:\n" + e.ExceptionObject.ToString());

            if (e.IsTerminating)
            {
                DialogResult result = MessageBox.Show(
                    "Unhandled Exception:\n" + e.ExceptionObject.ToString() +
                    "\nApplication will exit..",
                    "Fatal Application Exception",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
            else
            {
                DialogResult result = MessageBox.Show(
                    "Unhandled Exception:\n" + e.ExceptionObject.ToString() + 
                    "\nAttempt to continue?",
                    "Application Exception",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation);

                if (result == DialogResult.No)
                    Application.Exit();
            }
        }
    }
}
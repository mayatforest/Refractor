using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SDILReader;
using System.Reflection;

namespace SDILReaderTest
{
    public partial class frmTestILReader : Form
    {
        private string _asmSearchPath;
        
        public List<MethodInfo> methods = new List<MethodInfo>();

        public frmTestILReader()
        {
            Globals.LoadOpCodes();
            InitializeComponent();

            // http://www.devcity.net/Articles/254/1/article.aspx
            AppDomain.CurrentDomain.AssemblyResolve +=
                new ResolveEventHandler(CurrentDomain_AssemblyResolve);

        }

        private void btnOpenAssembly_Click(object sender, EventArgs e)
        {
            // clear the methods cache
            methods.Clear();

            // clear the listview with the available methods
            lbAvailableMethodsList.Items.Clear();

            dlgOpenAssembly.ShowDialog();
            // get the filename of the assembly
            string assemblyName = dlgOpenAssembly.FileName;
            try
            {

//hwd
                _asmSearchPath = Path.GetDirectoryName(assemblyName);
//
                // load the assembly
                Assembly ass = Assembly.LoadFile(assemblyName);

                // get all the methods within the loaded assembly
                Module[] modules = ass.GetModules();
                for (int i = 0; i < modules.Length; i++)
                {
                    Type[] types = modules[i].GetTypes();
                    for (int k = 0; k < types.Length; k++)
                    {
                        MethodInfo[] mis = types[k].GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                        for (int j = 0; j < mis.Length; j++)
                        {
                            // check if the method has a body
                            if (mis[j].GetMethodBody() != null)
                            {
                                methods.Add(mis[j]);
                                lbAvailableMethodsList.Items.Add(mis[j].Name);
                            }
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Invalid assembly");
            }
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {

            // old

            string[] asmName = args.Name.Split(',');
            string asmPath = Path.Combine(_asmSearchPath, asmName[0] + ".dll");

            if (!File.Exists(asmPath)) return null;

            return Assembly.Load(asmPath);
        }

        private void lbAvailableMethodsList_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                MethodInfo mi = methods[lbAvailableMethodsList.SelectedIndex];
                SDILReader.MethodBodyReader mr = new MethodBodyReader(mi);
                rchMethodBodyCode.Clear();
                rchMethodBodyCode.Text = mr.GetBodyCode();    
            }
            catch
            {

            }
        }
    }
}
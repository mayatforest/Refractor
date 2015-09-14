using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Refractor
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void AboutForm_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AboutForm_Activated(object sender, EventArgs e)
        {
        }
    }


}
namespace SDILReaderTest
{
    partial class frmTestILReader
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOpenAssembly = new System.Windows.Forms.Button();
            this.dlgOpenAssembly = new System.Windows.Forms.OpenFileDialog();
            this.lbAvailableMethodsList = new System.Windows.Forms.ListBox();
            this.rchMethodBodyCode = new System.Windows.Forms.RichTextBox();
            this.lblCode = new System.Windows.Forms.Label();
            this.lblAvailableMethods = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOpenAssembly
            // 
            this.btnOpenAssembly.Location = new System.Drawing.Point(479, 124);
            this.btnOpenAssembly.Name = "btnOpenAssembly";
            this.btnOpenAssembly.Size = new System.Drawing.Size(106, 23);
            this.btnOpenAssembly.TabIndex = 0;
            this.btnOpenAssembly.Text = "Open Assembly ...";
            this.btnOpenAssembly.UseVisualStyleBackColor = true;
            this.btnOpenAssembly.Click += new System.EventHandler(this.btnOpenAssembly_Click);
            // 
            // dlgOpenAssembly
            // 
            this.dlgOpenAssembly.FileName = "*.*";
            // 
            // lbAvailableMethodsList
            // 
            this.lbAvailableMethodsList.FormattingEnabled = true;
            this.lbAvailableMethodsList.Location = new System.Drawing.Point(12, 26);
            this.lbAvailableMethodsList.Name = "lbAvailableMethodsList";
            this.lbAvailableMethodsList.Size = new System.Drawing.Size(461, 121);
            this.lbAvailableMethodsList.TabIndex = 1;
            this.lbAvailableMethodsList.SelectedValueChanged += new System.EventHandler(this.lbAvailableMethodsList_SelectedValueChanged);
            // 
            // rchMethodBodyCode
            // 
            this.rchMethodBodyCode.Location = new System.Drawing.Point(12, 175);
            this.rchMethodBodyCode.Name = "rchMethodBodyCode";
            this.rchMethodBodyCode.Size = new System.Drawing.Size(573, 177);
            this.rchMethodBodyCode.TabIndex = 2;
            this.rchMethodBodyCode.Text = "";
            this.rchMethodBodyCode.WordWrap = false;
            // 
            // lblCode
            // 
            this.lblCode.AutoSize = true;
            this.lblCode.Location = new System.Drawing.Point(12, 159);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(94, 13);
            this.lblCode.TabIndex = 3;
            this.lblCode.Text = "MethodBody code";
            // 
            // lblAvailableMethods
            // 
            this.lblAvailableMethods.AutoSize = true;
            this.lblAvailableMethods.Location = new System.Drawing.Point(9, 9);
            this.lblAvailableMethods.Name = "lblAvailableMethods";
            this.lblAvailableMethods.Size = new System.Drawing.Size(93, 13);
            this.lblAvailableMethods.TabIndex = 4;
            this.lblAvailableMethods.Text = "Available methods";
            // 
            // frmTestILReader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 364);
            this.Controls.Add(this.lblAvailableMethods);
            this.Controls.Add(this.lblCode);
            this.Controls.Add(this.rchMethodBodyCode);
            this.Controls.Add(this.lbAvailableMethodsList);
            this.Controls.Add(this.btnOpenAssembly);
            this.Name = "frmTestILReader";
            this.Text = "Test SDIILReader";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpenAssembly;
        private System.Windows.Forms.OpenFileDialog dlgOpenAssembly;
        private System.Windows.Forms.ListBox lbAvailableMethodsList;
        private System.Windows.Forms.RichTextBox rchMethodBodyCode;
        private System.Windows.Forms.Label lblCode;
        private System.Windows.Forms.Label lblAvailableMethods;
    }
}


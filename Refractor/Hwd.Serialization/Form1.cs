using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Hwd.Serialization
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Encoder encoder = new Encoder();
            encoder.Indent = true;

            Test o = new Test();
            string s = encoder.Encode(o);

            richTextBox1.Clear();
            richTextBox1.AppendText(s);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Decoder decoder = new Decoder();

            string s = richTextBox1.Text;

            try
            {
                Test o = (Test) decoder.Decode(s);
                richTextBox1.AppendText("\n" + "ok");
            }
            catch (Exception exc)
            {
                richTextBox1.AppendText("\n" + exc.ToString());
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Encoder encoder = new Encoder();
            encoder.Indent = true;
            Test o = new Test();
            string s = encoder.Encode(o);

            Decoder decoder = new Decoder();
            Test o2 = (Test)decoder.Decode(s);
            string s2 = encoder.Encode(o2);

            if (s == s2)
                richTextBox1.AppendText("OK\n");
            else 
                richTextBox1.AppendText("FAIL\n");
        }
    }

    public class MyBase
    {
        public string BaseField1 = "testbase1";
        public string BaseField2 = "testbase2";
        public string BaseField3 = "testbase3";
    }

    public class MySpecialiazed : MyBase
    {
        public string SpecField1 = "test1";
        public string SpecField2 = "test2";
        public string SpecField3 = "test3";
        public Color MyColor = Color.White;
        public DateTime MyDateTime = DateTime.Now;
    }

    public class Test
    {
        public string Field1 = "field1";
        public string Field2 = "field2";
        public string Field3 = "¬`!\"£$%&*()_-+={[}]:;@'~#<,>.?/|\\!)$!) and 'µ'"; 

        private List<MyBase> _theList;
        public List<MyBase> TheList { get { return _theList; } set { _theList = value; } }

        public Dictionary<string, List<string>> MyDictionary = new Dictionary<string, List<string>>();

        public Dictionary<string, List<MyBase>> MyDictionary2 = new Dictionary<string, List<MyBase>>();

        public Test()
        {
            _theList = new List<MyBase>();

            _theList.Add(new MySpecialiazed());
            _theList.Add(new MyBase());
            _theList.Add(new MySpecialiazed());


            List<string> list = new List<string>();
            list.Add("test1");
            list.Add("test2");
            MyDictionary.Add("1", list);

            MyDictionary2.Add("test", _theList);

        }
    }


}

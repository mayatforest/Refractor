using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Data.SqlClient;


namespace Dev
{

	/// <summary>
	/// general purpose logging class
	/// </summary>
	public class Log
	{
		private static string File1 = "c:\\_dev_log.txt";       // standard log ToFile()
		private static string File2 = "c:\\_dev_log_time.txt";  // time log ToFileTime()
		private static string File3 = "c:\\_dev_log_new.txt";   // new file each time ToFileNew()
		private static string File4 = "c:\\_dev_table.txt";     // table log new file each time TableToFileNew()		

        private static string File5 = "c:\\_dev_special.txt";     // 

        private static object _lock = new object();

		/// <summary>
		/// standard log
		/// </summary>
		/// <param name="s"></param>
		public static void ToFile(string s)
		{
            lock(_lock)
            {
                StreamWriter SW;
                SW = File.AppendText(File1);
                SW.WriteLine(s);
                SW.Flush();
                SW.Close();
            }
		}

        /// <summary>
        /// standard log
        /// </summary>
        /// <param name="s"></param>
        public static void ToFileAppend(string s)
        {
            lock (_lock)
            {
                StreamWriter SW;
                SW = File.AppendText(File1);
                SW.Write(s);
                SW.Flush();
                SW.Close();
            }
        }

        public static void ClearSpecial()
        {
            if (File.Exists(File5)) File.Delete(File5);
        }

        /// <summary>
        /// Incrementally build as csv
        /// </summary>
        /// <param name="list"></param>
        public static void ToFileSpecial(List<string> list)
        {
            lock (_lock)
            {
                if (File.Exists(File5))
                {
                    string[] lines = File.ReadAllLines(File5);
                    int i = 0;
                    foreach (string s in list)
                    {
                        lines[i] = lines[i] + s + ",";
                        i++;
                    }
                    while (i < lines.Length)
                    {
                        lines[i] = lines[i] + ",";
                        i++;
                    }
                    File.WriteAllLines(File5, lines);
                }
                else
                {
                    string[] lines = new string[20]; //MAX
                    for (int idx = 0; idx < 20; idx++)
                    {
                        lines[idx] = string.Empty;
                    }
                    int i = 0;
                    foreach (string s in list)
                    {
                        lines[i] = lines[i] + s + ",";
                        i++;
                    }
                    while (i < lines.Length)
                    {
                        lines[i] = lines[i] + ",";
                        i++;
                    }
                    File.WriteAllLines(File5, lines);
                }
            }

        }


        /// <summary>
        /// standard log
        /// </summary>
        /// <param name="s"></param>
        public static void ToFile(string s, string filename)
        {
            lock (_lock)
            {
                StreamWriter SW;
                SW = File.AppendText(filename);
                SW.WriteLine(s);
                SW.Flush();
                SW.Close();
            }
        }

		/// <summary>
		/// standard log with time stamp
		/// </summary>
		/// <param name="s"></param>
		public static void ToFileTime(string s)
		{
			StreamWriter SW;
			SW = File.AppendText(File2);
			SW.WriteLine(DateTime.Now.ToLongTimeString() + "." + DateTime.Now.Millisecond.ToString() + "\t : " + s);
			SW.Flush();
			SW.Close();
		}


		/// <summary>
		/// standard log new file each time
		/// </summary>
		/// <param name="s"></param>
		public static void ToFileNew(string s)
		{
			StreamWriter SW;
			SW = File.CreateText(File3);
			SW.WriteLine(s);
			SW.Flush();
			SW.Close();
		}


		/// <summary>
		/// DataTable to new file each time
		/// </summary>
		/// <param name="aDBTable"></param>
		public static void TableToFileNew(DataTable aDBTable)
		{
			int[] aWidths;
			aWidths = new Int32[aDBTable.Columns.Count];
			
			//widths
			for (int j=0; j<aDBTable.Columns.Count; j++)
			{
				aWidths[j] = 0;
				if (aDBTable.Columns[j].Caption.Length >= aWidths[j])
					aWidths[j] = aDBTable.Columns[j].Caption.Length + 1;

				for (int i=0; i<aDBTable.Rows.Count; i++)
					if (aDBTable.Rows[i].ItemArray[j].ToString().Length >= aWidths[j])
						aWidths[j] = aDBTable.Rows[i].ItemArray[j].ToString().Length + 1;
			}			
			
			StreamWriter SW;
			SW = File.CreateText(File4);

			//captions
			for (int j=0; j<aDBTable.Columns.Count; j++)
			{
				SW.Write(aDBTable.Columns[j].Caption);
				SW.Write(Util.GetNSpaces(aWidths[j] - aDBTable.Columns[j].Caption.Length));
			}
			SW.WriteLine("");
			for (int j=0; j<aDBTable.Columns.Count; j++)
			{
				for (int i=0; i<aWidths[j]; i++)				
					SW.Write("-");
			}
			SW.WriteLine("");
			
			//data
			for (int i=0; i<aDBTable.Rows.Count; i++)
			{
				for (int j=0; j<aDBTable.Columns.Count; j++)
				{
					SW.Write(aDBTable.Rows[i].ItemArray[j].ToString());
					SW.Write(Util.GetNSpaces(aWidths[j] - aDBTable.Rows[i].ItemArray[j].ToString().Length));

				}
				SW.WriteLine("");
			}			
			SW.Flush();
			SW.Close(); 		
		}


	
	}

	/// <summary>
	/// some useful conversion functions
	/// </summary>
	public class Util
	{

		/// <summary>
		/// gets n spaces
		/// </summary>
		/// <param name="N"></param>
		/// <returns></returns>
		public static string GetNSpaces(int N)
		{
			string aResult = "";
			for (int i=0; i<N; i++)
				aResult += " ";
			return aResult;
		}


		/// <summary>
		/// string[] to string
		/// </summary>
		/// <param name="sa"></param>
		/// <returns></returns>
		public static string StrArrayToStr(string[] sa)
		{
			string aResult = "";
			for (int i=0; i<=sa.GetUpperBound(0); i++)
			{
				aResult += sa[i] + ",";				
			}
			return aResult;
		}


		/// <summary>
		/// string[,] to string
		/// </summary>
		/// <param name="sa"></param>
		/// <returns></returns>
		public static string StrMatrixToStr(string[,] sa)
		{
			string aResult = "";
			for (int i=0; i<=sa.GetUpperBound(0); i++)
			{
				aResult += "[";
				for (int j=0; j<=sa.GetUpperBound(1); j++)
				{
					aResult += sa[i,j] + ",";
				}
				aResult += "]";
			}
			return aResult;
		}


		/// <summary>
		/// double[,] to string
		/// </summary>
		/// <param name="da"></param>
		/// <returns></returns>
		public static string DoubleMatrixToStr(double[,] da)
		{
			string aResult = "";
			for (int i=0; i<=da.GetUpperBound(0); i++)
			{
				aResult += "[";
				for (int j=0; j<=da.GetUpperBound(1); j++)
				{
					aResult += da[i,j].ToString() + ",";				
				}
				aResult += "]";
			}
			return aResult;
		}
	}
	
	
}			   

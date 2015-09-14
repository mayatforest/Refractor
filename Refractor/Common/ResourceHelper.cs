using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Refractor.Common
{    
    /// <summary>
    /// Based on http://www.codeproject.com/KB/cs/RERM.aspx
    /// Allows multiple assemblies and embedded files.
    /// </summary>
    public static class ResourceHelper
    {
        static ResourceHelper() 
        {
            _bitmaps = new Dictionary<string, Bitmap>();
            _files = new Dictionary<string, string>();
        }

        public static void CheckAssembly(Assembly target)
        {
            foreach (string resource in target.GetManifestResourceNames()) 
            {
                string ext = Path.GetExtension(resource).ToLower();

                // Pluck the simple name of the resource out of
                // the fully qualified string.  tokens[tokens.Length - 1]
                // is the file extension, also not needed.
                // NB This works only for resource names that don't contain the period.
                string[] tokens = resource.Split('.');
                string name = tokens[tokens.Length - 2];
                name = name.ToLower();                

                if (ext == ".bmp" || ext == ".gif" || ext == ".jpg" || ext == ".jpeg")
                {

                    Bitmap bitmap = (Bitmap)Bitmap.FromStream(target.GetManifestResourceStream(resource));
                    bitmap.MakeTransparent(Color.White);

                    _bitmaps.Add(name, bitmap);
                }
                else if (ext == ".config" || ext == ".xml" || ext == ".txt" || ext == ".dll")
                {
                    StreamReader sr = new StreamReader(target.GetManifestResourceStream(resource));
                    string fileText = sr.ReadToEnd();

                    _files.Add(name, fileText);
                }
            }
        }

        public static Bitmap GetBitmap(string name)
        {
            name = name.ToLower();
            if (_bitmaps.ContainsKey(name))
            {
                return _bitmaps[name];
            }
            
            return null;
        }

        public static string GetFileText(string name)
        {
            name = name.ToLower();
            if (_files.ContainsKey(name))
            {
                return _files[name];
            }

            return null;
        }

        private static Dictionary<string, Bitmap> _bitmaps;
        private static Dictionary<string, string> _files;

    }
}
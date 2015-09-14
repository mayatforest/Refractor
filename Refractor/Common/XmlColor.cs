//
// http://www.pocketnerd.net
//
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Reflection;

namespace Refractor.Common
{
    //hwd
    [TypeConverter(typeof(ColorConverter))]
    [Editor("System.Drawing.Design.ColorEditor, System.Drawing.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]

    /// <summary>
    /// Represents a color that can be serialized to XML
    /// </summary>
    [
    Serializable
    ]
    public struct XmlColor
    {
        /// <summary>
        /// Gets and Sets the color that this class acts as a wrapper for
        /// </summary>
        [
        XmlIgnore
        ]
        public Color Color
        {
            get { return Color.FromArgb(_a, _r, _g, _b); }
            set { _a = value.A; }
        }


        /// <summary>
        /// The alpha Component value
        /// </summary>
        private byte _a;

        /// <summary>
        /// Gets the alpha Component value
        /// </summary>
        public byte A
        {
            get { return _a; }
            set { _a = value; }
        }


        /// <summary>
        /// The blue Component value
        /// </summary>
        private byte _b;

        /// <summary>
        /// Gets the blue Component value
        /// </summary>
        public byte B
        {
            get { return _b; }
            set { _b = value; }
        }


        /// <summary>
        /// The green Component value
        /// </summary>
        private byte _g;

        /// <summary>
        /// Gets the green Component value
        /// </summary>
        public byte G
        {
            get { return _g; }
            set { _g = value; }
        }


        /// <summary>
        /// The red Component value
        /// </summary>
        private byte _r;

        /// <summary>
        /// Gets the red Component value
        /// </summary>
        public byte R
        {
            get { return _r; }
            set { _r = value; }
        }


        /// <summary>
        /// Creates a new XmlColor from the passed in color
        /// </summary>
        /// <param name="color">The color that is to be wrapped</param>
        public XmlColor(Color color)
        {
            _a = color.A;
            _b = color.B;
            _g = color.G;
            _r = color.R;
        }


        /// <summary>
        /// Creates a new XmlColor from the passed in color
        /// </summary>
        public XmlColor(byte a, byte r, byte g, byte b)
        {
            _a = a;
            _b = b;
            _g = g;
            _r = r;
        }


        /// <summary>
        /// Explicitly converts a Color into an XmlColor
        /// </summary>
        /// <param name="color">The Color that is to be converted</param>
        /// <returns>The new XmlColor</returns>
        public static explicit operator XmlColor(Color color)
        {
            XmlColor xmlColor = new XmlColor(
                color.A, color.R, color.G, color.B );
            
            return xmlColor;
        }


        /// <summary>
        /// Implicitly converts a Color into an XmlColor
        /// </summary>
        /// <param name="xmlColor">The XmlColor that is to be converted</param>
        /// <returns>The new XmlColor</returns>
        public static implicit operator Color(XmlColor xmlColor)
        {
            return xmlColor.Color;
        }


        /// <summary>
        /// Serializes the Color details into a string
        /// </summary>
        public string Serialize()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(GetType());

            string data = string.Empty;

            using (StringWriter writer = new StringWriter())
            {
                xmlSerializer.Serialize(writer, this);

                data = writer.ToString();
            }

            return data;
        }


        /// <summary>
        /// Loads the User Settings from the passed in file name
        /// </summary>
        /// <param name="data">The name of the file to load the user settings from</param>
        /// <returns>The previously saved User Settings or a new UserSettings instance if
        /// the settings have not previously been saved</returns>
        public static XmlColor Deserialize(string data)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof (XmlColor));

                using (XmlTextReader reader = new XmlTextReader(data, XmlNodeType.Document, null))
                {
                    return (XmlColor) xmlSerializer.Deserialize(reader);
                }
            }
            catch
            {
                return new XmlColor(Color.Empty);
            }
        }


        /// <summary>
        /// Gets the string that represents this object
        /// </summary>
        /// <returns>The string that represents this object</returns>
        public override string ToString()
        {
            return Color.ToString();
        }
    }




    public class ColorConverter : TypeConverter
    {
        private static Hashtable colorConstants;
        private static string ColorConstantsLock = "colorConstants";
        private static Hashtable systemColorConstants;
        private static string SystemColorConstantsLock = "systemColorConstants";
        private static TypeConverter.StandardValuesCollection values;
        private static string ValuesLock = "values";

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return ((destinationType == typeof(InstanceDescriptor)) || base.CanConvertTo(context, destinationType));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string str = value as string;
            if (str == null)
            {
                return base.ConvertFrom(context, culture, value);
            }
            object namedColor = null;
            string name = str.Trim();
            if (name.Length == 0)
            {
                return XmlColor.Empty;
            }
            namedColor = GetNamedColor(name);
            if (namedColor == null)
            {
                if (culture == null)
                {
                    culture = CultureInfo.CurrentCulture;
                }
                char ch = culture.TextInfo.ListSeparator[0];
                bool flag = true;
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
                if (name.IndexOf(ch) == -1)
                {
                    if (((name.Length >= 2) && ((name[0] == '\'') || (name[0] == '"'))) && (name[0] == name[name.Length - 1]))
                    {
                        namedColor = XmlColor.FromName(name.Substring(1, name.Length - 2));
                        flag = false;
                    }
                    else if ((((name.Length == 7) && (name[0] == '#')) || ((name.Length == 8) && (name.StartsWith("0x") || name.StartsWith("0X")))) || ((name.Length == 8) && (name.StartsWith("&h") || name.StartsWith("&H"))))
                    {
                        namedColor = XmlColor.FromArgb(-16777216 | ((int) converter.ConvertFromString(context, culture, name)));
                    }
                }
                if (namedColor == null)
                {
                    string[] strArray = name.Split(new char[] { ch });
                    int[] numArray = new int[strArray.Length];
                    for (int i = 0; i < numArray.Length; i++)
                    {
                        numArray[i] = (int) converter.ConvertFromString(context, culture, strArray[i]);
                    }
                    switch (numArray.Length)
                    {
                        case 1:
                            namedColor = XmlColor.FromArgb(numArray[0]);
                            break;

                        case 3:
                            namedColor = XmlColor.FromArgb(numArray[0], numArray[1], numArray[2]);
                            break;

                        case 4:
                            namedColor = XmlColor.FromArgb(numArray[0], numArray[1], numArray[2], numArray[3]);
                            break;
                    }
                    flag = true;
                }
                if ((namedColor != null) && flag)
                {
                    int num2 = ((XmlColor) namedColor).ToArgb();
                    foreach (XmlColor color in Colors.Values)
                    {
                        if (color.ToArgb() == num2)
                        {
                            namedColor = color;
                            break;
                        }
                    }
                }
            }
            if (namedColor == null)
            {
                //throw new ArgumentException(SR.GetString("InvalidColor", new object[] { name }));
            }
            return namedColor;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if (value is XmlColor)
            {
                if (destinationType == typeof(string))
                {
                    string[] strArray;
                    XmlColor color = (XmlColor) value;
                    if (color == XmlColor.Empty)
                    {
                        return string.Empty;
                    }
                    if (color.IsKnownColor)
                    {
                        return color.Name;
                    }
                    if (color.IsNamedColor)
                    {
                        return ("'" + color.Name + "'");
                    }
                    if (culture == null)
                    {
                        culture = CultureInfo.CurrentCulture;
                    }
                    string separator = culture.TextInfo.ListSeparator + " ";
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
                    int num = 0;
                    if (color.A < 0xff)
                    {
                        strArray = new string[4];
                        strArray[num++] = converter.ConvertToString(context, culture, color.A);
                    }
                    else
                    {
                        strArray = new string[3];
                    }
                    strArray[num++] = converter.ConvertToString(context, culture, color.R);
                    strArray[num++] = converter.ConvertToString(context, culture, color.G);
                    strArray[num++] = converter.ConvertToString(context, culture, color.B);
                    return string.Join(separator, strArray);
                }
                if (destinationType == typeof(InstanceDescriptor))
                {
                    MemberInfo member = null;
                    object[] arguments = null;
                    XmlColor color2 = (XmlColor) value;
                    if (color2.IsEmpty)
                    {
                        member = typeof(XmlColor).GetField("Empty");
                    }
                    else if (color2.IsSystemColor)
                    {
                        member = typeof(SystemColors).GetProperty(color2.Name);
                    }
                    else if (color2.IsKnownColor)
                    {
                        member = typeof(XmlColor).GetProperty(color2.Name);
                    }
                    else if (color2.A != 0xff)
                    {
                        member = typeof(XmlColor).GetMethod("FromArgb", new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) });
                        arguments = new object[] { color2.A, color2.R, color2.G, color2.B };
                    }
                    else if (color2.IsNamedColor)
                    {
                        member = typeof(XmlColor).GetMethod("FromName", new Type[] { typeof(string) });
                        arguments = new object[] { color2.Name };
                    }
                    else
                    {
                        member = typeof(XmlColor).GetMethod("FromArgb", new Type[] { typeof(int), typeof(int), typeof(int) });
                        arguments = new object[] { color2.R, color2.G, color2.B };
                    }
                    if (member != null)
                    {
                        return new InstanceDescriptor(member, arguments);
                    }
                    return null;
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        private static void FillConstants(Hashtable hash, Type enumType)
        {
            MethodAttributes attributes = MethodAttributes.Static | MethodAttributes.Public;
            foreach (PropertyInfo info in enumType.GetProperties())
            {
                if (info.PropertyType == typeof(XmlColor))
                {
                    MethodInfo getMethod = info.GetGetMethod();
                    if ((getMethod != null) && ((getMethod.Attributes & attributes) == attributes))
                    {
                        object[] index = null;
                        hash[info.Name] = info.GetValue(null, index);
                    }
                }
            }
        }

        internal static object GetNamedColor(string name)
        {
            object obj2 = null;
            obj2 = Colors[name];
            if (obj2 != null)
            {
                return obj2;
            }
            return SystemColors[name];
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (values == null)
            {
                lock (ValuesLock)
                {
                    if (values == null)
                    {
                        ArrayList list = new ArrayList();
                        list.AddRange(Colors.Values);
                        list.AddRange(SystemColors.Values);
                        int count = list.Count;
                        for (int i = 0; i < (count - 1); i++)
                        {
                            for (int j = i + 1; j < count; j++)
                            {
                                if (list[i].Equals(list[j]))
                                {
                                    list.RemoveAt(j);
                                    count--;
                                    j--;
                                }
                            }
                        }
                        list.Sort(0, list.Count, new ColorComparer());
                        values = new TypeConverter.StandardValuesCollection(list.ToArray());
                    }
                }
            }
            return values;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        private static Hashtable Colors
        {
            get
            {
                if (colorConstants == null)
                {
                    lock (ColorConstantsLock)
                    {
                        if (colorConstants == null)
                        {
                            Hashtable hash = new Hashtable(StringComparer.OrdinalIgnoreCase);
                            FillConstants(hash, typeof(XmlColor));
                            colorConstants = hash;
                        }
                    }
                }
                return colorConstants;
            }
        }

        private static Hashtable SystemColors
        {
            get
            {
                if (systemColorConstants == null)
                {
                    lock (SystemColorConstantsLock)
                    {
                        if (systemColorConstants == null)
                        {
                            Hashtable hash = new Hashtable(StringComparer.OrdinalIgnoreCase);
                            FillConstants(hash, typeof(SystemColors));
                            systemColorConstants = hash;
                        }
                    }
                }
                return systemColorConstants;
            }
        }

        private class ColorComparer : IComparer
        {
            public int Compare(object left, object right)
            {
                XmlColor color = (XmlColor) left;
                XmlColor color2 = (XmlColor) right;
                return string.Compare(color.Name, color2.Name, false, CultureInfo.InvariantCulture);
            }
        }
    }

     
 

}
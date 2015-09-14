using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;//
using System.Text;
using System.Xml;

namespace Hwd.Serialization
{
    public delegate void DOnEncode(object instance, StringBuilder resultXml);

    /// <summary>
    /// Centralized serialization worker class.
    /// Supports Collections of mixed types and Dictionaries.
    /// Inferred types. Special characters.
    /// No object graph detection.
    /// </summary>
    public class Encoder
    {
        static Encoder()
		{
			string s;
			for ( int i = 0; i <= _maxDepth; i++ )
			{
				s = "";
				for ( int j = 0; j < i; j++ ) s += " ";
				_spaces[i] = s;
			}
		}

        public static string ItemsTagName = "SerializedItem";
        public static string ItemsListTagName = "SerializedItems";

        public bool Indent = true;
        public string Eol = "\r";
        // todo, public/published option.
        
        public Encoder()
        {
            _overrides.Add(typeof(DateTime), EncodeDateTime);
            _overrides.Add(typeof(Color), EncodeColor);
        }

        public string Encode(object o)
        {
            _resultXml.Length = 0;

            VisitObject(o, 0, null);

            return _resultXml.ToString();
        }

        public void AddOverride(Type t, DOnEncode onEncode)
        {
            if (t == null) return;

            if (_overrides.ContainsKey(t))
            {
                if (onEncode == null)
                    _overrides.Remove(t);
                else
                    _overrides[t] = onEncode;
            }
            else
            {
                if (onEncode != null)
                    _overrides.Add(t, onEncode);
            }
        }

        private StringBuilder _resultXml = new StringBuilder();
        private Dictionary<Type, DOnEncode> _overrides = new Dictionary<Type, DOnEncode>();

        private void AppendTagStart(string name, string typeName)
        {
            if (name == null) name = "Unknown";
            _resultXml.Append("<");
            _resultXml.Append(name);
            if (typeName != null)
            {
                _resultXml.Append(" type='");
                _resultXml.Append(typeName);
                _resultXml.Append("'");
            }
            _resultXml.Append(">");
        }

        private void AppendTagStart(string name, string typeName, int count)
        {
            if (name == null) name = "Unknown";
            _resultXml.Append("<");
            _resultXml.Append(name);
            if (typeName != null)
            {
                _resultXml.Append(" type='");
                _resultXml.Append(typeName);
                _resultXml.Append("'");
            }
            _resultXml.Append(" count='");
            _resultXml.Append(count);
            _resultXml.Append("'>");
        }

        private void AppendTagEnd(string name)
        {
            if (name == null) name = "Unknown";
            _resultXml.Append("</");
            _resultXml.Append(name);
            _resultXml.Append(">");
        }

        private void VisitTuple(int depth, object o, Type originalType)
        {
            // A special case VisitClass for dictionary support.
            _resultXml.Append(Eol);

            FieldInfo fi; PropertyInfo pi;
            GetInfo(originalType, "Key", out fi, out pi);          ///////////

            object value = GetFieldValue(fi, pi, o);
            VisitObject(value, depth + 1, "TupleKey");

            GetInfo(originalType, "Value", out fi, out pi);

            value = GetFieldValue(fi, pi, o);
            VisitObject(value, depth + 1, "TupleValue");

            _resultXml.Append(GetNSpaces(depth * 2));
        }

        private void VisitClass(int depth, object o, Type t)
        {
            _resultXml.Append(Eol);

            // Run over any members.
            MemberInfo[] ilist = FindMembers(t);
            if (ilist.Length > 0)
            {
                string[] parts;
                for (int j = 0; j < ilist.Length; j++)
                {
                    if (ilist[j].MemberType == MemberTypes.Field || ilist[j].MemberType == MemberTypes.Property)
                    {
                        // Get the name and the type of this member.
                        parts = ilist[j].ToString().Split(' ');
                        if (parts.Length == 2)
                        {
                            FieldInfo fi; PropertyInfo pi;
                            GetInfo(t, parts[1], out fi, out pi);

                            if (GetFieldCanWrite(fi, pi))
                            {
                                object value = GetFieldValue(fi, pi, o);
                                VisitObject(value, depth + 1, parts[1]);
                            }
                        }
                    }
                }
            }

            // Deal with collection items.
            if (o is ICollection)
            {
                ICollection collection = o as ICollection;

                _resultXml.Append(GetNSpaces(depth * 2 + 2));
                AppendTagStart(ItemsListTagName, "ICollection");

                _resultXml.Append(Eol);
                foreach (object item in collection)
                {
                    VisitObject(item, depth + 2, ItemsTagName);
                }
                
                _resultXml.Append(GetNSpaces(depth * 2 + 2));
                AppendTagEnd(ItemsListTagName);
                _resultXml.Append(Eol);
            }

            _resultXml.Append(GetNSpaces(depth * 2));
        }

        private void VisitObject(object o, int depth, string memberName)
        {
            if (o == null) return;
            Type t = o.GetType();

            // Special case
            Type originalType = null;
            if (t.FullName.StartsWith("System.Collections.Generic.KeyValuePair`2"))
            {
                originalType = t;
                Type keyType = t.GetGenericArguments()[0];
                Type valueType = t.GetGenericArguments()[1];
                Type memberType = Type.GetType("Hwd.Serialization.Tuple`2");
                t = memberType.MakeGenericType(new Type[2] { keyType, valueType });
            }

            _resultXml.Append(GetNSpaces(depth * 2));
            if (t != null && t.IsArray)
                AppendTagStart(memberName, GetTypeName(memberName, t), (o as IList).Count);
            else
                AppendTagStart(memberName, GetTypeName(memberName, t));

            if (t == typeof(System.String))
                AppendString(o as string);
            else if (t == typeof(System.Int16))
                _resultXml.Append(System.Convert.ToInt16(o).ToString());
            else if (t == typeof(System.Int32))
                _resultXml.Append(System.Convert.ToInt32(o).ToString());
            else if (t == typeof(System.Int64))
                _resultXml.Append(System.Convert.ToInt64(o).ToString());
            else if (t == typeof(System.UInt16))
                _resultXml.Append(System.Convert.ToUInt16(o).ToString());
            else if (t == typeof(System.UInt32))
                _resultXml.Append(System.Convert.ToUInt32(o).ToString());
            else if (t == typeof(System.UInt64))
                _resultXml.Append(System.Convert.ToUInt64(o).ToString());
            else if (t == typeof(System.Boolean))
                _resultXml.Append(System.Convert.ToBoolean(o).ToString());
            else if (t == typeof(System.Double))
                _resultXml.Append(System.Convert.ToDouble(o).ToString());
            else if (t == typeof(System.Single))
                _resultXml.Append(System.Convert.ToSingle(o).ToString());
            else if (t == Type.GetType("System.RuntimeType"))
                _resultXml.Append(GetTypeName(memberName, o as Type)); 
            else if (t == typeof(System.Object))
                _resultXml.Append(string.Empty);
            else if (t.IsGenericType && t.FullName.StartsWith("Hwd.Serialization.Tuple`2"))
            {
                // Special case: Kvp does not have writable Key/Value parts.
                VisitTuple(depth, o, originalType);
            }
            else if (t.IsClass || t.IsGenericType)
            {
                // Run over any members and/or collection items.
                VisitClass(depth, o, t);
            }

            else if (_overrides.ContainsKey(t))
            {
                // We could speed everything up by switching all types here.
                _overrides[t](o, _resultXml);
            }
            else throw new SerializationException("Unhandled object of type:" + t.FullName);


            AppendTagEnd(memberName);
            _resultXml.Append(Eol);
        }

        private string AppendString(string nonXmlText)
        {
            StringBuilder builder = new StringBuilder();
            Char[] originalChars = nonXmlText.ToCharArray();

            for (int i = 0; i < originalChars.Length; i++)
            {
                switch ((byte)originalChars[i])
                {
                    case 34:
                    case 38:
                    case 39:
                    case 60:
                    case 61:
                    case 62:
                        _resultXml.Append("&#");
                        _resultXml.Append(Convert.ToInt16(originalChars[i]));
                        _resultXml.Append(";");
                        break;

                    default:
                        _resultXml.Append(originalChars[i]);
                        break;
                }
            }

            return builder.ToString();
        }



        private static int _maxDepth = 1000;
        private static string[] _spaces = new string[_maxDepth + 1];

        private static string GetNSpaces(int depth)
        {
            if (depth > _maxDepth)
            {
                depth = _maxDepth;
            }

            return _spaces[depth];
        }

        private static bool DelegateToSearchCriteria(MemberInfo objMemberInfo, object objSearch)
        {
            return true;
        }
        
        private static MemberInfo[] FindMembers(Type t)
        {
            MemberTypes mt = MemberTypes.Field | MemberTypes.Property;
            BindingFlags bf = (BindingFlags.Public | BindingFlags.Instance);
            MemberFilter mf = new MemberFilter(DelegateToSearchCriteria);
            object fc = "ReferenceEquals";
            return t.FindMembers(mt, bf, mf, fc);
        }

        private static void GetInfo(Type t, string name, out FieldInfo fi, out PropertyInfo pi)
        {
            fi = t.GetField(name, BindingFlags.Public | BindingFlags.Instance);
            pi = null;

            if (fi == null)
            {
                pi = t.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            }
        }

        private static object GetFieldValue(FieldInfo fi, PropertyInfo pi, object o)
        {
            if (o == null) return null;

            if (fi != null)
            {
                return fi.GetValue(o);
            }
            else
            {
                return pi.GetValue(o, null);
            }
        }

        private static bool GetFieldCanWrite(FieldInfo fi, PropertyInfo pi)
        {
            if (fi != null)
            {
                return true;
            }
            else
            {
                return pi.CanWrite;
            }
        }

        private static string GetTypeName(string memberName, Type type)
        {
            string result = null;

            // If member name is null, or it's a generic collection item, 
            // then we can't infer the type on decode, so encode it explicitly.
            if (memberName == null || memberName == ItemsTagName)
            {
                if (type.Assembly.GetName().Name == "mscorlib")
                {
                    result = type.FullName;
                }
                else
                {
                    result = type.AssemblyQualifiedName;
                }
            }
            
            return result;
        }


        private static void EncodeDateTime(object o, StringBuilder sb)
        {
            // Format issues        
            sb.Append(System.Convert.ToDateTime(o).ToString());
        }

        private static void EncodeColor(object o, StringBuilder sb)
        {
            System.Drawing.Color c = (System.Drawing.Color)o;

            if (c.IsNamedColor)
                sb.Append(c.Name);
            else
                sb.Append(c.R + "," + c.G + "," + c.B + "," + c.A);
        }
    }
}

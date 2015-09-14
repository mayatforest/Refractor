using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;//
using System.Text;
using System.Xml;

namespace Hwd.Serialization
{
    public delegate object DOnDecode(XmlNodeReader reader);

    /// <summary>
    /// Centralized deserialization worker class.
    /// </summary>
    public class Decoder
    {
        public Decoder()
        {
            _overrides.Add(typeof(DateTime), DecodeDateTime);
            _overrides.Add(typeof(Color), DecodeColor);
        }

        public object Decode(string s)
        {
            if (string.IsNullOrEmpty(s)) return null;

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(s);

            _reader = new XmlNodeReader(xd);

            if (!_reader.Read()) return null;
            return VisitElement(null, null);
        }

        public object Decode(XmlNodeReader reader)
        {
            if (reader == null) return null;
            _reader = reader;

            if (!_reader.Read()) return null;
            return VisitElement(null, null);
        }

        public void AddOverride(Type t, DOnDecode onDecode)
        {
            if (t == null) return;

            if (_overrides.ContainsKey(t))
            {
                if (onDecode == null)
                    _overrides.Remove(t);
                else
                    _overrides[t] = onDecode;
            }
            else
            {
                if (onDecode != null)
                    _overrides.Add(t, onDecode);
            }
        }

        private XmlNodeReader _reader;
        private Dictionary<Type, DOnDecode> _overrides = new Dictionary<Type, DOnDecode>();

        private int GetCount()
        {
            string countStr = _reader.GetAttribute("count");
            if (string.IsNullOrEmpty(countStr)) return 0;
            else return int.Parse(countStr);
        }

        private object GetPrimitiveValue(Type t)
        {
            // Decode any primitive values from text.
            if (t == typeof(System.String))
                return System.Convert.ToString(_reader.Value);
            else if (t == typeof(System.Int16))
                return System.Convert.ToInt16(_reader.Value);
            else if (t == typeof(System.Int32))
                return System.Convert.ToInt32(_reader.Value);
            else if (t == typeof(System.Int64))
                return System.Convert.ToInt64(_reader.Value);
            else if (t == typeof(System.UInt16))
                return System.Convert.ToUInt16(_reader.Value);
            else if (t == typeof(System.UInt32))
                return System.Convert.ToUInt32(_reader.Value);
            else if (t == typeof(System.UInt64))
                return System.Convert.ToUInt64(_reader.Value);
            else if (t == typeof(System.Boolean))
                return System.Convert.ToBoolean(_reader.Value);
            else if (t == typeof(System.Double))
                return System.Convert.ToDouble(_reader.Value);
            else if (t == typeof(System.Single))
                return System.Convert.ToSingle(_reader.Value);
            else if (t == Type.GetType("System.RuntimeType"))
                return Type.GetType(_reader.Value);
            else if (_overrides.ContainsKey(t))
            {
                return _overrides[t](_reader);
            }
            else throw new SerializationException("Unhandled Text element:" + t.FullName);
        }

        private void VisitCollectionItems(Type parentType, object parentObject)
        {
            if (parentType.IsArray)
            {
                int idx = 0;
                while (_reader.NodeType != XmlNodeType.EndElement)
                {
                    (parentObject as Array).SetValue(
                        VisitElement(null, null), idx++);
                    if (!_reader.Read()) throw new SerializationException("Unexpected eof during Array set");
                }
            }
            else if (parentObject is IDictionary)
            {
                object collectionObject;
                Type collectionType;
                while (_reader.NodeType != XmlNodeType.EndElement)
                {
                    collectionObject = VisitElement(null, null);
                    collectionType = collectionObject.GetType();

                    // Special case
                    if (collectionType.FullName.StartsWith("Hwd.Serialization.Tuple`2"))
                    {
                        if ((collectionObject as ITuple).GetKey() == null) throw new SerializationException("Dictionary key is null");

                        (parentObject as IDictionary).Add(
                            (collectionObject as ITuple).GetKey(),
                            (collectionObject as ITuple).GetValue());

                    }
                    else throw new SerializationException("Unrecognised IDictionary type:" + collectionType.FullName);

                    if (!_reader.Read()) throw new SerializationException("Unexpected eof during IList set");
                }
            }
            else if (parentObject is ICollection)
            {
                while (_reader.NodeType != XmlNodeType.EndElement)
                {
                    object o = VisitElement(null, null);
                    (parentObject as IList).Add(o);
                    if (!_reader.Read()) throw new SerializationException("Unexpected eof during IList set");
                }
            }
            else throw new SerializationException("Unexpected collection type:" + parentType.FullName);
        }

        private object VisitElement(Type parentType, object parentObject)
        {
            if (_reader.NodeType != XmlNodeType.Element) throw new SerializationException("Expecting element found:" + _reader.NodeType);

            string name = _reader.Name;
            string typeName = _reader.GetAttribute("type");

            Type t = null;
            if (typeName == null)
            {
                if (parentType == null) throw new SerializationException("Expecting parent type to infer type of member:" + name);
                MemberInfo[] mis = parentType.GetMember(name);
                if (mis == null) throw new SerializationException("Didn't find member:" + name);
                if (mis.Length == 0) throw new SerializationException("Zero count, didn't find member:" + name);
                if (mis.Length > 1) throw new SerializationException("More than one member of this name:" + name);

                MemberInfo mi = mis[0];
                if (mi is PropertyInfo) t = (mi as PropertyInfo).PropertyType;
                else if (mi is FieldInfo) t = (mi as FieldInfo).FieldType;
                else throw new SerializationException("Field is not proeprty or field:" + name);                
            }
            else
            {
                t = Type.GetType(typeName);
            }

            object o = null;
            int count = (t != null && t.IsArray) ? GetCount() : 0;

            if (!_reader.Read()) throw new SerializationException("Unexpected eof");

            if (_reader.NodeType == XmlNodeType.Text)
            {
                o = GetPrimitiveValue(t);
                if (!_reader.Read()) throw new SerializationException("Unexpected eof during primitive conversion");
            }
            else if (_reader.NodeType == XmlNodeType.Element)
            {
                // Detect dictionary/list item by name.
                if (name == Encoder.ItemsListTagName)
                {
                    // Decode any collection items recursively.
                    // Adding them to the parent, which is the collection
                    VisitCollectionItems(parentType, parentObject);
                }
                else
                {
                    // Decode any members recursively.
                    o = CreateInstance(t, count);
                    while (_reader.NodeType != XmlNodeType.EndElement)
                    {
                        VisitElement(t, o);
                        if (!_reader.Read()) throw new SerializationException("Unexpected eof during members list");
                    }
                }
            }
            else if (_reader.NodeType == XmlNodeType.EndElement)
            {
                // Types with no members or collection itmes.
                if (t != null) 
                    o = CreateInstance(t, count);
            }


            if (_reader.NodeType != XmlNodeType.EndElement) throw new SerializationException("Expected EndElement found:" + _reader.NodeType);
            if (_reader.Name != name) throw new SerializationException("Unmatching end tag:" + _reader.Name);

            // Make sure we set the field, if we've been passed a parent.
            if ((o != null) && (parentObject != null))
            {
                SetFieldValue(parentType, name, parentObject, o);
            }

            return o;
        }


        private static void SetField(FieldInfo fi, PropertyInfo pi, object o, object v)
        {
            Type t = GetFieldType(fi, pi);

            //todo: constants
            if (t == System.Type.GetType("System.String"))
                SetFieldValue(fi, pi, o, System.Convert.ToString(v));
            else if (t == System.Type.GetType("System.Int16"))
                SetFieldValue(fi, pi, o, System.Convert.ToInt16(v));
            else if (t == System.Type.GetType("System.Int32"))
                SetFieldValue(fi, pi, o, System.Convert.ToInt32(v));
            else if (t == System.Type.GetType("System.Int64"))
                SetFieldValue(fi, pi, o, System.Convert.ToInt64(v));
            else if (t == System.Type.GetType("System.Boolean"))
                SetFieldValue(fi, pi, o, System.Convert.ToBoolean(v));
            else if (t == System.Type.GetType("System.Double"))
                SetFieldValue(fi, pi, o, System.Convert.ToDouble(v));
            else if (t == System.Type.GetType("System.Single"))
                SetFieldValue(fi, pi, o, System.Convert.ToSingle(v));
            else if (t == System.Type.GetType("System.Type"))               ///////////
                SetFieldValue(fi, pi, o, Type.GetType(v as string));
        }

        private static void SetFieldValue(FieldInfo fi, PropertyInfo pi, object o, object value)
        {
            if (fi != null)
            {
                fi.SetValue(o, value);
            }
            else
            {
                pi.SetValue(o, value, null);
            }
        }

        private static void SetFieldValue(Type t, string name, object o, object value)
        {
            FieldInfo fi = t.GetField(name, BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo pi = null;

            if (fi == null)
            {
                pi = t.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                pi.SetValue(o, value, null);
            }
            else
            {
                fi.SetValue(o, value);
            }
        }

        private static Type GetFieldType(FieldInfo fi, PropertyInfo pi)
        {
            if (fi != null)
            {
                return fi.FieldType;
            }
            else
            {
                return pi.PropertyType;
            }
        }

        private static object CreateInstance(Type t, int count)
        {
            if (t == typeof(string))
            {
                return string.Empty;
            }
            else if (t.IsArray)
            {
                return Activator.CreateInstance(t, new object[1] { count });
            }
            else
            {
                return Activator.CreateInstance(t); // must have default constructor
            }
        }


        private static object DecodeDateTime(XmlNodeReader reader)
        {
            // Format issues?
            return System.Convert.ToDateTime(reader.Value);
        }

        private static object DecodeColor(XmlNodeReader reader)
        {
            if (reader.Value.Contains(","))
            {
                int a, r, g, b;
                string[] bits = reader.Value.Split('.');
                int.TryParse(bits[0], out r);
                int.TryParse(bits[1], out g);
                int.TryParse(bits[2], out b);
                int.TryParse(bits[3], out a);
                return System.Drawing.Color.FromArgb(a, r, g, b);
            }
            else
            {
                return System.Drawing.Color.FromName(reader.Value);
            }
        }
    }
}

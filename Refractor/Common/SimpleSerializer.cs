using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Mtc.SimpleSerializer
{
	public class Serializer
	{
		protected MemoryStream ms;
		protected XmlTextWriter xtw;
		protected string xml;

		public string Xml
		{
			get {return xml;}
		}

		public void Start()
		{
			ms=new MemoryStream();
			xtw = new XmlTextWriter(ms, Encoding.UTF8);
			xtw.Formatting=Formatting.Indented;
			xtw.Namespaces=false;
			xtw.WriteStartDocument();
			xtw.WriteComment("Auto-Serialized");
			xtw.WriteStartElement("Objects");
		}

		public string Finish()
		{
			Trace.Assert(xtw != null, "Must call Serializer.Start() first.");

			xtw.WriteEndElement();
			xtw.Flush();
			xtw.Close();
			Encoding e8=new UTF8Encoding();
			xml=e8.GetString(ms.ToArray(), 1, ms.ToArray().Length-1);
			return xml;
		}

		// simple property serialization
		public void Serialize(object obj)
		{
			Trace.Assert(xtw != null, "Must call Serializer.Start() first.");
			Trace.Assert(obj != null, "Cannot serialize a null object.");

			Type t=obj.GetType();
			xtw.WriteStartElement(t.Name);
			foreach(PropertyInfo pi in t.GetProperties())
			{
				Type propertyType=pi.PropertyType;
				// with enum properties, IsPublic==false, even if marked public!
				if ( (propertyType.IsSerializable) && (!propertyType.IsArray) && (pi.CanWrite) && ( (propertyType.IsPublic) || (propertyType.IsEnum) ) )
				{
					object val=pi.GetValue(obj, null);
					if (val != null)
					{
						bool isDefaultValue=false;

						// look for a default value attribute.
						foreach(object attr in pi.GetCustomAttributes(false))
						{
							if (attr is DefaultValueAttribute)
							{
								// it exists--compare current value to default value
								DefaultValueAttribute dva=(DefaultValueAttribute)attr;
								isDefaultValue=val.Equals(dva.Value);
							}
						}

						// only non-default values or properties without a default value are serialized.
						if (!isDefaultValue)
						{
							// do a type conversion to a string, as this yields a deserializable value, rather than what ToString returns.
							TypeConverter tc=TypeDescriptor.GetConverter(propertyType);
							if (tc.CanConvertTo(typeof(string)))
							{
								val=tc.ConvertTo(val, typeof(string));
								xtw.WriteAttributeString(pi.Name, val.ToString());
							}
							else
							{
								Trace.WriteLine("Cannot convert "+pi.Name+" to a string value.");
							}
						}
					}
					else
					{
						// null values not supported!
					}
				}
			}
			xtw.WriteEndElement();
		}
	}

	public class Deserializer
	{
		protected XmlDocument doc;

		public void Start(string text)
		{
			doc=new XmlDocument();
			doc.LoadXml(text);
		}

		// for completeness only
		public void Finish()
		{
		}

		// simple property deserialization
		public void Deserialize(object obj, int idx)
		{
			Trace.Assert(doc != null, "Must call Deserializer.Start() first.");
			Trace.Assert(doc.ChildNodes.Count==3, "Incorrect xml format.");
			Trace.Assert(idx < doc.ChildNodes[2].ChildNodes.Count, "No element for the specified index.");
			Trace.Assert(obj != null, "Cannot deserialize to a null object");

			// skip the encoding and comment, and get the indicated child in the Objects tag
			XmlNode node=doc.ChildNodes[2].ChildNodes[idx];
			Type t=obj.GetType();
			Trace.Assert(t.Name==node.Name, "Object name does not match element tag.");

			// set all properties that have a default value and not overridden.
			foreach(PropertyInfo pi in t.GetProperties())
			{
				Type propertyType=pi.PropertyType;

				// look for a default value attribute.
				foreach(object attr in pi.GetCustomAttributes(false))
				{
					if (attr is DefaultValueAttribute)
					{
						// it has a default value
						DefaultValueAttribute dva=(DefaultValueAttribute)attr;
						if (node.Attributes[pi.Name] == null)
						{
							// assign the default value, as it's not being overridden.
							// this reverts the object's property back to the default
							pi.SetValue(obj, dva.Value, null);
						}
					}
				}
			}

			// now parse the xml attributes that are going to change property values
			foreach(XmlAttribute attr in node.Attributes)
			{
				string pname=attr.Name;
				string pvalue=attr.Value;
				PropertyInfo pi=t.GetProperty(pname);
				if (pi != null)
				{
					TypeConverter tc=TypeDescriptor.GetConverter(pi.PropertyType);
					if (tc.CanConvertFrom(typeof(string)))
					{
						try
						{
							object val=tc.ConvertFrom(pvalue);
							pi.SetValue(obj, val, null);
						}
						catch(Exception e)
						{
							Trace.WriteLine("Setting "+pname+" failed:\r\n"+e.Message);
						}
					}
				}
			}
		}
	}
}

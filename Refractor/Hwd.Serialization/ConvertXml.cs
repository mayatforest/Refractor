using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Hwd.Serialization
{
    public class SerializationException : Exception
    {
        public SerializationException(string message) : base(message) { }
    }
    
    /// <summary>
    /// Centralized serialization static wrapper class.
    /// </summary>
    public class ConvertXml
	{
        public static string EncodeNative(object instance)
        {
            string result = null;

            TextWriter writer = null;

            using (writer = new StringWriter())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(instance.GetType());

                xmlSerializer.Serialize(writer, instance);

                writer.Flush();
            }

            if (writer != null) result = writer.ToString();

            return result;
        }
        
        public static object DecodeNative(string value, Type type)
        {
            // we have to know the type we're deserializing to do this..
            object result = null;

            if (value != null)
            {
                using (TextReader reader = new StringReader(value))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(type);

                    result = xmlSerializer.Deserialize(reader);
                }
            }

            return result;
        }

        public static string Encode(object instance)
        {
            Encoder c = new Encoder();
            return c.Encode(instance);
        }
        
        public static object Decode(string value)
        {
            Decoder c = new Decoder();
            return c.Decode(value);
        }

    }
}

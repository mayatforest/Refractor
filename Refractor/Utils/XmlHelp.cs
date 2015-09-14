using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Refractor
{
    public static class XmlHelp
    {
        public static void Save(string filename, object value)
        {
            using (XmlTextWriter writer = new XmlTextWriter(filename, Encoding.ASCII))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(value.GetType());

                xmlSerializer.Serialize(writer, value);
            }
        }

        public static object Load(string filename, Type type)
        {
            object result = null;

            // If we use a standard File.Open stream here, we get access denied failures
            // when asp/net tries to open the file, which is located above the main
            // bin folder. Use the XmlTextReader instead, which is not a stream.

            using (XmlTextReader reader = new XmlTextReader(filename))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(type);

                result = xmlSerializer.Deserialize(reader);
            }

            return result;
        }

    }
}

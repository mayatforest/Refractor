using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Refractor.Common
{
    public class RefHelp
    {
        /// <summary>
        /// Get unique ID string for method.
        /// </summary>
        /// <param name="mi"></param>
        /// <returns></returns>
        public static string GetNameWithParameterList(MethodInfo mi)
        {
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {                
                writer.Write(mi.ReturnType.FullName);
                writer.Write(" ");
                writer.Write(mi.Name);
                writer.Write("(");

                ParameterInfo[] parameters = mi.GetParameters();
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (i != 0)
                    {
                        writer.Write(", ");
                    }

                    writer.Write(parameters[i].ParameterType.ToString());
                }

                if (mi.CallingConvention == CallingConventions.VarArgs)
                {
                    if (parameters.Length > 0)
                    {
                        writer.Write(", ");
                    }

                    writer.Write("...");
                }

                writer.Write(")");


                // Static methods can have same name and sig as instance methods!
                if (mi.IsStatic)
                {
                    writer.Write(" static");

                }

                return writer.ToString();
            }
        }

        public static string GetNameWithParameterList(ConstructorInfo mi)
        {
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                writer.Write(mi.Name);
                writer.Write("(");

                ParameterInfo[] parameters = mi.GetParameters();
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (i != 0)
                    {
                        writer.Write(", ");
                    }

                    writer.Write(parameters[i].ParameterType.ToString());
                }

                if (mi.CallingConvention == CallingConventions.VarArgs)
                {
                    if (parameters.Length > 0)
                    {
                        writer.Write(", ");
                    }

                    writer.Write("...");
                }

                writer.Write(")");

                return writer.ToString();
            }
        }


    }

}

using System;
using System.IO;
using System.Reflection;

namespace Ss.RealEstate.Library2
{
    internal class Utility
    {
        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }

        public static uint GetUnsignedIntFromString(string yourString)
        {
            uint value = 0;
            foreach (char c in yourString)
            {
                if ((c >= '0') && (c <= '9'))
                {
                    value = Convert.ToUInt32(value * 10 + (c - '0'));
                }
            }

            return value;
        }

        public static double GetDoubleFromString(string yourString)
        {
            string valueStr = string.Empty;
            foreach (char c in yourString)
            {
                if (((c >= '0') && (c <= '9')) || (c == '.'))
                {
                    valueStr += c;
                }
            }

            return string.IsNullOrEmpty(valueStr) ? 0.0 : Convert.ToDouble(valueStr);
        }

        static public string CurrentAssemblyDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace Sentiment.Analyzer
{
    internal static class ResourceHelper
    {
        public static readonly string TempDirectory = Path.GetTempPath();

        public static string ExtractResource(string resourceName)
        {
            string directory = Path.GetDirectoryName(TempDirectory);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var path = Path.Combine(TempDirectory, resourceName);
            byte[] data = ResourceHelper.ReadResource(resourceName);
            File.WriteAllBytes(path, data);

            return path;
        }

        public static byte[] ReadResource(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (var memoryStream = new MemoryStream())
            {
                assembly.GetManifestResourceStream(resourceName).CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static string ReadResourceString(string resourceName)
        {
            var bytes = ReadResource(resourceName);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string WriteToTemp(string filename, string data)
        {
            var path = Path.Combine(TempDirectory, filename);

            File.WriteAllText(Path.Combine(TempDirectory, filename), data);

            return path;
        }

        public static string WriteToTemp(string filename, IEnumerable<string> data)
        {
            var path = Path.Combine(TempDirectory, filename);

            File.WriteAllLines(path, data.ToArray());

            return path;
        }
    }
}

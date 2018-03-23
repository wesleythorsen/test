using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace Sentiment.Analyzer
{
    internal class ExeHelper
    {
        private List<string> _Data;

        public string[] Output
        {
            get
            {
                return _Data.ToArray();
            }
        }


        public string Run(string exePath, string argStr)
        {
            Process build = new Process();
            build.StartInfo.WorkingDirectory = Path.GetDirectoryName(exePath);
            build.StartInfo.Arguments = argStr;
            build.StartInfo.FileName = exePath;

            build.StartInfo.UseShellExecute = false;
            build.StartInfo.RedirectStandardOutput = true;
            build.StartInfo.RedirectStandardError = true;
            build.StartInfo.CreateNoWindow = true;
            build.ErrorDataReceived += (s, e) => _Data.Add(e.Data);
            build.OutputDataReceived += (s, e) => _Data.Add(e.Data);
            build.EnableRaisingEvents = true;

            _Data = new List<string>();

            build.Start();
            build.BeginOutputReadLine();
            build.BeginErrorReadLine();
            build.WaitForExit();

            return string.Join("\n", _Data);
        }
    }
}

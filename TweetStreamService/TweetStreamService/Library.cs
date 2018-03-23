using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TweetStreamService
{
    public static class Library
    {
        public static void WriteErrorLog(Exception ex)
        {
            WriteErrorLog(ex.Source + ": " + ex.Message);
        }

        public static void WriteErrorLog(string Message)
        {
            try
            {
                using (var sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "errorlog.txt"), true))
                {
                    sw.WriteLine(DateTime.Now.ToString() + ": " + Message);
                }
            }
            catch { }
        }

        public static Exception GetInnermostException(this Exception ex)
        {
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            return ex;
        }
    }
}

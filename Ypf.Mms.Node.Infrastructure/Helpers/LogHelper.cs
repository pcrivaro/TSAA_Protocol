
using System;
using System.IO;

namespace Ypf.Mms.Node.Infrastructure.Helpers
{
    public static class LogHelper
    {
        public static object _sync = new object();

        public static void Log(string str)
        {
            lock (_sync)
            {
                File.AppendAllText("log.txt", str);
                File.AppendAllText("log.txt", Environment.NewLine);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ypf.Mms.Node.Runner
{
    class Program
    {
        private static NodeManager _manager = new NodeManager();

        static void Main(string[] args)
        {
            _manager.Start();

            while( Console.ReadLine().ToUpper() !="EXIT")
            {
            }

            _manager.Stop();
        }
    }
}

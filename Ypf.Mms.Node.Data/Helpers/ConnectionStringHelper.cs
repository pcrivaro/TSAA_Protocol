using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ypf.Mms.Node.Data.Helpers
{
    public static class ConnectionStringHelper
    {

        public static string Build()
        {
            return $"Server={System.Configuration.ConfigurationManager.AppSettings["DataBaseServer"]};Database=mms;Uid=mms;Pwd=mms2018!;Pooling=True;";
        }

    }
}

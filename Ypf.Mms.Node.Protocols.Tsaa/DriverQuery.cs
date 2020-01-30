
using System.Collections.Generic;
using Ypf.Mms.Node.Protocols.Tsaa.Enumerations;

namespace Ypf.Mms.Node.Protocols.Tsaa
{
    public class DriverQuery
    {
        public BinTypes BinType { get; set; }
        public ushort Min { get; set; }
        public ushort Max { get; set; }
        public List<string> VariableNames { get; set; }

        public ushort Count
        {
            get
            {
                var c = (ushort)(Max - Min + 1);

                if (c < 0)
                    return 0;
                else
                    return c;
            }
        }

    }
}

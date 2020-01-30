using System.Collections.Generic;
using Ypf.Mms.Node.Core.Enumerations;

namespace Ypf.Mms.Node.Core
{
    public class Device : BaseEntity
    {
        public string SerialPort { get; set; }
        public string IPAddress { get; set; }
        public int Port{ get; set; }
        public DeviceTypes DeviceType { get; set; }
        public ConnectionTypes ConnectionType { get; set; }
        public int PoolingInterval { get; set; }

        public virtual Driver Driver { get; set; }

        public string DriverData { get; set; }

        public virtual List<Item> Items { get; set; }
    }
}

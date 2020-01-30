
using System.Collections.Generic;

namespace Ypf.Mms.Node.Core
{
    public class Node : BaseEntity
    {
        public bool Reload { get; set; }

        public int? CheckInterval { get; set; }

        public virtual List<Device> Devices { get; set; }
    }
}

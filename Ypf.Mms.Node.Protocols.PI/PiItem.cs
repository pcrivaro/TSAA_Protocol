
using Ypf.Mms.Node.Core;

namespace Ypf.Mms.Node.Protocols.PI
{
    public class PiItem
    {
        public string FullName { get; set; }
        public string Server { get; set; }
        public string PointName { get; set; }
        public Item Item { get; set; }
    }
}

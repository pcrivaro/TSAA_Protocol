
using Ypf.Mms.Node.Protocols.Tsaa.Enumerations;

namespace Ypf.Mms.Node.Protocols.Tsaa.Core
{
    public class MessageHeader
    {
        public MessageTypes MessageType { get; set; }
        public byte NodeNumber { get; set; }
        public byte SeqNumber { get; set; }
        public byte Version { get; set; }
        public byte Flag { get; set; }
        public byte Id { get; set; }
        public ushort Length{ get; set; }
    }
}

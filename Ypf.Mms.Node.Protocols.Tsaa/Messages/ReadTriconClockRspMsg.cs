using System;
using Ypf.Mms.Node.Protocols.Tsaa.Core;
using Ypf.Mms.Node.Protocols.Tsaa.Enumerations;

namespace Ypf.Mms.Node.Protocols.Tsaa.Messages
{
    public class ReadTriconClockRspMsg : Message
    {
        private DateTime _datetime = new DateTime(1970, 1, 1, 0, 0, 0);

        public ResponseCodes ResponseCode { get; set; }
        public byte SubReason { get; set; }
        public byte[] Rfu { get; set; }
        public int RelSec { get; set; }
        public short MilliSec { get; set; }
        public byte[] Rfu2 { get; set; }


        public ReadTriconClockRspMsg(Message message) : base(message)
        {
        }

        public DateTime DateTime
        {
            get
            {
                return _datetime.AddSeconds(RelSec).AddMilliseconds(MilliSec);
            }
        }
    }
}

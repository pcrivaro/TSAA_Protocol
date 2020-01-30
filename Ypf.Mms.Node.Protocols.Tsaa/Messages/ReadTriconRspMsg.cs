using System.Collections;
using System.Collections.Generic;
using Ypf.Mms.Node.Protocols.Tsaa.Core;
using Ypf.Mms.Node.Protocols.Tsaa.Enumerations;

namespace Ypf.Mms.Node.Protocols.Tsaa.Messages
{   
    public class ReadTriconRspMsg : Message
    {
        public class BinData
        {
            private BitArray _boolArray;

            public BinTypes Bin { get; set; }
            public byte Rfu { get; set; }
            public ushort Offset { get; set; }
            public int RelSec { get; set; }
            public ushort MilliSec { get; set; }
            public ushort NumberOfValues { get; set; }            
            public byte[] Data { get; set; }

            public BitArray BoolArray
            {
                get
                {

                    if (_boolArray == null && Data != null && Data.Length > 0)
                    {
                        _boolArray = new BitArray(Data);
                    }

                    return _boolArray;
                }
            }
        }



        public byte ResponseCode { get; set; }
        public byte SubReason { get; set; }
        public short NumberOfBlocks { get; set; }
        public List<BinData> Bins { get; set; }


        public ReadTriconRspMsg(Message message) : base(message)
        {
            Bins = new List<BinData>();
        }

    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using Ypf.Mms.Node.Protocols.Tsaa.Core;
using Ypf.Mms.Node.Protocols.Tsaa.Enumerations;

namespace Ypf.Mms.Node.Protocols.Tsaa.Messages
{
    public class ReadTriconDataMsg : Message
    {
        public class BinData
        {
            public BinData()
            {
                Rfu = new byte[3] { 0, 0, 0 };
            }

            public BinTypes BinNumber { get; set; }
            public byte[] Rfu { get; set; }
            public ushort NumberOfValues { get; set; }
            public ushort Offset { get; set; }
        }


        public byte[] Rfu { get; set; }
        public List<BinData> Bins { get; set; }


        public ReadTriconDataMsg()
        {
            Header.MessageType = MessageTypes.READ_TRICON_DATA;

            Rfu = new byte[2] { 0, 0 };

            Bins = new List<BinData>();
        }


        public byte[] GetBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange((BitConverter.GetBytes((short)Bins.Count)).Reverse().ToArray());

            bytes.AddRange(Rfu.Reverse().ToArray());

            foreach (var bin in Bins)
            {
                bytes.Add((byte)bin.BinNumber);

                bytes.AddRange(bin.Rfu.Reverse().ToArray());

                bytes.AddRange((BitConverter.GetBytes(bin.Offset)).Reverse().ToArray());

                bytes.AddRange((BitConverter.GetBytes(bin.NumberOfValues)).Reverse().ToArray());
            }

            bytes.Reverse();

            return bytes.ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Ypf.Mms.Node.Protocols.Tsaa.Core;
using Ypf.Mms.Node.Protocols.Tsaa.Enumerations;

namespace Ypf.Mms.Node.Protocols.Tsaa.Messages
{
    public enum BinsRequestedTypes
    {
        MasksAllBins = 0x1fff,
        MasksDiscreteOutput = 0x0001,
        MasksReadWriteDiscreteMemory = 0x0002,
        MasksDiscreteInput = 0x0004,
        MasksReadOnlyDiscreteMemory = 0x0008,
        MasksAnalogInput = 0x0010,
        MasksReadOnlyIntegerMemory = 0x0020,
        MasksRealInput = 0x0040,
        MasksReadOnlyRealMemory = 0x0080,
        MasksDiscreteSystemStatus = 0x0100,
        MasksIntegerSystemStatus = 0x0200,
        MasksAnalogOutput = 0x0400,
        MasksReadWriteIntegerMemory = 0x0800,
        MasksReadWriteRealMemory = 0x1000
    }


    public class TriconDataReqMsg : Message
    {
        public BinsRequestedTypes BinsRequested { get; set; }
        public short ReqTime { get; set; }
        
        public TriconDataReqMsg()
        {
            Header.MessageType = MessageTypes.TRICON_DATA_REQ;
        }

        public byte [] GetBytes()
        {
            List<byte> bytes = new List<byte>();
            
            bytes.AddRange((BitConverter.GetBytes((short)BinsRequested)).Reverse().ToArray());

            bytes.AddRange((BitConverter.GetBytes(ReqTime)).Reverse().ToArray());

            bytes.Reverse();

            return bytes.ToArray();
        }
    }
}

using Ypf.Mms.Node.Protocols.Tsaa.Core;
using Ypf.Mms.Node.Protocols.Tsaa.Enumerations;

namespace Ypf.Mms.Node.Protocols.Tsaa.Messages
{
    public class CpStatus
    {
        public byte[] Rfu { get; set; }
        public byte SubReason{ get; set; }
        public ResponseCodes ResponseCode { get; set; }
    }

    public class CpStat
    {
        public byte LoadInProgress { get; set; }
        public byte CpLoadState { get; set; }
        public byte ModInProgress { get; set; }
        public byte CpSingleScan { get; set; }
        public byte CpValid { get; set; }
        public byte CpKeySwitch { get; set; }
        public byte CpRunState { get; set; }
        public byte SxSubVersion { get; set; }
        public string StartOfInternals { get; set; }
        public string StartOfMY { get; set; }
        public string StartOfUS { get; set; }
        public string StartOfDS { get; set; }
        public string FirstCPPage { get; set; }
        public string LastCPPage { get; set; }
        public ushort SizeOfMY { get; set; }
        public ushort SizeOfUS { get; set; }
        public ushort SizeOfDS { get; set; }
        public ushort SizeOfMYdfA { get; set; }
        public ushort SizeOfMYdfB { get; set; }
        public ushort SizeOfMYdfC { get; set; }
        public ushort SizeOfInternals { get; set; }
        public ushort ConfigSize { get; set; }
        public ushort CpScanTime { get; set; }
        public ushort ActualScanTime { get; set; }
        public short AvgTimeAvail { get; set; }
        public ushort TsxVersion { get; set; }
        public string CpVersion  { get; set; }
        public uint CpDwnLdTime { get; set; }
        public string ProgramName { get; set; }
        public ushort SxVendorCode { get; set; }
        public ushort PtsDisabled { get; set; }
        public byte AlarmStatus { get; set; }
        public byte NumberSoes { get; set; }
        public byte SoeState { get; set; }
    }


    public class TriconCpStatusMsg : Message
    {
        public short NumberOfBlocks { get; set; }
        public byte[] Rfu { get; set; }
        public CpStatus CpStatus { get; set; }
        public CpStat CpStat { get; set; }


        public TriconCpStatusMsg(Message message) : base(message)
        {
            CpStat = new CpStat();
            CpStatus = new CpStatus();
        }      

    }
}


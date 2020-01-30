using Ypf.Mms.Node.Protocols.Tsaa.Core;
using Ypf.Mms.Node.Protocols.Tsaa.Enumerations;

namespace Ypf.Mms.Node.Protocols.Tsaa.Messages
{
    public class TriconCpStatusReqMsg : Message
    {
        public TriconCpStatusReqMsg()
        {
            Header.MessageType = MessageTypes.TRICON_CPSTATUS_REQ;
        }
    }
}

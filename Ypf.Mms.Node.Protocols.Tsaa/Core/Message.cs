
namespace Ypf.Mms.Node.Protocols.Tsaa.Core
{
    public class Message
    {
        public Message()
        {
            Header = new MessageHeader();
        }

        public Message(Message msg) 
        {
            Header = msg.Header;
            Data = msg.Data;
        }

        public MessageHeader Header { get; set; }
        public byte[] Data { get; set; }
    }
}

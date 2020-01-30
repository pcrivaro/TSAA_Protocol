
using System;
using System.Net;
using System.Net.Sockets;
using Ypf.Mms.Node.Protocols.Tsaa.Core;
using System.Collections.Generic;
using Ypf.Mms.Node.Protocols.Tsaa.Helpers;
using Ypf.Mms.Node.Protocols.Tsaa.Messages;
using System.Linq;
using Ypf.Mms.Node.Protocols.Tsaa.Enumerations;
using System.Text;
using static Ypf.Mms.Node.Protocols.Tsaa.Messages.ReadTriconRspMsg;
using Ypf.Mms.Node.Infrastructure.Helpers;

namespace Ypf.Mms.Node.Protocols.Tsaa
{
    public class TsaaManager
    {
        private UdpClient _client;
        public event EventHandler<Message> ReceivedMessage;
        private IPEndPoint _endPoint;

        private Dictionary<MessageTypes, Message> _messages = new Dictionary<MessageTypes, Message>();


        public bool IsConnected { get; set; }
        public bool IsStoppedPooling{ get; set; }


        public void Connect(IPEndPoint endPoint)
        {
            _endPoint = endPoint;

            _client = new UdpClient();
            _client.Connect(endPoint);

            IsConnected = true;
            IsStoppedPooling = false;

            _client.BeginReceive(DataReceived, _client);
        }


        private void DataReceived(IAsyncResult ar)
        {
            UdpClient client = (UdpClient)ar.AsyncState;

            try
            {
                Byte[] receivedBytes = client.EndReceive(ar, ref _endPoint);

                byte[] crc = new byte[4];

                int dataLength = receivedBytes.Length - crc.Length;

                Array.Copy(receivedBytes, dataLength, crc, 0, crc.Length);

                var calculatedCrc = Crc32Helper.ComputeChecksumBytes(receivedBytes.Take(dataLength).ToArray());

                if (!calculatedCrc.SequenceEqual(crc))
                    return;

                var message = new Message();

                message.Header.MessageType = (MessageTypes)receivedBytes[0];
                message.Header.NodeNumber = receivedBytes[1];
                message.Header.SeqNumber = receivedBytes[2];
                message.Header.Version = receivedBytes[3];
                message.Header.Flag = receivedBytes[4];
                message.Header.Id = receivedBytes[5];

                ///
                //Revisar si hay que invertir el length siguiente
                //
                //var ln = dataLength;
                //var g = receivedBytes.Length;

                //var t = BitConverter.ToInt16(new byte[] { receivedBytes[7], receivedBytes[6] }, 0);
                //var h = BitConverter.ToInt16(new byte[] { receivedBytes[6], receivedBytes[7] }, 0);

                if (message.Header.MessageType == MessageTypes.READ_TRICON_RSP)
                {
                    message.Header.Length = BitConverter.ToUInt16(new byte[] { receivedBytes[6], receivedBytes[7] }, 0);
                }
                else
                {
                    message.Header.Length = BitConverter.ToUInt16(new byte[] { receivedBytes[7], receivedBytes[6] }, 0);
                }

                message.Data = receivedBytes.Skip(8).Take(message.Header.Length - 8).ToArray();

                var msg = ParseMessage(message);

                if (msg == null)
                    return;

                ThrowMessage(msg);
            }
            catch (Exception exc)
            {
            }

            if (IsConnected && !IsStoppedPooling)
            {   
                client.BeginReceive(DataReceived, ar.AsyncState);
            }
        }



        private void ThrowMessage(Message message)
        {
            //if (message is TriconDataMsg tr) {
            //    g += string.Join(Environment.NewLine, tr.Bins.Select(x => "Bin: "  + x.Bin.ToString() + "  Length: " + x.Length.ToString()  + "  Offset: " + x.Offset.ToString()));
            //    g += Environment.NewLine;
            //}

            LogHelper.Log("message.Header.Flag=" + message.Header.Flag);

            switch (message.Header.Flag)
            {

                case 1:
                case 0:

                    switch (message.Header.MessageType)
                    {
                        case MessageTypes.TRICON_DATA:

                            var msg = (TriconDataMsg)message;

                            switch (message.Header.Flag)
                            {
                                case 1:

                                    if (!_messages.ContainsKey(MessageTypes.TRICON_DATA))
                                    {
                                        _messages.Add(msg.Header.MessageType, msg);
                                    }
                                    else
                                    {
                                        Compete(msg);

                                        var storeMessage = _messages[MessageTypes.TRICON_DATA];

                                        _messages.Remove(MessageTypes.TRICON_DATA);

                                        ReceivedMessage?.Invoke(_client, storeMessage);
                                    }

                                    break;

                                case 0:

                                    Compete(msg);

                                    break;
                            }

                            break;
                    }

                    break;

                default:

                    LogHelper.Log("ReceivedMessage?.Invoke(_client, message)");

                    ReceivedMessage?.Invoke(_client, message);

                    break;
            }
        }


        private void Compete(TriconDataMsg msg)
        {
            var storedMessage = _messages[msg.Header.MessageType] as TriconDataMsg;

            if (storedMessage == null)
                return;

            foreach (var bin in msg.Bins)
            {
                var storedBin = storedMessage.Bins.SingleOrDefault(x => x.Bin == bin.Bin);

                if (storedBin == null)
                {
                    storedMessage.Bins.Add(bin);
                }
                else
                {
                    var list = new List<byte>(storedBin.Data);

                    list.AddRange(bin.Data);

                    storedBin.Data = list.ToArray();
                }
            }
        }


        private Message ParseMessage(Message message)
        {
            Message msg = null;

            if (message == null)
                return msg;

            switch (message.Header.MessageType)
            {
                case MessageTypes.TRICON_DATA:

                    LogHelper.Log("MessageTypes.TRICON_DATA");

                    var triconDataMsg = new TriconDataMsg(message)
                    {
                        NumberOfBlocks = BitConverter.ToInt16(new byte[] { message.Data[1], message.Data[0] }, 0),
                        Rfu = new byte[] { message.Data[3], message.Data[2] }
                    };

                    var data = message.Data.Skip(4).ToArray();

                    for (var i = 0; i < triconDataMsg.NumberOfBlocks; i++)
                    {
                        var totalBinLength = BitConverter.ToInt16(new byte[] { data[3], data[2] }, 0);

                        var binHeader = new BinHeader
                        {
                            Bin = (BinTypes)data[0],
                            Rfu = data[1],
                            TotalLength = totalBinLength,
                            Offset = BitConverter.ToInt16(new byte[] { data[5], data[4] }, 0),
                            Length = BitConverter.ToInt16(new byte[] { data[7], data[6] }, 0)
                        };

                        binHeader.Data = new byte[binHeader.Length];

                        Array.Copy(data, 8, binHeader.Data, 0, binHeader.Length);

                        data = data.Skip(binHeader.Length + 8).ToArray();

                        triconDataMsg.Bins.Add(binHeader);
                    }

                    msg = triconDataMsg;

                    break;


                case MessageTypes.READ_TRICON_CLOCK_RSP:

                    LogHelper.Log("MessageTypes.READ_TRICON_CLOCK_RSP");

                    var readTriconClockRspMsg = new ReadTriconClockRspMsg(message);

                    readTriconClockRspMsg.ResponseCode = (ResponseCodes)message.Data[0];
                    readTriconClockRspMsg.SubReason = message.Data[1];
                    readTriconClockRspMsg.Rfu = new byte[] { message.Data[3], message.Data[2] };
                    readTriconClockRspMsg.RelSec = BitConverter.ToInt32(new byte[] { message.Data[7], message.Data[6], message.Data[5], message.Data[4] }, 0);
                    readTriconClockRspMsg.MilliSec = BitConverter.ToInt16(new byte[] { message.Data[9], message.Data[8] }, 0);
                    readTriconClockRspMsg.Rfu = new byte[] { message.Data[11], message.Data[10] };

                    msg = readTriconClockRspMsg;

                    break;


                case MessageTypes.TRICON_CPSTATUS_RSP:

                    LogHelper.Log("MessageTypes.TRICON_CPSTATUS_RSP");

                    var triconCpStatusMsg = new TriconCpStatusMsg(message);

                    triconCpStatusMsg.CpStatus.ResponseCode = (ResponseCodes)message.Data[0];
                    triconCpStatusMsg.CpStatus.SubReason = message.Data[1];
                    triconCpStatusMsg.Rfu = new byte[] { message.Data[3], message.Data[2] };
                    triconCpStatusMsg.CpStat.LoadInProgress = message.Data[4];
                    triconCpStatusMsg.CpStat.ModInProgress = message.Data[5];
                    triconCpStatusMsg.CpStat.CpLoadState = message.Data[6];
                    triconCpStatusMsg.CpStat.CpSingleScan = message.Data[7];
                    triconCpStatusMsg.CpStat.CpValid = message.Data[8];
                    triconCpStatusMsg.CpStat.CpKeySwitch = message.Data[9];
                    triconCpStatusMsg.CpStat.CpRunState = message.Data[10];
                    triconCpStatusMsg.CpStat.SxSubVersion = message.Data[11];
                    triconCpStatusMsg.CpStat.CpScanTime = BitConverter.ToUInt16(new byte[] { message.Data[52], message.Data[53] }, 0);
                    triconCpStatusMsg.CpStat.ActualScanTime = BitConverter.ToUInt16(new byte[] { message.Data[54], message.Data[55] }, 0);
                    triconCpStatusMsg.CpStat.CpVersion = string.Format("{0}.{1}", message.Data[62], message.Data[60]);
                    triconCpStatusMsg.CpStat.CpDwnLdTime = BitConverter.ToUInt32(new byte[] { message.Data[64], message.Data[65], message.Data[66], message.Data[67] }, 0);

                    var programNameBytes = message.Data.Skip(68).Take(10);
                    triconCpStatusMsg.CpStat.ProgramName = Encoding.ASCII.GetString(programNameBytes.ToArray());

                    triconCpStatusMsg.CpStat.SxVendorCode = BitConverter.ToUInt16(new byte[] { message.Data[79], message.Data[80] }, 0);
                    triconCpStatusMsg.CpStat.PtsDisabled = BitConverter.ToUInt16(new byte[] { message.Data[81], message.Data[82] }, 0);
                    triconCpStatusMsg.CpStat.AlarmStatus = message.Data[83];
                    triconCpStatusMsg.CpStat.NumberSoes = message.Data[84];
                    triconCpStatusMsg.CpStat.SoeState = message.Data[85];

                    msg = triconCpStatusMsg;

                    break;


                case MessageTypes.READ_TRICON_RSP:

                    LogHelper.Log("MessageTypes.READ_TRICON_RSP");

                    var readTriconRspMsg = new ReadTriconRspMsg(message)
                    {
                        ResponseCode = message.Data[0],
                        SubReason = message.Data[1],
                        NumberOfBlocks = BitConverter.ToInt16(new byte[] { message.Data[3], message.Data[2] }, 0),
                    };

                    LogHelper.Log("readTriconRspMsg.ResponseCode =" + readTriconRspMsg.ResponseCode);

                    if (readTriconRspMsg.ResponseCode != 0)
                    {
                        return null;
                    }

                    var rspData = message.Data.Skip(4).ToArray();

                    LogHelper.Log("readTriconRspMsg.NumberOfBlocks =" + readTriconRspMsg.NumberOfBlocks);

                    for (var i = 0; i < readTriconRspMsg.NumberOfBlocks; i++)
                    {
                        var binData = new BinData
                        {
                            Bin = (BinTypes)rspData[0],
                            Rfu = rspData[1],
                            Offset = BitConverter.ToUInt16(new byte[] { rspData[3], rspData[2] }, 0),
                            RelSec = BitConverter.ToInt32(new byte[] { rspData[7] , rspData[6] ,rspData[5], rspData[4] }, 0),
                            MilliSec = BitConverter.ToUInt16(new byte[] { rspData[9], rspData[8] }, 0),
                            NumberOfValues = BitConverter.ToUInt16(new byte[] { rspData[11], rspData[10] }, 0)
                        };

                        rspData = rspData.Skip(12).ToArray();

                        int length = 0;

                        if (BinHelper.IsBoolValue(binData.Bin))
                        {
                            length = Convert.ToInt32(Math.Ceiling((double)binData.NumberOfValues / 8));
                        }
                        else
                        {
                            length = binData.NumberOfValues * 4;
                        }

                        binData.Data = rspData.Take(length).ToArray();

                        rspData = rspData.Skip(length).ToArray();

                        readTriconRspMsg.Bins.Add(binData);
                    }

                    msg = readTriconRspMsg;

                    break;

            }
            
            return msg;
        }


        public void Disconnect()
        {
            IsStoppedPooling = true;
            IsConnected = false;            

            if (_client != null)
            {
                _client.Close();
                _client = null;
            }
        }


        public void SendMessage(Message message)
        {
            List<byte> msg = new List<byte>
            {
                (byte)message.Header.MessageType,
                message.Header.NodeNumber,
                message.Header.SeqNumber,
                message.Header.Version,
                message.Header.Flag,
                message.Header.Id
            };

            if (message.Header.Length == 0 && message.Data != null && message.Data.Length > 0)
            {
                message.Header.Length = Convert.ToUInt16(message.Data.Length + 8); 
            }

            var length = BitConverter.GetBytes(message.Header.Length);

            msg.AddRange(length.Reverse().ToArray());

            if (message.Data != null)
            {
                msg.AddRange(message.Data.Reverse());
            }

            var crc = Crc32Helper.ComputeChecksumBytes(msg.ToArray());

            msg.AddRange(crc);

            _client.Send(msg.ToArray(), msg.Count);
        }


    }
}

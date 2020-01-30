using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Ypf.Mms.Node.Core;
using Ypf.Mms.Node.Infrastructure.Helpers;
using Ypf.Mms.Node.Protocols.Tsaa.Core;
using Ypf.Mms.Node.Protocols.Tsaa.Enumerations;
using Ypf.Mms.Node.Protocols.Tsaa.Helpers;
using Ypf.Mms.Node.Protocols.Tsaa.Messages;

namespace Ypf.Mms.Node.Protocols.Tsaa
{
    public class TsaaDriver : BaseDriver
    {
        private TsaaManager _manager;

        //private bool _discartMessage = false;

        private List<AcquiredData> _acquiredData;


        private Dictionary<BinTypes, DriverQuery> _driverQueries;
        private List<Item> _systemQueries;


        public TsaaDriver() : base()
        {
            _acquiredData = new List<AcquiredData>();
            _systemQueries = new List<Item>();

            _manager = new TsaaManager();
            _manager.ReceivedMessage += TsaaManager_ReceivedMessage;
        }


        private void TsaaManager_ReceivedMessage(object sender, Message e)
        {
            LogHelper.Log("TsaaManager_ReceivedMessage   ---------------->");
            LogHelper.Log("Is ReadTriconRspMsg = " + (e is ReadTriconRspMsg).ToString());
            LogHelper.Log("Is TriconCpStatusMsg = " + (e is TriconCpStatusMsg).ToString());


            if (e is TriconCpStatusMsg statusMsg)
            {
                if (_systemQueries.Count == 0)
                    return;

                var dt = DateTime.Now;

                foreach (var item in _systemQueries)
                {
                    var acq = new AcquiredData
                    {
                        DateTime = dt,
                        Item = item,
                        Name = item.Name
                    };

                    switch (item.Name.Trim().ToUpper())
                    {
                        case "SYS_PROGRAM_NAME":
                            acq.StringValue = statusMsg.CpStat.ProgramName;
                            break;

                        case "SYS_ACTUAL_SCAN_TIME":
                            acq.NumericValue = statusMsg.CpStat.ActualScanTime;
                            break;

                        case "SYS_CP_SCAN_TIME":
                            acq.NumericValue = statusMsg.CpStat.CpScanTime;
                            break;

                        case "SYS_CP_VERSION":
                            acq.StringValue = statusMsg.CpStat.CpVersion;
                            break;

                        case "SYS_LOAD_IN_PROGRESS":
                            acq.NumericValue = statusMsg.CpStat.LoadInProgress;
                            break;

                        case "SYS_CP_DWNLD_TIME":
                            //Ver fecha 
                            acq.StringValue = new DateTime(1970,1,1,0,0,0).AddSeconds(statusMsg.CpStat.CpDwnLdTime).ToString();
                            acq.NumericValue= statusMsg.CpStat.CpDwnLdTime;
                            break;

                        case "SYS_CP_KEY_SWITCH":

                            switch (statusMsg.CpStat.CpKeySwitch)
                            {
                                case 0:
                                    acq.StringValue = "STOP";
                                    break;

                                case 1:
                                    acq.StringValue = "PROGRAM";
                                    break;

                                case 2:
                                    acq.StringValue = "RUN";
                                    break;

                                case 3:
                                    acq.StringValue = "REMOTE";
                                    break;
                            }

                            break;

                        case "SYS_CP_RUN_STATE":

                            switch(statusMsg.CpStat.CpRunState)
                            {
                                case 0:
                                    acq.StringValue = "RUN";
                                    break;

                                case 1:
                                    acq.StringValue = "STOP";
                                    break;

                                case 2:
                                    acq.StringValue = "PAUSE";
                                    break;
                            }
                            
                            break;

                    }

                    _acquiredData.Add(acq);
                }

                _manager.IsStoppedPooling = true;
                AutoResetEvent.Set();

                return;
            }

            if (e is ReadTriconRspMsg msg)
            {
                lock (_acquiredData)
                {
                    var dt = DateTime.Now;

                    LogHelper.Log("msg.Bins!=" + (msg.Bins != null).ToString());

                    if (msg.Bins != null)
                    {
                        LogHelper.Log("msg.Bins.Count=" + msg.Bins.Count);
                    }

                    foreach (var binData in msg.Bins)
                    {
                        var items = Device.Items.Where(x => IsNumericAddess(x.Address) && BinHelper.GetBinTypeByAddress(x.Address) == binData.Bin).OrderBy(o=>o.Address).ToList();

                        foreach (var item in items)
                        {
                            var acq = new AcquiredData
                            {
                                DateTime = dt,
                                Item = item,
                                Name = item.Name
                            };

                            var index = Convert.ToUInt16(item.Address) - BinHelper.GetStartPositionByBinType(binData.Bin) - binData.Offset;

                            if (BinHelper.IsBoolValue(item.Address))
                            {
                                acq.NumericValue = Convert.ToInt16(binData.BoolArray[index]);
                            }
                            else
                            {
                                if (BinHelper.IsRealValue(item.Address))
                                {
                                    acq.NumericValue = BitConverter.ToSingle(new byte[] { binData.Data[index], binData.Data[index + 1], binData.Data[index + 2], binData.Data[index + 3] }, 0);
                                }
                                else
                                {
                                    if (binData.Bin == BinTypes.DintSystemStatusRead)
                                    {
                                        acq.NumericValue = BitConverter.ToUInt16(new byte[] { binData.Data[index], binData.Data[index + 1] }, 0);
                                    }
                                    else
                                    {
                                        acq.NumericValue = BitConverter.ToInt32(new byte[] { binData.Data[index], binData.Data[index + 1], binData.Data[index + 2], binData.Data[index + 3] }, 0);
                                    }
                                }
                            }

                            LogHelper.Log("acq.NumericValue = " + acq.NumericValue);

                            _acquiredData.Add(acq);
                        }
                    }
                }

                if (_systemQueries.Count == 0)
                {
                    _manager.IsStoppedPooling = true;
                    AutoResetEvent.Set();
                }

                return;
            }
        }


        private bool IsNumericAddess(string address)
        {
            return int.TryParse(address, out int num);
        }



        public override void Initialize(Device device)
        {
            base.Initialize(device);

            _driverQueries = new Dictionary<BinTypes, DriverQuery>();

            foreach (var item in device.Items)
            {
                if (!IsNumericAddess(item.Address))
                {
                    _systemQueries.Add(item);

                    continue;
                }

                var addr = Convert.ToUInt16(item.Address);

                var binType = BinHelper.GetBinTypeByAddress(addr);

                if (!_driverQueries.ContainsKey(binType))
                {
                    _driverQueries[binType] = new DriverQuery { BinType = binType, Min = addr, Max = addr };
                }
                else
                {
                    var driverQuery = _driverQueries[binType];

                    driverQuery.Min = addr < driverQuery.Min ? addr : driverQuery.Min;
                    driverQuery.Max = addr > driverQuery.Max ? addr : driverQuery.Max;
                }
            }            
        }



        public override List<AcquiredData> Execute()
        {
            _acquiredData.Clear();

            //var list = base.Execute();

            LogHelper.Log("Conecta al plc");

            _manager.Connect(new IPEndPoint(IPAddress.Parse(Device.IPAddress), Device.Port));

            ReadTriconDataMsg();

            if (_systemQueries.Count > 0)
            {
                LogHelper.Log("_systemQueries.Count = " + _systemQueries.Count);

                TriconCpStatusReqMsg();
            }

            AutoResetEvent.WaitOne(3000);

            LogHelper.Log("Desconecta el plc");

            _manager.Disconnect();

            return _acquiredData;
        }



        private void TriconCpStatusReqMsg()
        {
            LogHelper.Log("TriconCpStatusReqMsg");

            TriconCpStatusReqMsg msg = new TriconCpStatusReqMsg();

            msg.Header.NodeNumber = GetParameterToByte("NodeNumber");
            msg.Header.SeqNumber = 1;
            msg.Header.Version = 0;
            msg.Header.Flag = 0x03;
            msg.Header.Id = 1;
            
            _manager.SendMessage(msg);
        }



        private void ReadTriconDataMsg()
        {
            LogHelper.Log("ReadTriconDataMsg");

            var msg = new ReadTriconDataMsg();

            msg.Header.NodeNumber = GetParameterToByte("NodeNumber");
            msg.Header.SeqNumber = 1;
            msg.Header.Version = 0;
            msg.Header.Flag = 0x03;
            msg.Header.Id = 1;

            LogHelper.Log("_driverQueries id null =" + (_driverQueries!=null).ToString());
            LogHelper.Log("_driverQueries.Values.Count =" + (_driverQueries.Values.Count).ToString());

            foreach (var query in _driverQueries.Values)
            {
                msg.Bins.Add(new ReadTriconDataMsg.BinData
                {
                    BinNumber = query.BinType,
                    Offset = (ushort)(query.Min - BinHelper.GetStartPositionByBinType(query.BinType)),
                    NumberOfValues = query.Count
                });
            }

            msg.Data = msg.GetBytes();

            _manager.SendMessage(msg);
        }

    }
}

using PISDK;
using System;
using System.Collections.Generic;
using Ypf.Mms.Node.Core;
using Ypf.Mms.Node.Infrastructure.Helpers;

namespace Ypf.Mms.Node.Protocols.PI
{
    public class PiDriver : BaseDriver
    {
        PISDK.PISDK _sdk;

        Dictionary<string, List<PiItem>> _driverPoints ;


        public PiDriver() : base()
        {
            _sdk = new PISDK.PISDK();
        }


        public override void Initialize(Device device)
        {
            base.Initialize(device);

            _driverPoints = new Dictionary<string, List<PiItem>>();

            foreach (var item in device.Items)
            {
                var address = item.Address.Trim();

                if (!address.StartsWith($"\\"))
                {
                    continue;
                }
                else
                {
                    address = address.Remove(0, 2);
                }

                var position = address.IndexOf('\\', 0);

                if (position <= 0)
                    continue;

                var serverName = address.Substring(0, position);
                var pointName = address.Substring(position + 1);

                if (string.IsNullOrEmpty(serverName) || string.IsNullOrEmpty(pointName))
                    continue;

                List<PiItem> server;

                if (!_driverPoints.ContainsKey(serverName))
                {
                    server = new List<PiItem>();

                    _driverPoints[serverName] = server;
                }
                else
                {
                    server = _driverPoints[serverName];
                }

                server.Add(new PiItem {FullName = item.Address, Server = serverName, PointName = pointName, Item = item });
            }
        }


        public override List<AcquiredData> Execute()
        {
            List<AcquiredData> _acquiredData = new List<AcquiredData>();

            var dt = DateTime.Now;

            foreach(var server in _driverPoints)
            {
                foreach(var piItem in server.Value)
                {
                    try
                    {
                        var acq = new AcquiredData
                        {
                            DateTime = dt,
                            Item = piItem.Item,
                            Name = piItem.FullName
                        };

                        PIValue value = _sdk.Servers[piItem.Server].PIPoints[piItem.PointName].Data.Snapshot;

                        if (value == null)
                            continue;

                        if (value.Value is DigitalState)
                        {
                            acq.NumericValue = ((DigitalState)value.Value).Code;
                        }
                        else
                        {
                            switch (Type.GetTypeCode(value.GetType()))
                            {
                                case TypeCode.Char:
                                case TypeCode.DateTime:
                                case TypeCode.String:

                                    acq.StringValue = value.Value.ToString();

                                    break;

                                case TypeCode.Boolean:

                                    acq.NumericValue = (bool)value.Value ? 1 : 0;

                                    break;

                                default:

                                    acq.NumericValue = value.Value;

                                    break;
                            }
                        }


                        _acquiredData.Add(acq);
                    }
                    catch (Exception exc)
                    {
                        ExceptionHelper.Manager(exc);
                    }
                }
            }

            return _acquiredData;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Ypf.Mms.Node.Core;
using Ypf.Mms.Node.Core.Interfaces;

namespace Ypf.Mms.Node.Protocols
{
    public class BaseDriver : IDriver
    {

        private Hashtable _driverData;

        protected AutoResetEvent AutoResetEvent { get; set; }

        protected Device Device { get; set; }


        protected void SetParameter(string key, string value)
        {
            if (!_driverData.ContainsKey(key))
            {
                _driverData.Add(key, value);
            }
        }


        protected byte GetParameterToByte(string key, byte defaultValue = 0)
        {
            var value = GetParameter(key);

            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            else
            {
                return Convert.ToByte(value);
            }
        }


        protected string GetParameter(string key)
        {
            if (_driverData.ContainsKey(key))
            {
                return _driverData[key].ToString();
            }

            return string.Empty;
        }


        public BaseDriver()
        {
            _driverData = new Hashtable();

            AutoResetEvent = new AutoResetEvent(false);
        }


        public virtual List<AcquiredData> Execute()
        {
            return new List<AcquiredData>();
        }


        public virtual void Initialize(Device device)
        {
            Device = device;

            if(device != null && !string.IsNullOrEmpty(device.DriverData))
            {
                var list = device.DriverData.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach(var par in list)
                {
                    var parameter = par.Split(new[] { '=', ':' }, StringSplitOptions.None);

                    SetParameter(parameter[0], parameter[1]);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Timers;
using Ypf.Mms.Node.Core;
using Ypf.Mms.Node.Core.Interfaces;
using Ypf.Mms.Node.Infrastructure.Helpers;

namespace Ypf.Mms.Node
{
    public class WorkItem
    {
        public event EventHandler<List<AcquiredData>> AvailableData;

        private Timer _poolingTimer;

        public IDriver DriverInstance { get; set; }

        public WorkItem (Device device)
        {
            Device = device;

            DriverInstance = ActivatorHelper.CreateInstance<IDriver>(device.Driver.FullTypeName);

            if (DriverInstance != null)
            {
                LogHelper.Log("Inicializa el driver");

                DriverInstance.Initialize(device);

                LogHelper.Log("Fin inicialización del driver");
            }

            _poolingTimer = new Timer(device.PoolingInterval);
            _poolingTimer.Elapsed += PoolingTimer_Elapsed;
        }


        private void Execute()
        {
            LogHelper.Log("Ejecuta e driver");

            var list = DriverInstance.Execute();

            LogHelper.Log("Fin ejecución del driver");

            if (list != null && list.Count > 0)
            {
                AvailableData?.Invoke(this, list);
            }
        }



        private void PoolingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _poolingTimer.Stop();

            try
            {
                Execute();
            }
            catch (Exception exc)
            {
                ExceptionHelper.Manager(exc);
            }

            _poolingTimer.Start();
        }


        public void Start()
        {
            if (DriverInstance == null)
                return;

            Execute();

            _poolingTimer.Start();
        }


        public void Stop()
        {
            if (DriverInstance == null)
                return;

            _poolingTimer.Stop();
        }


        public void Dispose()
        {
            _poolingTimer.Dispose();
        }
        

        public Device Device { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Ypf.Mms.Node.Data;
using System.Data.Entity;
using Ypf.Mms.Node.Infrastructure.Helpers;

namespace Ypf.Mms.Node
{
    public class NodeManager
    {
        private Timer _checkTimer;

        private Core.Node Node { get; set; }

        private List<WorkItem> _workItems;
        
        private string Name
        {
            get
            {
                return ConfigurationManager.AppSettings["NodeName"];
            }
        }


        private void StartService(Core.Node node)
        {
            Node = node;

            Initialize();

            if (Node == null)
                return;

            CreateAndStartWorkItems(Node);
        }


        public void Pause()
        {
            if (Node == null || _workItems == null || _workItems.Count == 0)
                return;

            _checkTimer.Stop();

            foreach (var wi in _workItems)
            {
                try
                {
                    wi.Stop();
                }
                catch
                {
                }
            }
        }


        public void Resume()
        {
            if (Node == null || _workItems == null || _workItems.Count == 0)
                return;

            _checkTimer.Start();

            foreach (var wi in _workItems)
            {
                try
                {
                    wi.Start();
                }
                catch
                {
                }
            }
        }

        
        public void Restart()
        {
            Stop();
            Start();
        }


        public void Start()
        {
            try
            {
                Node = GetNode();

                Initialize();

                if (Node == null)
                    return;

                CreateAndStartWorkItems(Node);
            }
            catch(Exception exc)
            {
                ExceptionHelper.Manager(exc);
            }
        }


        private void Initialize()
        {
            int interval = Node != null && Node.CheckInterval > 0 ? Node.CheckInterval.Value : 30000;

            _checkTimer = new Timer(interval);
            _checkTimer.Elapsed += CheckTimer_Elapsed;
            _checkTimer.Start();
        }


        private void CheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _checkTimer.Stop();

            bool isRestart = false;

            try
            {
                var node = GetNode(false, true);

                if (node != null && (node.Reload || Node == null))
                {
                    isRestart = true;

                    Task.Factory.StartNew(() =>
                    {
                        Restart();
                    });
                }
            }
            catch(Exception exc)
            {

            }

            if (!isRestart)
            {
                _checkTimer.Start();
            }
        }


        private Core.Node GetNode(bool changeReload = true, bool nodeOnly = false)
        {
            Core.Node node = null;

            using (var context = new NodeContext())
            {
                if (nodeOnly)
                {
                    node = context.Nodes.SingleOrDefault(x => x.Name == Name);
                }
                else
                {
                    node = context.Nodes
                        .Include(u => u.Devices.Select(b => b.Items))
                        .Include(u => u.Devices.Select(c => c.Driver))
                        .SingleOrDefault(x => x.Name == Name);
                }

                if (node != null && changeReload)
                {
                    node.Reload = false;

                    context.SaveChanges();
                }
            }

            return node;
        }


        public void CreateAndStartWorkItems(Core.Node node)
        {
            _workItems = new List<WorkItem>();

            foreach (var d in Node.Devices)
            {
                if (d.Driver == null)
                    continue;

                try
                {
                    var wi = new WorkItem(d);

                    wi.AvailableData += WorkItemAvailableData;

                    _workItems.Add(wi);

                    wi.Start();
                }
                catch(Exception exc)
                {

                }
            }
        }


        private void WorkItemAvailableData(object sender, List<Core.AcquiredData> e)
        {
            LogHelper.Log("WorkItemAvailableData");

            if (e == null || e.Count == 0)
                return;

            LogHelper.Log("Entro");


            try
            {
                using (var context = new NodeContext())
                {
                    foreach (var acd in e)
                    {
                        var item = context.Items.SingleOrDefault(x => x.Id == acd.Item.Id);

                        LogHelper.Log("item!=null" + (item != null).ToString());

                        if (item == null)
                            continue;

                        acd.Item = item;

                        context.AcquiredData.Add(acd);
                    }

                    context.SaveChanges();
                }
            }
            catch(Exception exc)
            {
                LogHelper.Log(exc.Message);

                ExceptionHelper.Manager(exc);
            }
        }


        public void StopAndDisposeWorkItems(Core.Node node)
        {
            if (_workItems != null)
            {
                foreach (var wItem in _workItems)
                {
                    wItem.AvailableData -= WorkItemAvailableData;
                    wItem.Stop();
                    wItem.Dispose();
                }

                _workItems.Clear();
                _workItems = null;
            }
        }


        public void Stop()
        {
            _checkTimer.Stop();

            StopAndDisposeWorkItems(Node);

            Node = null;
        }

    }
}

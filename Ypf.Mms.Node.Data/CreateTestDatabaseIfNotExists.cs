using System;
using System.Collections.Generic;
using System.Data.Entity;


namespace Ypf.Mms.Node.Data
{
    public class CreateTestDatabaseIfNotExists<TContext> : IDatabaseInitializer<TContext> where TContext : NodeContext
    {
        public void InitializeDatabase(TContext context)
        {
            if (!context.Database.Exists())
            {
                try
                {
                    context.Database.Create();
                }
                catch
                {

                }

               
                context.Database.ExecuteSqlCommand($"DROP TABLE {context.Database.Connection.Database}.__migrationhistory");

                var driver = new Core.Driver
                {
                    Name = "TsaaTriconex",
                    FullTypeName = "Ypf.Mms.Node.Protocols.Tsaa.TsaaDriver, Ypf.Mms.Node.Protocols.Tsaa"                   
                };

                var driver2 = new Core.Driver
                {
                    Name = "PI",
                    FullTypeName = "Ypf.Mms.Node.Protocols.PI.PiDriver, Ypf.Mms.Node.Protocols.PI"
                };

                context.Drivers.Add(driver);
                context.Drivers.Add(driver2);

                var item = new Core.Item
                {
                    Address = "SYS_CP_DWNLD_TIME",
                    Name = "SYS_CP_DWNLD_TIME",
                    Description = "SYS_CP_DWNLD_TIME"
                };

                var item2 = new Core.Item
                {
                    Address = "39663",
                    Name = "CPVer",
                    Description = "CPVer"
                };

                var item3 = new Core.Item
                {
                    Address = "SYS_PROGRAM_NAME",
                    Name = "SYS_PROGRAM_NAME",
                    Description = "SYS_PROGRAM_NAME"
                };

                var item4 = new Core.Item
                {
                    Address = "SYS_CP_VERSION",
                    Name = "SYS_CP_VERSION",
                    Description = "SYS_CP_VERSION"
                };

                //var item5 = new Core.Item
                //{
                //    Address = "\\\\piqca\\ARO_TI09411",
                //    Name = "Status",
                //    Description = "Status"
                //};


                context.Items.Add(item);
                context.Items.Add(item2);
                context.Items.Add(item3);
                context.Items.Add(item4);
                //context.Items.Add(item5);

                var node = new Core.Node
                {
                    CheckInterval = 10000,
                    Name = "PRUEBA",
                    Reload = true,
                    Devices = new List<Core.Device>()
                };


                node.Devices.Add(new Core.Device
                {
                    Name = "Device1",
                    PoolingInterval = 5000,
                    Driver = driver,
                    Items = new List<Core.Item> { item, item2, item3, item4 },
                    DriverData = "NodeNumber=2",
                    IPAddress = "192.168.1.2",
                    Port = 1500
                });

                //node.Devices.Add(new Core.Device
                //{
                //    Name = "PI",
                //    PoolingInterval = 5000,
                //    Driver = driver2,
                //    Items = new List<Core.Item> { item5 },
                //    DriverData = "",
                //    IPAddress = "",
                //    Port = 0
                //});

                context.Nodes.Add(node);

                context.SaveChanges();
            }
        }

        //protected override void Seed(NodeContext context)
        //{
        //    if (!context.Database.Exists())
        //    {
        //        var node = new Core.Node
        //        {
        //            CheckInterval = 10000,
        //            Name = "PRUEBA",
        //            Reload = true,
        //            Devices = new List<Core.Device>()
        //        };

        //        node.Devices.Add(new Core.Device
        //        {
        //            Name ="Device1",
        //            PoolingInterval = 5000,
        //        });

        //        context.Nodes.Add(node);
        //    }

        //    base.Seed(context);

        //    context.SaveChanges();
        //}
    }
}

using System.Data.Entity;
using Ypf.Mms.Node.Data.Configurations;


namespace Ypf.Mms.Node.Data
{

    public class NodeContext : DbContext
    {
        public NodeContext() : base("DataServerConnectionString")
        {
            Configuration.LazyLoadingEnabled = false;

            Database.SetInitializer(new CreateTestDatabaseIfNotExists<NodeContext>());
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new NodeConfig());
            modelBuilder.Configurations.Add(new DeviceConfig());
            modelBuilder.Configurations.Add(new ItemConfig());
            modelBuilder.Configurations.Add(new DriverConfig());
            modelBuilder.Configurations.Add(new AcquiredDataConfig());            
        }


        public DbSet<Core.Node> Nodes { get; set; }

        public DbSet<Core.Device> Devices { get; set; }

        public DbSet<Core.Driver> Drivers { get; set; }

        public DbSet<Core.Item> Items { get; set; }

        public DbSet<Core.AcquiredData> AcquiredData { get; set; }
    }
}

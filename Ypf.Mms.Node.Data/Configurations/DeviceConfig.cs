using System.Data.Entity.ModelConfiguration;


namespace Ypf.Mms.Node.Data.Configurations
{
    public class DeviceConfig : EntityTypeConfiguration<Core.Device>
    {
        public DeviceConfig()
        {
            ToTable("Devices")
                .HasKey(s => s.Id);

            Property(p => p.Name)
                    .HasMaxLength(50)
                    .IsRequired();

            Property(p => p.SerialPort)
                    .HasMaxLength(6);

            Property(p => p.IPAddress)
                    .HasMaxLength(20);

            HasRequired(x => x.Driver)
                .WithMany()
                .Map(x => x.MapKey("DriverId"));
            
            Property(p => p.DriverData)
                    .HasMaxLength(300);
            
            HasMany(s => s.Items)
              .WithMany()
              .Map(cs =>
              {
                  cs.MapLeftKey("DeviceId");
                  cs.MapRightKey("ItemId");
                  cs.ToTable("Devices_Items");
              });

        }
    }
}

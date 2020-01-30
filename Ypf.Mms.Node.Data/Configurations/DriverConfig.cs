using System.Data.Entity.ModelConfiguration;


namespace Ypf.Mms.Node.Data.Configurations
{
    public class DriverConfig : EntityTypeConfiguration<Core.Driver>
    {
        public DriverConfig()
        {
            ToTable("Drivers")
                .HasKey(s => s.Id);

            Property(p => p.Name)
                    .HasMaxLength(50)
                    .IsRequired();

            Property(p => p.FullTypeName)
                    .HasMaxLength(250)
                    .IsRequired();
        }
    }
}

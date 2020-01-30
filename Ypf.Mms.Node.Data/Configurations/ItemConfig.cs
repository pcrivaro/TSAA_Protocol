using System.Data.Entity.ModelConfiguration;


namespace Ypf.Mms.Node.Data.Configurations
{
    public class ItemConfig : EntityTypeConfiguration<Core.Item>
    {
        public ItemConfig()
        {
            ToTable("Items")
                .HasKey(s => s.Id);
            
            Property(p => p.Name)
                    .HasMaxLength(50)
                    .IsRequired();

            Property(p => p.Description)
                    .HasMaxLength(190);

            Property(p => p.Address)
                    .HasMaxLength(100);
        }
    }
}

using System.Data.Entity.ModelConfiguration;


namespace Ypf.Mms.Node.Data.Configurations
{
    public class NodeConfig : EntityTypeConfiguration<Core.Node>
    {
        public NodeConfig()
        {
            ToTable("Nodes")
                .HasKey(s => s.Id);

            Property(p => p.Name)
                    .HasMaxLength(50)
                    .IsRequired();

            HasMany(g => g.Devices)
                .WithRequired()
                .Map(x => x.MapKey("NodeId"));
        }
    }
}

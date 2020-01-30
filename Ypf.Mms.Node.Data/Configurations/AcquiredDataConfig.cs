using System.Data.Entity.ModelConfiguration;


namespace Ypf.Mms.Node.Data.Configurations
{
    public class AcquiredDataConfig : EntityTypeConfiguration<Core.AcquiredData>
    {
        public AcquiredDataConfig()
        {
            ToTable("AcquiredData")
                .HasKey(s => s.Id);

            Ignore(p => p.Name);

            Property(p => p.DateTime)
                    .IsRequired();

            Property(p => p.StringValue)
                .HasMaxLength(300);

            HasRequired(x => x.Item)
                .WithMany()
                .Map(x => x.MapKey("ItemId"));
        }
    }
}

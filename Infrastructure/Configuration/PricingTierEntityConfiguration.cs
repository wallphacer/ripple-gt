using Domain.Events;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PricingTierConfiguration : IEntityTypeConfiguration<PricingTier>
{
    public void Configure(EntityTypeBuilder<PricingTier> builder)
    {
        builder.HasKey(pt => pt.Id);

        builder.Property(pt => pt.Id)
            .ValueGeneratedOnAdd();


        builder.Property(pt => pt.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pt => pt.CostInCents)
            .IsRequired();

        builder.HasOne<Event>(pt => pt.Event)
            .WithMany(e => e.PricingTiers)
            .HasForeignKey(pt => pt.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(pt => pt.Capacity)
            .IsRequired();

        builder.Property(p => p.Version)
            .IsConcurrencyToken();
    }
}
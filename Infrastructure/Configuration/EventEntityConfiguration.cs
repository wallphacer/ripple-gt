using Domain.Events;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(e => e.Venue)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(e => e.EventDate)
            .IsRequired();

        builder.Property(e => e.TotalCapacity)
            .IsRequired();

        builder.HasMany<PricingTier>(e => e.PricingTiers)
            .WithOne(pt => pt.Event)
            .HasForeignKey(pt => pt.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.Venue);
        builder.HasIndex(e => e.EventDate);
    }
}
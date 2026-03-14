using Domain.Shared;
using Domain.Tickets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd();

        builder.Property(t => t.CustomerEmail)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasOne<PricingTier>(t => t.PricingTier)
            .WithMany(pt => pt.Tickets)
            .HasForeignKey(t => t.PricingTierId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasIndex(t => t.CustomerEmail);
    }
}
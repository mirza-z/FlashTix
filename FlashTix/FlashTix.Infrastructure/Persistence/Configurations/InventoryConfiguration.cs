using FlashTix.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashTix.Infrastructure.Persistence.Configurations;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("inventory", t =>
        {
            t.HasCheckConstraint("ck_inventory_available_range",
                "available >= 0 AND available <= capacity");
        });

        builder.HasKey(i => i.EventId);
        builder.Property(i => i.EventId).ValueGeneratedNever();

        builder.Property(i => i.Version).IsConcurrencyToken();

        builder.HasOne<Event>()
            .WithOne()
            .HasForeignKey<Inventory>(i => i.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
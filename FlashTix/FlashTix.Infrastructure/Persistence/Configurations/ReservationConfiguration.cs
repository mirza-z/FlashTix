using FlashTix.Domain.Events;
using FlashTix.Domain.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashTix.Infrastructure.Persistence.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("reservations", t =>
            t.HasCheckConstraint("ck_reservations_quantity_positive", "quantity > 0"));

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();

        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(r => r.PaymentIntentId).HasMaxLength(100);
        builder.Property(r => r.IdempotencyKey).HasMaxLength(100).IsRequired();

        builder.HasOne<Event>()
            .WithMany()
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.EventId, r.UserId });
        builder.HasIndex(r => new { r.Status, r.ExpiresAt });
        builder.HasIndex(r => new { r.UserId, r.IdempotencyKey }).IsUnique();
    }
}
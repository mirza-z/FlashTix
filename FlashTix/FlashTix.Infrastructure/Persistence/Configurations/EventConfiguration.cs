using FlashTix.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashTix.Infrastructure.Persistence.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("events", t =>
            t.HasCheckConstraint("ck_events_capacity_positive", "capacity > 0"));

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Title).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Venue).HasMaxLength(200).IsRequired();
        builder.Property(e => e.ImageUrl).HasMaxLength(500);
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
    }
}
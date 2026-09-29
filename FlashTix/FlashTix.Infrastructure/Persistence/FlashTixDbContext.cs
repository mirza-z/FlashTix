using FlashTix.Domain.Events;
using FlashTix.Domain.Reservations;
using Microsoft.EntityFrameworkCore;

namespace FlashTix.Infrastructure.Persistence;

public class FlashTixDbContext(DbContextOptions<FlashTixDbContext> options) : DbContext(options)
{
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Inventory> Inventory => Set<Inventory>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(FlashTixDbContext).Assembly);
}
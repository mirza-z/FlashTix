using FlashTix.Application.Abstractions.Persistence;
using FlashTix.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlashTix.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<FlashTixDbContext>(options =>
            options
                .UseNpgsql(configuration.GetConnectionString("Postgres"))
                .UseSnakeCaseNamingConvention());

        services.AddSingleton(TimeProvider.System);

        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
using FlashTix.Application.Abstractions.Messaging;
using FlashTix.Application.Reservations;
using FlashTix.Application.Reservations.Commands.HoldTickets;
using FlashTix.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSingleton(new ReservationOptions
{
    MaxTicketsPerUser = 4,
    HoldDuration = TimeSpan.FromMinutes(10)
});

builder.Services.AddScoped<ICommandHandler<HoldTicketsCommand, HoldTicketsResult>,
    HoldTicketsCommandHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

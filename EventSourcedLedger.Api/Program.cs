using EventSourcedLedger.Api;
using EventSourcedLedger.Core.Projections; // Required to find your Projection class
using JasperFx;
using JasperFx.Events.Projections;
using Marten;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddCors(opts =>
{
    opts.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Wolverine takes over handler routing
builder.Host.UseWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(EventSourcedLedger.Core.Commands.OpenAccount).Assembly);
});

builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("DefaultConnection")!);

    // If we are in development, AutoCreate generates the Postgres tables for us dynamically
    if (builder.Environment.IsDevelopment())
    {
        opts.AutoCreateSchemaObjects = AutoCreate.All;
    }

    // Register the Projection Engine
    opts.Projections.Add<AccountDashboardProjection>(ProjectionLifecycle.Inline);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAngular");
app.UseHttpsRedirection();

app.MapLedgerEndpoints();

app.Run();
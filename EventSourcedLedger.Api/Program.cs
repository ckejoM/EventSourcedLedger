using JasperFx;
using Marten;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("DefaultConnection")!);

    // If we are in development, AutoCreate generates the Postgres tables for us dynamically
    if (builder.Environment.IsDevelopment())
    {
        opts.AutoCreateSchemaObjects = AutoCreate.All;
    }
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
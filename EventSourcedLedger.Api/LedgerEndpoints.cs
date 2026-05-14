using EventSourcedLedger.Core.Commands;
using EventSourcedLedger.Core.Projections;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace EventSourcedLedger.Api;

public static class LedgerEndpoints
{
    public static void MapLedgerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/accounts");

        group.MapPost("/", async (OpenAccount command, IMessageBus bus) =>
        {
            var accountId = await bus.InvokeAsync<Guid>(command);

            return Results.Created($"/api/accounts/{accountId}", new {AccountId = accountId});
        });

        group.MapPost("/{id:guid}/deposit", async (Guid id, DepositRequest request, IMessageBus bus) =>
        {
            var command = new DepositFunds(id, request.Amount, request.Reference);

            await bus.InvokeAsync(command);
            return Results.Ok(new { Message = "Funds deposited successfully." });
        });

        group.MapGet("/{id:guid}", async (Guid id, IQuerySession session) =>
        {
            var view = await session.LoadAsync<AccountDashboardView>(id);

            return view is not null ? Results.Ok(view) : Results.NotFound();
        });
    }
}

public record DepositRequest(decimal Amount, string Reference);
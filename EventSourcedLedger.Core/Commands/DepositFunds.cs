using Marten;
using EventSourcedLedger.Core.Events;

namespace EventSourcedLedger.Core.Commands;

public record DepositFunds(Guid AccountId, decimal Amount, string Reference);

public class DepositFundsHandler
{
    public async Task Handle(DepositFunds command, IDocumentSession session, CancellationToken ct)
    {
        var @event = new FundsDeposited(command.AccountId, command.Amount, command.Reference, DateTimeOffset.UtcNow);

        // Append to the EXISTING stream
        session.Events.Append(command.AccountId, @event);

        await session.SaveChangesAsync(ct);
    }
}
using Marten;
using EventSourcedLedger.Core.Events;

namespace EventSourcedLedger.Core.Commands;

// Pure, immutable command payload
public record OpenAccount(string OwnerName);
public class OpenAccountHandler
{
    public async Task<Guid> Handle(OpenAccount command, IDocumentSession session, CancellationToken ct)
    {
        var accountId = Guid.NewGuid();

        // 1. Create the immutable 'Fact'
        var @event = new AccountOpened(accountId, command.OwnerName, DateTimeOffset.UtcNow);

        // 2. Start a NEW stream in Marten using the AccountId as the Stream Key
        session.Events.StartStream<AccountOpened>(accountId, @event);

        // 3. Persist the event to Postgres
        await session.SaveChangesAsync(ct);

        return accountId;
    }
}
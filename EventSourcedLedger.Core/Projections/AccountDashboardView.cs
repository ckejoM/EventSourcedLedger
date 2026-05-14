using EventSourcedLedger.Core.Events;
using Marten.Events.Aggregation;

namespace EventSourcedLedger.Core.Projections;

public class AccountDashboardView
{
    // The ID must match the Stream ID of the events
    public Guid Id { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public decimal CurrentBalance { get; set; }
    public int TotalTransactions { get; set; }
    public DateTimeOffset LastUpdatedAt { get; set; }
}

public class AccountDashboardProjection : SingleStreamProjection<AccountDashboardView, Guid>
{
    // 1. When an account is opened, CREATE the initial view
    public AccountDashboardView Create(AccountOpened @event)
    {
        return new AccountDashboardView
        {
            Id = @event.AccountId,
            OwnerName = @event.OwnerName,
            CurrentBalance = 0,
            TotalTransactions = 0,
            LastUpdatedAt = @event.OpenedAt
        };
    }

    // 2. When funds are deposited, APPLY the changes to the existing view
    public void Apply(FundsDeposited @event, AccountDashboardView view)
    {
        view.CurrentBalance += @event.Amount;
        view.TotalTransactions += 1;
        view.LastUpdatedAt = @event.DepositedAt;
    }

    // 3. When funds are withdrawn, APPLY the changes to the existing view
    public void Apply(FundsWithdrawn @event, AccountDashboardView view)
    {
        view.CurrentBalance -= @event.Amount;
        view.TotalTransactions += 1;
        view.LastUpdatedAt = @event.WithdrawnAt;
    }
}
namespace EventSourcedLedger.Core.Events;
// EVENT 1: The inception of the aggregate stream
public record AccountOpened(
    Guid AccountId,
    string OwnerName,
    DateTimeOffset OpenedAt);

// EVENT 2: Adding funds to the ledger
public record FundsDeposited(
    Guid AccountId,
    decimal Amount,
    string Reference,
    DateTimeOffset DepositedAt);

// EVENT 3: Removing funds from the ledger
public record FundsWithdrawn(
    Guid AccountId,
    decimal Amount,
    string Reference,
    DateTimeOffset WithdrawnAt);
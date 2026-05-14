# 🏦 Project 8: Event-Sourced Financial Ledger

> **An enterprise-grade financial ledger demonstrating Event Sourcing and strict Command Query Responsibility Segregation (CQRS).**

Traditional CRUD applications destroy historical context with every UPDATE or DELETE statement. In domains requiring strict auditability—such as fintech, logistics, and healthcare—losing the sequence of state changes is unacceptable. 

This project solves the "loss of history" problem by implementing **Event Sourcing**. Every business action is persisted as an immutable domain event (a "Fact") within an append-only stream. The current system state is then dynamically calculated by replaying these events through highly optimized read-model projections.

---

## 🏗️ System Architecture

The architecture enforces a strict physical and logical boundary between writes (Commands) and reads (Queries) to optimize for both high-throughput transaction logging and lightning-fast UI data retrieval.

### The Write Model (Commands)
- **Message Routing:** Incoming HTTP POST requests are mapped to pure C# `record` commands and routed via **Wolverine**, eliminating boilerplate and abstracting the transport layer.
- **Event Store:** Commands are validated and transformed into immutable Domain Events (e.g., FundsDeposited). **Marten** persists these events to a PostgreSQL JSONB stream. State is never mutated directly.

### The Read Model (Queries)
- **Projections:** Marten’s Projection Engine continuously listens to the event stream. When a new event is appended, it automatically applies the delta to a flat, relational Read Model (AccountDashboardView).
- **Data Retrieval:** The Angular client performs simple, sub-millisecond GET requests against the pre-calculated Read Model, entirely bypassing the complex event stream.

---

## 🔄 Event Flow & CQRS Topology

sequenceDiagram
    autonumber
    actor Client as Angular UI
    participant API as ASP.NET Minimal API
    participant Bus as Wolverine (Command Bus)
    participant ES as Marten Event Store (mt_events)
    participant Proj as Projection Engine (Inline)
    participant RM as Read Model (mt_doc_accountdashboardview)

    Note over Client, ES: ─── WRITE SIDE (COMMAND) ───
    Client->>API: POST /api/accounts/{id}/deposit
    API->>Bus: Dispatch DepositFunds Command
    Bus->>ES: Start Transaction & Append FundsDeposited Event
    ES->>ES: Save Immutable Event to JSONB Stream
    
    Note over ES, RM: ─── PROJECTION PIPELINE ───
    ES-->>Proj: Trigger Apply(FundsDeposited)
    Proj->>RM: Mutate State (Balance = Balance + Amount)
    RM-->>ES: Commit Database Transaction
    API-->>Client: 200 OK (Event Accepted)

    Note over Client, RM: ─── READ SIDE (QUERY) ───
    Client->>API: GET /api/accounts/{id}
    API->>RM: IQuerySession.LoadAsync(id)
    RM-->>API: Return flat AccountDashboardView
    API-->>Client: 200 OK (Current Balance Displayed)

---

## 🛠️ Technology Stack

| Concern | Technology | Purpose |
| :--- | :--- | :--- |
| **Frontend UI** | Angular 17+ (Standalone) | Reactive client-side dashboard consuming optimized read models. |
| **API Gateway** | ASP.NET Core 9 (Minimal APIs) | Thin routing layer; zero business logic in controllers. |
| **Command Bus** | Wolverine | In-process mediator for dispatching Commands to Handlers without interface bloat. |
| **Event Store & Projections** | Marten | Leverages PostgreSQL JSONB to act as a native Event Store and CQRS Projection Engine. |
| **Infrastructure** | PostgreSQL 16 (Docker) | Containerized relational database powering both the raw event stream and relational read models. |

---

## 📐 Architecture Decision Records (ADRs)

### 1. Marten over EventStoreDB or Kafka
While EventStoreDB and Kafka are industry standards for event streaming, they introduce significant infrastructure overhead and operational complexity. **Marten** was chosen because it provides robust Event Sourcing capabilities directly on top of PostgreSQL, drastically reducing the infrastructure footprint while maintaining enterprise features like optimistic concurrency and inline/async projections.

### 2. Wolverine over MediatR
MediatR is the traditional choice for .NET CQRS, but it requires heavy boilerplate (e.g., IRequest, constructor injection). **Wolverine** was selected to provide a cleaner, function-based message handling model. It allows handlers to be pure functions and natively supports outbox patterns for future scalability.

### 3. Inline vs. Asynchronous Projections
For this iteration, **Inline Projections** are utilized. The Read Model updates in the exact same database transaction as the appended event, guaranteeing **Strong Consistency** for the client. As write-throughput scales, this can be toggled to Asynchronous Projections (Eventual Consistency) with a one-line configuration change.

---

## 🚀 Getting Started

### Prerequisites
- .NET 9 SDK
- Node.js & Angular CLI (npm install -g @angular/cli)
- Docker Desktop

### Local Development Environment

1. **Spin up the Event Store:**

    docker-compose up -d

2. **Run the Backend API:**

    dotnet run

3. **Run the Angular Client:**

    cd Frontend/event-ledger-ui
    ng serve

Navigate to http://localhost:4200 to interact with the ledger.

---
*Architected and developed by **Jovan Madzic***  
**Software Engineer | Belgrade, Serbia**  
[LinkedIn](https://www.linkedin.com/in/jovan-madzic-12093b202/)
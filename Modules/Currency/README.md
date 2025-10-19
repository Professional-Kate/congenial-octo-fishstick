[← Modules](../README.md) ▸ Currency

# Modules/Currency
The `Currency` module stores player currency and mutates them on a `CurrencyUpdate`. 

---

## Contracts

### `Currency` model

```csharp
public sealed class Currency
{
    public readonly CurrencyType CurrencyType;
    public uint Amount { get; set; }
}
```

- Only one `Currency` of each `CurrencyType` is allowed.

---

## Description

`Currency` in IdelPog is simple. Currently, we just store a `uint Amount` with a `CurrencyType`.

- Commands are simple but easily mutate state.

---

## Commands

### `CurrencyCreation`

```csharp
public readonly record struct CurrencyCreation
{
    public required CurrencyType CurrencyType { get; init; }
    public required uint StartingAmount { get; init; }
}
```

`CurrencyCreation` is used to create a new `Currency` with a specific `StartingAmount`. 

- Creation will fail if the records `CurrencyType` already exists. `CurrencyType` must be unique.

| Buffered records           | Requirements                    | Description                                                                                                 |
|----------------------------|---------------------------------|-------------------------------------------------------------------------------------------------------------|
| `CurrencyCreation`         | None                            | Creates new `Currency` with a specific `uint` `StartingAmount`. Will create one `Currency` for each record. |
| `CurrencyCreationResponse` | Successful `CurrencyCreation`   | Each response will contain a newly created `Currency`.                                                      |
| `CurrencyCreationError`    | Unsuccessful `CurrencyCreation` | Will be dispatched automatically whenever a `CurrencyCreation` fails.                                       |

---

### `CurrencyUpdate`

```csharp

public readonly record struct CurrencyUpdate
{ 
    public required CurrencyType CurrencyType { get; init; }
    public required uint Amount { get; init; }
    public required ActionType ActionType { get; init; }
}

```

`CurrencyUpdate` defines how to mutate a `Currency` that already exists. `ActionType` defines how to mutate the `Currency`.
The update works in a unique way. To reduce the amount we need to process we summarize `CurrencyUpdate`s before processing.
IE,

- Three `CurrencyUpdate`s with `CurrencyType: GOLD, Amount: 5, ActionType: ADD` will result in one `CurrencyType: GOLD, Amount 15, ActionType ADD` command.
- Two `CurrencyUpdates`s with `CurrencyType: GOLD, Amount: 5, ActionType: REMOVE` and one `CurrencyType: GOLD, Amount: 5, ActionType: ADD` will result in one `CurrencyType: GOLD, Amount 5, ActionType REMOVE` command.

Summaries are unique per `CurrencyType`. If after summarization the total `Amount` is 0, no `CurrencyUpdate` will be applied for that `CurrencyType`.

- Updating will fail if the `CurrencyType` is not found. 
- Updating will fail if `ActionType` is `REMOVE` and the `Currency` doesn't have enough `Amount`.
- Updating will fail if after summarization the total amount of summerized records is 0.

| Buffered records         | Requirements                  | Description                                                                                                                                                                |
|--------------------------|-------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `CurrencyUpdate`         | `CurrencyCreation`            | Will update one `Currency` per record. Can either remove or add an `Amount`.                                                                                               |
| `CurrencyUpdateResponse` | Successful `CurrencyUpdate`   | Each response will contain the new state of each `Currenecy` updated. If the total `Amount` of one update is zero, no response will be dispatched for that `CurrencyType`. |
| `CurrencyUpdateError`    | Unsuccessful `CurrencyUpdate` | Will be dispatched automatically whenever a `CurrencyUpdate` fails.                                                                                                        |
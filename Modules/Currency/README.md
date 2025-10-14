# Modules/Currency
The `Currency` module stores player currency and mutates them on request. 

Each `Currency` object just contain their `uint amount`. 

## Contracts

### `Currency` model

```csharp
public sealed class Currency
{
    public readonly CurrencyType CurrencyType;
    public uint Amount { get; set; }
}
```

### Description

`Currency` in IdelPog is just a `uint amount` and that specific currencies `CurrencyType`.

### Listens to

| Command            | Requirements       | Usage                                                                                                                               |
|--------------------|--------------------|-------------------------------------------------------------------------------------------------------------------------------------|
| `CurrencyCreation` | None               | Used to create a new `Currency` object with a given `uint StartingAmount`.                                                          |
| `CurrencyUpdate`   | `CurrencyCreation` | Used to mutate `Currency` based on the commands `ActionType` and `Amount`. The `Currency` updated is defined by the `CurrencyType`. |

### Dispatches

| Command                    | When                            | Usage                                                                                                                             |
|----------------------------|---------------------------------|-----------------------------------------------------------------------------------------------------------------------------------|
| `CurrencyCreationResponse` | Successful `CurrencyCreation`   | This response will be dispatched for each successful `CurrencyCreation` and defines the properties of the `Currency` created.     |
| `CurrencyCreationError`    | Unsuccessful `CurrencyCreation` | This will contain a `BaseError` giving details on why this error occured and a `CurrencyCreation[]` of the Buffer that caused it. |
| `CurrencyUpdateResponse`   | Successful `CurrencyUpdate`     | This response will detail the new amount of that `Currency`. Dispatches one response for each `CurrencyUpdate` input.             |
| `CurrencyUpdateError`      | Unsuccessful `CurrencyUpdate`   | This will contain a `BaseError` giving details on why this error occured and a `CurrencyUpdate[]` of the Buffer that caused it.   |
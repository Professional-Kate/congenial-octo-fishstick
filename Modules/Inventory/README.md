# Modules/Inventory
The `Inventory` module stores player `Item`s and handles all mutations, adding or removing from their `Amount` on request.

It also manages `Crafting` and `Recipes` which define how sets of input Items can be transformed into new output Items.

---

## Contracts

### `Item` model

```csharp
public sealed class Item
{
    public readonly ItemID ItemID;
    public readonly uint BaseSellPrice;
    public readonly Information Information;
    public uint Amount { get; set; }
}
```

Items are used within the `Inventory`. If an `Item` doesn't exist in the `Inventory` but is added to, an `Item` object 
with the required `ItemID` and `Amount` will be created. If an `Item` is removed from and reduced to zero `Amount`
it will be removed from the `Inventory`.

---

### `ItemDefinition` record

```csharp

public readonly record struct ItemDefinition
{
    public required ItemID ItemID { get; init; }
    public required uint BaseSellPrice { get; init; }
    public required Information Information { get; init; }
}
```

This record will be read when a new `Item` is generated, ensuring each have the correct metadata. 
If an `ItemDefinition` for any given `ItemID` is not found an `Assertion` will be triggered. 

---

### `RecipeInputComponent` and `RecipeOutputComponent` component records

```csharp
 
public readonly record struct RecipeInputComponent : IComponent<RecipeInputComponent>
{
    public required ItemID ItemID { get; init; }
    public required uint RequiredAmount { get; init; }
}
    
public readonly record struct RecipeOutputComponent : IComponent<RecipeOutputComponent>
{
    public required ItemID ItemID { get; init; }
    public required uint OutputAmount { get; init; }
}
```

These components will define the structure of a crafting recipe:
- Inputs list all required Items and Amounts
- Outputs define what the recipe produces.

---

### `CraftingRecipeEntity`

```csharp
public sealed record CraftingRecipeEntity : Entity
{
    private readonly ComponentStore<RecipeInputComponent> _ingredientStore;
    private readonly ComponentStore<RecipeOutputComponent> _outputStore;
}
```

A `CraftingRecipeEntity` defines a complete recipe using its input and output `ComponentStore`s.
It represents the full transformation of input `ItemID`s into output `ItemID`s during crafting.

---

## Description

The `Inventory` module controls the full lifecycle of all `Item` objects. The `Inventory` is used by the Crafting system
to allow recipes to consume existing Items and generate new ones.

`Item`s can be added and removed by both `InventoryUpdate` and `ItemCraft` but, internally, all items are mutated via `InventoryUpdate`s.

- `Item`s can be removed by both `InventoryUpdate` and `ItemCraft`. These flows share `Item` mutation logic.
- `InventoryUpdateResponse` will be dispatched by anything that modifies `Item`s. Regardless of the input command. IE, `ItemCraft` will also dispatch `InventoryUpdateResponse`s on successful craft.

---

## Commands

### `ItemDefinitionCreation`

````csharp
public readonly record struct ItemDefinitionCreation
{
    public required ItemID ItemID { get; init; }
    public required uint BaseSellPrice { get; init; }
    public required Information Information { get; init; }
}
````

`ItemDefinitionCreation` will create an `ItemDefinition` for the `ItemID`. 

`ItemDefinitions` are used when creating `Item`s. A definition informs the `Item`s `BaseSellPrice` and `Information`, both required fields in `Item` creation.
If an `Item` is created via `InventoryUpdate` and a definition is not found, the operation will fail. 

- Creation will fail if `ItemID` already exists. `ItemID` must be unique.
- Creation will fail if the `BaseSellPrice` is zero. Only positive, non-zero sell prices are allowed currently.

| Buffered records                 | Requirements                          | Description                                                                 |
|----------------------------------|---------------------------------------|-----------------------------------------------------------------------------|
| `ItemDefinitionCreation`         | None                                  | Creates new `ItemDefinition`s for each record.                              |
| `ItemDefinitionCreationResponse` | Successful `ItemDefinitionCreation`   | Each response will contain a newly created `ItemDefinition`.                |
| `ItemDefinitionCreationError`    | Unsuccessful `ItemDefinitionCreation` | Will be dispatched automatically whenever a `ItemDefinitionCreation` fails. |

---

### `InventoryUpdate`

```csharp
public readonly record struct InventoryUpdate
{
    public required ItemID ItemID { get; init; }
    public required uint Amount { get; init; }
    public required ActionType ActionType { get; init; }
}
```

`InventoryUpdate` is used to mutate `Item`s in the `Inventory`.
`ItemDefinition`s will only be required if the update needs to create a new `Item`. IE, you have zero `STONE` but add one. This will create `STONE` thus requiring a `STONE` `ItemDefinition`.

- An update can create new `Item`s if not already existing. This will require an `ItemDefinition`.
- Updates can add and remove an `Item`s `Amount` if they exist. 
- They can also remove `Item`s from the `Inventory` if their `Amount` is 0 after the update.

`InventoryUpdate`s are first summerized before any processing occurs. IE, 

- Three `InventoryUpdate`s with `ItemID: STONE, Amount 1, ActionType: ADD` will result in one `ItemID: STONE, Amount 3, ActionType: ADD` command.
- Two `InventoryUpdate`s with `ItemID: STONE, Amount 1, ActionType: REMOVE` and one `ItemID: STONE, Amount 1, ActionType: ADD` will result in one `ItemID: STONE, Amount 1, ActionType: REMOVE` command.

Summaries are unique per `ItemID`. If after summarization the total `Amount` is 0, no `InventoryUpdate` will be applied for that `ItemID`.

- If the `ActionType` is `ADD` and the `Item` is not found, an `ItemDefinition` will be required. If this is not found, updating will fail.
- Updating can fail if the `ActionType` is `REMOVE` and the `Item` does not have enough `Amount` to remove.
- Updating will fail if after summarization the total amount of summerized records is 0.


| Buffered records          | Requirements                   | Description                                                                                                                                             |
|---------------------------|--------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------|
| `InventoryUpdate`         | `ItemDefinitionCreation`       | For each record will attempt to `ADD` or `REMOVE` from an `Item`. `Item`s will be created if not found or removed if their `Amount` is reduced to zero. |
| `InventoryUpdateResponse` | Successful `InventoryUpdate`   | Each response will contain the new state of an `Item` mutated in the update. One record per `Item` changed.                                             |
| `InventoryUpdateError`    | Unsuccessful `InventoryUpdate` | Will be dispatched automatically whenever a `InventoryUpdate` fails.                                                                                    |

---

## Crafting 

### `RecipeCreation`

```csharp
public readonly record struct RecipeCreation
{
    public required RecipeID RecipeID { get; init; }
    public required RecipeInput[] RecipeInputs { get; init; }
    public required RecipeOutput[] RecipeOutputs { get; init; }
}
```

`RecipeCreation` will create a new recipe using the `RecipeInput` and `RecipeOutput` records. 

Recipes can have x inputs to y output `Item`s. 

- `RecipeInputs` will remove `Item`s.
- `RecipeOutputs` will add `Item`s. 

`Item`s will be mutated using the same flow as `InventoryUpdate`. Please see that command for ways `Item` mutation can fail.

- Creation will fail if `RecipeID` already exists. `RecipeID` must be unique.
- Creation will fail if any Input/Output `Amount` is 0.

`Item`s are not required to have an `ItemDefinition` created. This command does not create `Item`s.

| Buffered records         | Requirements                  | Description                                                         |
|--------------------------|-------------------------------|---------------------------------------------------------------------|
| `RecipeCreation`         | None                          | For each record will attempt to create a new recipe.                |
| `RecipeCreationResponse` | Successful `RecipeCreation`   | Each response will contain one newly created Recipe.                |
| `RecipeCreationError`    | Unsuccessful `RecipeCreation` | Will be dispatched automatically whenever a `RecipeCreation` fails. |

---

### `ItemCraft`

```csharp
public readonly record struct ItemCraft
{
    public required RecipeID RecipeID { get; init; }
    public required uint Amount { get; init; }
}
```

`ItemCraft` will attempt to create the provided `RecipeID`. This will mutate the `Inventory` in the same way as `InventoryUpdate`.
If this operation is successful, multiple `InventoryUpdateResponses` will be dispatched. 

In the same way as `InventoryUpdate`, 

- Crafting will fail if any `RecipeInput.ItemID` does not have enough `Amount`.
- Crafting will fail if any `RecipeOutput.ItemID` does not have a created `ItemDefinition`.

Crafting both removes and adds `Item`s. The state of each item updated will be represented in the `InventoryUpdateResponse` records.

| Buffered records          | Requirements                               | Description                                                                                                                                                  |
|---------------------------|--------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `ItemCraft`               | `RecipeCreation`, `ItemDefinitionCreation` | For each record will attempt to craft a `RecipeID` in an `Amount`. Successfully doing this will remove `RecipeInput` `Item`s and add `RecipeOutput` `Item`s. |
| `ItemCraftResponse`       | Successful `ItemCraft`                     | Each response will contain one newly created Recipe. This response will not contain any details of the `Item`s changed.                                      |
| `ItemCraftError`          | Unsuccessful `ItemCraft`                   | Will be dispatched automatically whenever a `ItemCraft` fails.                                                                                               |
| `InventoryUpdateResponse` | Successful `ItemCraft`                     | Each unique `ItemID` mutated will dispatch one record. The record will not detail that it came from an `ItemCraft`.                                          |
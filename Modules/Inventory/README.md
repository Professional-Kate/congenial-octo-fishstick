# Modules/Inventory
The `Inventory` module stores player `Item`s and handles all mutations, adding or removing from their `Amount` on request.

It also manages `Crafting` and `Recipies` which define how sets of input Items can be transformed into new output Items.

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

### `CraftingRecipeEntity`

```csharp
public sealed record CraftingRecipeEntity : Entity
{
    private readonly ComponentStore<RecipeInputComponent> _ingredientStore;
    private readonly ComponentStore<RecipeOutputComponent> _outputStore;
}
```

A `CraftingRecipeEntity` defines a complete recipe using its input and output `ComponentStore`s.
It represents the full transformation of input `ItemID`s into ouput `ItemID`s during crafting.

### Description`

The `Inventory` module controls the full lifecycle of all `Item` objects. The `Inventory` is used by the Crafting system
to allow recipes to consume existing Items and generate new ones.

### Listens to

| Command                  | Requirements                               | Usage                                                                                                                                                                                       |
|--------------------------|--------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `ItemDefinitionCreation` | None                                       | When Items for the `Inventory` are generated an `ItemDefinition` will be read to assign the correct `Information` and `BaseSellPrice`. Each `Item` is required to have an `ItemDefinition`. |
| `InventoryUpdate`        | `ItemDefinitionCreation`                   | Contains an `ActionType` that will define how to mutate an `Item`. This will create Items if not found, or delete them if their final `Amount` is zero.                                     |
| `RecipeCreation`         | None                                       | Creates a new recipe by `RecipeID`. Recipes on creation will not require any Items to be in the `Inventory`.                                                                                |
| `ItemCraft`              | `RecipeCreation`, `ItemDefinitionCreation` | Crafts items using a recipe. Will consume input items, generate output items, and dispatch multiple `InventoryUpdateResponse`s when successful.                                             |

### Dispatches

| Command                          | When                                  | Usage                                                                                                                                                                                        |
|----------------------------------|---------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `ItemDefinitionCreationResponse` | Successful `ItemDefinitionCreation`   | This response will be dispatched for each `ItemDefinitionCreation` and defines the created `ItemDefinition`s properties.                                                                     |
| `ItemDefinitionCreationError`    | Unsuccessful `ItemDefinitionCreation` | This will contain a `BaseError` giving details on why this error occured and a `ItemDefinitionCreation[]` of the Buffer that caused it.                                                      |
| `InventoryUpdateResponse`        | Successful `InventoryUpdate`          | This response will contain the new state of the `Item`s mutated in the action. For each `Item` changed a Response will be dispatched.                                                        |
| `InventoryUpdateError`           | Unsuccessful `InventoryUpdate`        | This will contain a `BaseError` giving details on why this error occured and a `InventoryUpdate[]` of the Buffer that caused it.                                                             |
| `RecipeCreationResponse`         | Successful `RecipeCreation`           | This response will contain the `RecipeID` with the recipes input and output requirements. One response will be dispatched for each `RecipeCreation`.                                         |
| `RecipeCreationError`            | Unsuccessful `RecipeCreation`         | This will contain a `BaseError` giving details on why this error occured and a `RecipeCreation[]` of the Buffer that caused it.                                                              |
| `ItemCraftResponse`              | Successful `ItemCraft`                | This will attempt to craft a Recipe created with `RecipeCreation`. On successful craft this will also dispatch multiple `InventoryUpdateResponse`s with the same behaviour as that response. |
| `ItemCraftError`                 | Unsuccessful `ItemCraft`              | This will contain a `BaseError` giving details on why this error occured and a `ItemCraft[]` of the Buffer that caused it.                                                                   |

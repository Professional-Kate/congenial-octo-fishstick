# Modules/HarvestNode
The `HarvestNode` module controls the lifecycle of `HarvestNode`s. In IdelPog, these define nodes in the world that when
harvested will grant Items for the `Inventory`. These also contain a `Levelable` which on `HarvestNodeUpdate` will be progressed.

This module also contains commands for creating `LootTable`s for each `HarvestNode`, these tables have a chance of granting Items.

`HarvestNode`s can be locked via the `HarvestNodeRequirementsCreation` command. This will require certain `Skill` levels to unlock and use.

## Contracts

### `HarvestNode` model

```csharp
public sealed record HarvestNode
{
    public required LocationID LocationID { get; init; }
    public required ResourceID ResourceID { get; init; }
    public required Levelable Levelable { get; init; }
    public required Information Information { get; init; }
}
```

- `ResourceID` defines what in the world this `HarvestNode` is. 
- `LocationID` defines where in the world this `HarvestNode` is.
- The `Levelable` will be updated on every update action.

### `ReadOnlyHarvestNode` record

```csharp
public readonly record struct ReadOnlyHarvestNode
{
    public required LocationID LocationID { get; init; }
    public required ResourceID ResourceID { get; init; }
    public required ReadOnlyLevelable ReadOnlyLevelable { get; init; }
    public required Information Information { get; init; }
}
```

This record is used to represent a `HarvestNode`. `ReadOnlyLevelable` will always contain the new state of the `HarvestNode`.

### `HarvestTargetComponent` and `SkillComponent`

```csharp
public readonly record struct HarvestTargetComponent : IComponent<HarvestTargetComponent>
{
    public required ResourceID HarvestTarget { get; init; }
}

public readonly record struct SkillComponent : IComponent<SkillComponent>
{
    public required SkillID SkillID { get; init; }
}

```

These components in the `SkillNodeEntity` will define what `HarvestNode`s are connected with what skills. In IdelPog,
nodes are expected to be leveled alongside skills but not strictly required.

If a `HarvestNode` isn't registered to a `Skill` then any updates on that node will not be allowed. A `ResourceID` with 
matching `SkillID` is expected.

### `SkillNodeEntity`

````csharp

public sealed record SkillNodeEntity : Entity
{
    private readonly ComponentStore<HarvestTargetComponent> _harvestTargetStore;
    
    public SkillNodeEntity(IRepositoryAsserter repositoryAsserter, SkillComponent skillComponent, HarvestTargetComponent[] allowedNodes)
        : base(repositoryAsserter, new ComponentStore<HarvestTargetComponent>(allowedNodes), skillComponent)
    {
        _harvestTargetStore = GetComponent<ComponentStore<HarvestTargetComponent>>();
    }
}

````

This Entity contains a `ComponentStore` of `HarvestTargetComponent` which will be linked to the passed `SkillComponent` 
on construct.

### Progression: `UnlockRequirementsEntity` and `LevelRequirementComponent`

```csharp
public sealed record UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse> : Entity where TCommand : struct
{
    private readonly QueueComponentStore<LevelRequirementComponent<SkillID, HarvestNodeUnlockResponse>> _levelRequirementStore;
}

public readonly record struct LevelRequirementComponent<SkillID, HarvestNodeUnlockResponse> : IComponent<LevelRequirementComponent<SkillID, HarvestNodeUnlockResponse>> 
    where TCommand : struct
{
    public required SkillID ID { get; init; }
    public required byte Level { get; init; }
    public required HarvestNodeUnlockResponse OnUnlockCommand { get; init; }
}
```

This links the `Levelable.Level` of a `Skill` to locking/unlocking of `HarvestNode`s. Each node can be locked, or none at all can be locked. 
The required `Skill` `Level`, `OnUnlockCommand`, and linked `HarvestNode` can all be configured using the commands below.

### Description

`HarvestNode`s at their simplest are just `Levelable`s that can be progressed with `HarvestNodeUpdate`.
- `HarvestNode`s are linked to `Skill`s.
- `HarvestNode`s can update the `Inventory` via `InventoryUpdate` commands and their linked `LootTable`s.
- `HarvestNode`s can generate Items for both their `ResourceID` and `LocationID`.
- When locking a `HarvestNode` with `HarvestNodeRequirementsCreation`, it is not required to have that `HarvestNode` created. These services are separate. 

## APIs

### `HarvestNodeCreation`

```csharp
public readonly record struct HarvestNodeCreation
{
    public required ReadOnlyHarvestNode[] ReadOnlyHarvestNodes { get; init; }
    public required SkillID LinkedSkill { get; init; }
}
```

`HarvestNodeCreation` is used to create new `HarvestNode`s that are linked to the `LinkedSkill` `SkillID`.

- Creation will fail if the creations `ResourceID` already exists. `LocationID` can be shared, `ResourceID` must be unique.

| Buffered records              | Requirements                       | Description                                                                                                    |
|-------------------------------|------------------------------------|----------------------------------------------------------------------------------------------------------------|
| `HarvestNodeCreation`         | None                               | Creates new `HarvestNode`s for each record. The `SkillID` `LinkedSkill` will be used in the `SkillNodeEntity`. |
| `HarvestNodeCreationResponse` | Successful `HarvestNodeCreation`   | Each response will contain one newly created `HarvestNode` with linked `SkillID`.                              |
| `HarvestNodeCreationError`    | Unsuccessful `HarvestNodeCreation` | Will be dispatched automatically whenever a `HarvestNodeCreation` fails.                                       |

### `HarvestNodeUpdate`

```csharp
public readonly record struct HarvestNodeUpdate
{
    public required ResourceID ResourceID { get; init; }
    public required SkillID SkillID { get; init; }
}
```

`HarvestNodeUpdate` is used to progress `HarvestNode`s by updating their `Levelable` provided on `HarvestNodeCreation`.
This update will also generate `Item`s if a `LootTable` has been created with `ResourceLootCreation` or `LocationLootCreation`.

- Updating will fail if the `HarvestNode` is locked via `HarvestNodeRequirementsCreation`.
- Updating will fail if the `HarvestNode` is not linked to the records `SkillID`.
- The `HarvestNode` will still update if an `Item` creation occurs but fails due to any reason. 

| Buffered records            | Requirements                     | Description                                                                                 |
|-----------------------------|----------------------------------|---------------------------------------------------------------------------------------------|
| `HarvestNodeUpdate`         | `HarvestNodeCreation`            | Updates a `HarvestNode` by updating their `Levelable`. One node will be updated per record. |
| `HarvestNodeUpdateResponse` | Successful `HarvestNodeUpdate`   | Each response will contain the new state of any updated `HarvestNode`.                      |
| `HarvestNodeUpdateError`    | Unsuccessful `HarvestNodeUpdate` | Will be dispatched automatically whenever a `HarvestNodeUpdate` fails.                      |

### `HarvestNodeRequirementsCreation`

```csharp

public readonly record struct HarvestNodeRequirementsCreation
{
    public required SkillID SkillID { get; init; }
    public required HarvestNodeRequirement[] HarvestNodeRequirements { get; init; }
}
    
public readonly record struct HarvestNodeRequirement
{
    public required byte RequiredLevel { get; init; }
    public required HarvestNodeUnlockResponse OnUnlockCommand { get; init; }
}
```

`HarvestNodeRequirementsCreation` is used to lock `HarvestNode`s. A locked node cannot be updated with `HarvestNodeUpdate`.
`HarvestNode`s will be locked via a `Skill` level. To unlock a node you are required to dispatch a `HarvestNodeUnlock` containing a `SkillID` and a `SkillLevel`.
On unlock the `OnUnlockCommand` will be dispatched.

- Creation will fail if `SkillID` is already found. Duplicate `SkillID`s are not allowed.
- `HarvestNodeUnlockResponse` should inform on what `HarvestNode` was unlocked. 

`HarvestNodeUpdate` 

| Buffered records                          | Requirements                                   | Description                                                                          |
|-------------------------------------------|------------------------------------------------|--------------------------------------------------------------------------------------|
| `HarvestNodeRequirementsCreation`         | None                                           | Creates new requirement sets for each `HarvestNodeRequirement`.                      |
| `HarvestNodeRequirementsCreationResponse` | Successful `HarvestNodeRequirementsCreation`   | Each response will contain a newly created requirement.                              |
| `HarvestNodeRequirementsCreationError`    | Unsuccessful `HarvestNodeRequirementsCreation` | Will be dispatched automatically whenever a `HarvestNodeRequirementsCreation` fails. |

### `HarvestNodeUnlock`

```csharp
public readonly record struct HarvestNodeUnlock
{
    public required SkillID SkillID { get; init; }
    public required byte SkillLevel { get; init; }
}
```

`HarvestNodeUnlock` will attempt to unlock `HarvestNode`s by using the `SkillLevel`. Multiple requirements can be unlocked by one record, 
in this case, multiple `HarvestNodeUnlockResponse` will be dispatched.

- Unlocking will not fail if no `HarvestNodeRequirementsCreation` records have been dispatched. No found requirements mean the node is unlocked. 

| Buffered records            | Requirements                     | Description                                                                                                                     |
|-----------------------------|----------------------------------|---------------------------------------------------------------------------------------------------------------------------------|
| `HarvestNodeUnlock`         | None                             | Attempts to unlock any requirement who's `SkillLevel` is less than or equal to `HarvestNodeUnlock.SkillLevel`.                  |
| `HarvestNodeUnlockResponse` | Successful `HarvestNodeUnlock`   | Each response will contain one `HarvestNode` unlocked in the operation. One `HarvestNodeUnlock` can produce multiple responses. |
| `HarvestNodeUnlockError`    | Unsuccessful `HarvestNodeUnlock` | Will be dispatched automatically whenever a `HarvestNodeUnlock` fails.                                                          |

## Loot

```csharp

public readonly record struct LootTableEntry
{
    public required ItemID ItemID { get; init; }
    public required int Weight { get; init; }
}

public readonly record struct GrantPolicyEntry
{
    public required int GrantWeight { get; init; }
    public required int SkipWeight { get; init; }
}
```

`LootTableEntry` is expected to be used in an array.

- If only one `LootTableEntry` is provided, a `GrantTable` will be generated.
- If more than one `LootTableEntry` is provided, a `WeightedLootTable` will be used.
- If `GrantPolicy` is provided with 0 `GrantWeight` or `SkipWeight`, then `GrantPolicy` and `SkipPolicy` will be used respectively.

`Item` generation can fail and dispatch an `InventoryUpdateError`. This failure will not cause an update to fail.

### `ResourceLootCreation`

```csharp

public readonly record struct ResourceLootCreation
{
    public required ResourceID ResourceID { get; init; }
    public required LootTableEntry[] LootTableEntries { get; init; }
    public required GrantPolicyEntry GrantPolicyEntry { get; init; }
}
```

`ResourceLootCreation` will create a `LootTable` and a `GrantPolicy` based on the records in the command. 
This table and policy will be linked to a `ResourceID`. 

Any `HarvestNode` updated with `HarvestNodeUpdate` with matching `ResourceID` will first trigger the `GrantPolicy`. If this is successful, an `Item` will be 
generated using the `LootTableEntries`.

- Creation will fail if the `ResourceID` already exists. 
- Each `LootTableEntry` must have a non-zero, positive Weight. Creation will fail otherwise.

| Buffered records               | Requirements                        | Description                                                                                                                                                 |
|--------------------------------|-------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `ResourceLootCreation`         | None                                | Creates a new `LootTable` and `GrantPolicy` for the `ResourceID`. Whenever a `HarvestNode` with matching `ResourceID` is updated, the table will be rolled. |
| `ResourceLootCreationResponse` | Successful `ResourceLootCreation`   | Each response will contain one new `ResourceID` `LootTable` created.                                                                                        |
| `ResourceLootCreationError`    | Unsuccessful `ResourceLootCreation` | Will be dispatched automatically whenever a `ResourceLootCreation` fails.                                                                                   |

### `LocationLootCreation`

```csharp

public readonly record struct LocationLootCreation
{
    public required LocationID LocationID { get; init; }
    public required ResourceID ResourceID { get; init; }
    public required LootTableEntry[] LootTableEntries { get; init; }
    public required GrantPolicyEntry GrantPolicyEntry { get; init; }
}
```

`LocationLootCreation` will create a `LootTable` and a `GrantPolicy` based on the records in the command.
This table and policy will be linked to a `LocationID`.

Any `HarvestNode` updated with `HarvestNodeUpdate` with matching `LocationID` will first trigger the `GrantPolicy`. If this is successful, an `Item` will be
generated using the `LootTableEntries`.

- Creation will fail if the `LocationID` already exists.
- Each `LootTableEntry` must have a non-zero, positive Weight. Creation will fail otherwise.

| Buffered records               | Requirements                        | Description                                                                                                                                                 |
|--------------------------------|-------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `LocationLootCreation`         | None                                | Creates a new `LootTable` and `GrantPolicy` for the `LocationID`. Whenever a `HarvestNode` with matching `LocationID` is updated, the table will be rolled. |
| `LocationLootCreationResponse` | Successful `LocationLootCreation`   | Each response will contain one new `LocationID` `LootTable`created.                                                                                         |
| `LocationLootCreationError`    | Unsuccessful `LocationLootCreation` | Will be dispatched automatically whenever a `LocationLootCreation` fails.                                                                                   |
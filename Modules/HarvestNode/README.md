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

### Description`

`HarvestNode`s at their simplest are just `Levelable`s that can be progressed with `HarvestNodeUpdate`.
- `HarvestNode`s are linked to `Skill`s.
- `HarvestNode`s can update the `Inventory` via `InventoryUpdate` commands and their linked `LootTable`s.
- `HarvestNode`s can generate Items for both their `ResourceID` and `LocationID`.
- When locking a `HarvestNode` with `HarvestNodeRequirementsCreation`, it is not required to have that `HarvestNode` created. These services are separate. 

### Listens to

| Command                           | Requirements                      | Usage                                                                                                                                                                                                                                                                                        |
|-----------------------------------|-----------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `HarvestNodeCreation`             | None                              | A `HarvestNode[]`s are created with this command and linked with the commands `SkillID`.                                                                                                                                                                                                     |
| `HarvestNodeUpdate`               | `HarvestNodeCreation`             | Updates the `HarvestNode` in the command. This node must be linked with the `SkillID` also provided.                                                                                                                                                                                         |
| `HarvestNodeRequirementsCreation` | None                              | This command will lock `HarvestNode`s behind a `Skill` Level requirement. These nodes will be unable to be progressed with `HarvestNodeUpdate` until unlocked.                                                                                                                               |
| `HarvestNodeUnlock`               | `HarvestNodeRequirementsCreation` | Using the `SkillID` and `SkillLevel` this is queried against the `UnlockRequirementsEntity` to see if any nodes should be unlocked.                                                                                                                                                          |
| `ResourceLootCreation`            | None                              | This will create a `LootTable` and `GrantPolicy` using the properties of the command. Using this, any `HarvestNode` with matching `ResourceID` updated with `HarvestNodeUpdate` can now drop Items for the `Inventory`. The drop, and if the `Item` should drop, are defined by the command. |
| `LocationLootCreation`            | None                              | This will also create a `LootTable` and `GrantPolicy` using the command. Using this, another `LootTable` can be rolled when any `HarvestNode` has the matching `LocationID`. If a node has both matching `ResourceID` and `LocationID` then both `LootTable`s will be rolled.                |

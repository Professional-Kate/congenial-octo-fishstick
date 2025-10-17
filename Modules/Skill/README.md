# Modules/Skill
The `Skill` module defines playable skills and their progression mechanic in IdelPog. 

Each `Skill` contains a `Levelable` property, that tracks Experience, Level, Next Level 
Experience, and Experience Per Action. By dispatching the `SkillUpdate` this `Levelable`
will be updated according to the `Levelable.ExperiencePerAction` and automatically leveled.

---

## Contracts

### `Skill` model

```csharp
public record class Skill
{
    public required SkillID SkillID { get; init; }
    public required Levelable Levelable { get; init; }
    public required Information Information { get; init; }
}
```

- The `Levelable` will be updated on every update action.

---

## Description

Skills are simple in IdelPog. They just contain their `Information` and a `Levelable` which can be updated to progress the skill.

---

## Commands

### `SkillCreation`

````csharp
public readonly record struct SkillCreation
{
    public required SkillID SkillID { get; init; }
    public required ReadOnlyLevelable ReadOnlyLevelable { get; init; }
    public required Information Information { get; init; }
}
````

`SkillCreation` is used to create new `Skill`s with a specific `ReadOnlyLevelable`.

- Creation can fail if the `SkillID` already exists. `SkillID` must be unique.

| Buffered records        | Requirements                 | Description                                                        |
|-------------------------|------------------------------|--------------------------------------------------------------------|
| `SkillCreation`         | None                         | Creates new `Skill`s for each record.                              |
| `SkillCreationResponse` | Successful `SkillCreation`   | Each response will contain a newly created `Skill`.                |
| `SkillCreationError`    | Unsuccessful `SkillCreation` | Will be dispatched automatically whenever a `SkillCreation` fails. |

---

### `SkillUpdate``

```csharp
public readonly record struct SkillUpdate
{
    public required SkillID SkillID { get; init; }
}
```

`SkillUpdate` progresses the `Skill.Levelable`. Every update the `Levelable.ExperiencePerAction` will be applied to the `Levelable.Experience`.

- Updating can fail if the `SkillID` is not found.

| Buffered records      | Requirements               | Description                                                                                                                                                                                                                                                      |
|-----------------------|----------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `SkillUpdate`         | `SkillCreation`            | Will update a `Skill.Levelable` according to the `Levelable.ExperiencePerAction`. If the `Levelable.Experience` goes above the `Levelable.NextLevelExperience`, a level up will be triggered increasing the `Levelable.Level`. Will update one skill per record. |
| `SkillUpdateResponse` | Successful `SkillUpdate`   | Each response will contain the new state of the `Skill`s updated and a flag for if the `Skill` leveled with that update.                                                                                                                                         |
| `SkillUpdateError`    | Unsuccessful `SkillUpdate` | Will be dispatched automatically whenever a `SkillUpdate` fails.                                                                                                                                                                                                 |
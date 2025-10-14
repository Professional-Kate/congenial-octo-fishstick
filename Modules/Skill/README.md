# Modules/Skill
The `Skill` module defines playable skills and their progression mechanic in IdelPog. 

Each `Skill` contains a `Levelable` property, that tracks Experience, Level, Next Level 
Experience, and Experience Per Action. By dispatching the `SkillUpdate` this `Levelable`
will be updated according to the `Levelable.ExperiencePerAction` and automatically leveled.

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

### Description

Skills are simple in IdelPog. They just contain their `Information` and a `Levelable` which can be updated to progress the skill. Skills will be keyed by their `SkillID`.

### Listens to

| Command         | Requirements    | Usage                                                                                |
|-----------------|-----------------|--------------------------------------------------------------------------------------|
| `SkillCreation` | None            | Used To create `Skills` with the specified Levelable and Information.                |
| `SkillUpdate`   | `SkillCreation` | Used to update `Skills` by progressing their `Levelable` provided on `SkillCreation` |

### Dispatches 

| Command                 | When                         | Usage                                                                                                                                                     |
|-------------------------|------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------|
| `SkillCreationResponse` | Successful `SkillCreation`   | This response will be dispatched for each successful `SkillCreation` and defines the properties of the Skill created.                                     |
| `SkillCreationError`    | Unsuccessful `SkillCreation` | This will contain a `BaseError` giving details on why this error occured and a `SkillCreation[]` of the Buffer that caused it.                            |
| `SkillUpdateResponse`   | Successful `SkillUpdate`     | This response will contain the new state of the skills `Levelable` and a flag for if it leveled. Will dispatch one Response for each input `SkillUpdate`. |
| `SkillUpdateError`      | Unsuccessful `SkillUpdate`   | This will contain a `BaseError` giving details on why this error occured and a `SkillUpdate[]` of the Buffer that caused it.                              |
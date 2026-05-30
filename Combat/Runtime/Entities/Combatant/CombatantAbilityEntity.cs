using IdelPog.Combat.Contracts.Enum;
using IdelPog.ECS.Entity;

namespace IdelPog.Combat.Runtime.Entities.Combatant
{
    public sealed record CombatantAbilityEntity : Entity
    {
        public required byte CombatantID { get; init; }
        public required AbilityType AbilityType { get; init; }
    }
}
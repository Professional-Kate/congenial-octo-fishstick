using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Tests.TestFactory
{
    internal static class TestCombatantAbilityEntityFactory
    {
        internal static CombatantAbilityEntity Create(byte combatantID, AbilityType abilityType)
        {
            return new CombatantAbilityEntity { CombatantID = combatantID, AbilityType = abilityType };
        }

        internal static CombatantAbilityEntity CreateWithBaseComponents(byte combatantID, AbilityType abilityType)
        {
            CombatantAbilityEntity combatantAbilityEntity = Create(combatantID, abilityType);
            
            combatantAbilityEntity.AddComponent(new CooldownComponent { Cooldown = 1 });
            
            return combatantAbilityEntity;
        }
    }
}
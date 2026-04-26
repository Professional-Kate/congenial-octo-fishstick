using IdelPog.Combat.Contracts;
using IdelPog.Combat.Factory.Interface;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Factory
{
    public sealed class CombatantAbilityFactory : ICombatantAbilityFactory
    {
        public CombatantAbility CreateCombatantAbility(CombatantAbilityEntity combatantAbilityEntity)
        {
            return new CombatantAbility
            {
                AbilityType = combatantAbilityEntity.AbilityType,
                Damage = combatantAbilityEntity.GetComponent<DamageComponent>().Damage,
                Cooldown = combatantAbilityEntity.GetComponent<CooldownComponent>().Cooldown
            };
        }

        public CombatantAbility[] CreateCombatantAbilities(IReadOnlyList<CombatantAbilityEntity> combatantAbilityEntities)
        {
            CombatantAbility[] combatantAbilities = new CombatantAbility[combatantAbilityEntities.Count];
            for (int i = 0; i < combatantAbilityEntities.Count; i++)
            {
                combatantAbilities[i] = CreateCombatantAbility(combatantAbilityEntities[i]);
            }

            return combatantAbilities;
        }
    }
}
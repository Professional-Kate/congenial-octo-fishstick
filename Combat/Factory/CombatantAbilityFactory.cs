using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Card;
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
                DamageCard = CreateDamageCard(combatantAbilityEntity),
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

        private static DamageCard CreateDamageCard(CombatantAbilityEntity combatantAbilityEntity)
        {
            DamageComponent damageComponent = combatantAbilityEntity.GetComponent<DamageComponent>();

            return new DamageCard
            {
                PhysicalDamage = damageComponent.PhysicalDamage,
                LightningDamage = 0,
                FireDamage = 0,
                ColdDamage = 0
            };
        }
    }
}
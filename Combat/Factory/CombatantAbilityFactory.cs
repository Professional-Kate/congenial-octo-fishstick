using IdelPog.Combat.Contracts;
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
                ElementalDamageCard = CreateElementalDamageCard(combatantAbilityEntity),
                PhysicalDamageCard = CreatePhysicalDamageCard(combatantAbilityEntity),
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

        private static ElementalDamageCard CreateElementalDamageCard(CombatantAbilityEntity combatantAbilityEntity)
        {
            ElementalDamageComponent elementalDamageComponent = combatantAbilityEntity.GetComponent<ElementalDamageComponent>();

            return new ElementalDamageCard
            {
                LightningDamage = elementalDamageComponent.LightningDamage,
                FireDamage = elementalDamageComponent.FireDamage,
                ColdDamage = elementalDamageComponent.ColdDamage
            };
        }
        
        private static PhysicalDamageCard CreatePhysicalDamageCard(CombatantAbilityEntity combatantAbilityEntity)
        {
            PhysicalDamageComponent elementalDamageComponent = combatantAbilityEntity.GetComponent<PhysicalDamageComponent>();

            return new PhysicalDamageCard
            {
                StrikeDamage = elementalDamageComponent.StrikeDamage,
                SlashDamage = elementalDamageComponent.SlashDamage,
                ThrustDamage = elementalDamageComponent.ThrustDamage
            };
        }
    }
}
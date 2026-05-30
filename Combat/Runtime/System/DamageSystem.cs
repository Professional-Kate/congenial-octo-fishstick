using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class DamageSystem : IDamageSystem
    {
        public uint DealDamage(CombatantEntity targetCombatant, CombatantAbilityEntity attackerAbility)
        {
            StatsComponent targetStats = targetCombatant.GetComponent<StatsComponent>();
            
            uint newHealth = CalculateNewHealth(targetStats.Health, GetCalculatedDamage(attackerAbility));
            targetCombatant.ReplaceComponent(new StatsComponent { Health = newHealth });
            
            return newHealth;
        }

        public uint GetCalculatedDamage(CombatantAbilityEntity attackerAbility)
        {
            ElementalDamageComponent elementalDamageComponent = attackerAbility.GetComponent<ElementalDamageComponent>();
            PhysicalDamageComponent physicalDamageComponent = attackerAbility.GetComponent<PhysicalDamageComponent>();
            
            return elementalDamageComponent.TotalDamage + physicalDamageComponent.TotalDamage;
        }

        private static uint CalculateNewHealth(uint defenderHealth, uint calculatedDamage)
        {
            if (defenderHealth <= calculatedDamage)
            {
                return 0;
            }

            return defenderHealth - calculatedDamage;
        }
    }
}
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Logging.Contracts;
using IdelPog.Combat.Core.Logging.Interface;
using IdelPog.Combat.Stat.Contracts.Enum;

namespace IdelPog.Combat.Core.Logging
{
    public sealed class ReadOnlyCombatantFactory : IReadOnlyCombatantFactory
    {
        public ReadOnlyCombatant[] Create(CombatantEntity[] combatantEntities)
        {
            ReadOnlyCombatant[] readOnlyCombatants = new ReadOnlyCombatant[combatantEntities.Length];
            for (int i = 0; i < combatantEntities.Length; i++)
            {
                CombatantEntity combatantEntity = combatantEntities[i];
                readOnlyCombatants[i] = Create(combatantEntity);
            }
            
            return readOnlyCombatants;
        }

        private static ReadOnlyCombatant Create(CombatantEntity combatantEntity)
        {
            ReadOnlyCombatant readOnlyCombatant = new()
            {
                InstanceID = combatantEntity.InstanceID,
                CombatantID = combatantEntity.CombatantID,
                HealthCard = CreateStatCard(combatantEntity.GetStat(StatType.HEALTH), combatantEntity.GetStat(StatType.BASE_HEALTH)),
                AgilityCard = CreateAgilityCard(combatantEntity.GetStat(StatType.SPEED), combatantEntity.GetStat(StatType.INITIATIVE)),
                TargetingType = combatantEntity.TargetingType,
                IsAlive = combatantEntity.GetComponent<LifeStatusComponent>().IsAlive
            };
            
            return  readOnlyCombatant;
        }
        
        private static HealthCard CreateStatCard(uint health, uint baseHealth)
        {
            return new HealthCard
            {
                BaseHealth = baseHealth,
                Health = health
            };
        }

        private static AgilityCard CreateAgilityCard(uint speed, uint initiative)
        {
            return new AgilityCard
            {
                Speed = speed,
                Initiative = initiative
            };
        }
    }
}
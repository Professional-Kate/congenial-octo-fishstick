using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event;
using IdelPog.Combat.Runtime.System.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class CombatantAbilityInitializer : ICombatantAbilityInitializer
    {
        public void InitializeAbilities(CombatantEntity combatantEntity, IReadOnlyList<CombatantAbilityEntity> combatantAbilityEntities)
        { 
            InitializeRetaliationAbilities(combatantEntity, combatantAbilityEntities);
        }

        private static void InitializeRetaliationAbilities(CombatantEntity combatantEntity, IReadOnlyList<CombatantAbilityEntity> combatantAbilityEntities)
        {
            byte capacity = 0;
            foreach (CombatantAbilityEntity combatantAbilityEntity in combatantAbilityEntities)
            {
                foreach (CombatantAbilityStage combatantAbilityStage in combatantAbilityEntity.GetComponent<AbilityStagesComponent>().AbilityStages)
                {
                    if (combatantAbilityStage.AbilityStage.AbilityEffectType != AbilityEffectType.RETALIATION)
                    {
                        continue;
                    }

                    checked
                    {
                        capacity += combatantAbilityStage.AbilityStage.MaxTargets;
                    }
                }
            }

            if (capacity != 0)
            { 
                combatantEntity.AddComponent(new RetaliationComponent { Capacity = capacity });
            }
        }
    }
}
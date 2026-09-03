using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event;
using IdelPog.Combat.Runtime.System.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class AbilityInitializer : IAbilityInitializer
    {
        public void InitializeAbilities(CombatantEntity combatantEntity, IReadOnlyList<AbilityEntity> combatantAbilityEntities)
        { 
            InitializeRetaliationAbilities(combatantEntity, combatantAbilityEntities);
        }

        private static void InitializeRetaliationAbilities(CombatantEntity combatantEntity, IReadOnlyList<AbilityEntity> combatantAbilityEntities)
        {
            byte capacity = 0;
            foreach (AbilityEntity combatantAbilityEntity in combatantAbilityEntities)
            {
                foreach (AbilityStage combatantAbilityStage in combatantAbilityEntity.GetComponent<AbilityStagesComponent>().AbilityStages)
                {
                    if (combatantAbilityStage.AbilityStageCards.AbilityEffectType != AbilityEffectType.RETALIATION)
                    {
                        continue;
                    }

                    checked
                    {
                        capacity += combatantAbilityStage.AbilityStageCards.MaxTargets;
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
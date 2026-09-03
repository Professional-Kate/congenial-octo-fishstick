using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Event;

namespace IdelPog.Combat.Ability.Runtime.System
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
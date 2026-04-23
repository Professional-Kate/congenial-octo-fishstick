using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Skill;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Tests
{
    public static class CombatantCardFactory
    {
        public static CombatantCard CreateCombatantCard(CombatantType combatantType, StatCard statCard, Information information, params SkillCard[] skillCards)
        {
            return new CombatantCard
            {
                CombatantType = combatantType,
                Information = information,
                StatCard = statCard,
                SkillCards = skillCards
            };
        }
        
        public static CombatantCard CreateCombatantCard(CombatantType combatantType, StatCard statCard, Information information)
        {
            return CreateCombatantCard(combatantType, statCard, information, new SkillCard { SkillType = SkillType.BASIC_ATTACK, Strategy = new Strategy { TargetingType = TargetingType.HIGH_ATTACK }});
        }

        public static CombatantCard CreateCombatantCard(CombatantType combatantType, StatCard statCard)
        {
            return CreateCombatantCard(combatantType, statCard, new Information { Name = "", Description = "" });
        }
        
        public static CombatantCard CreateCombatantCard(CombatantType combatantType)
        {
            return CreateCombatantCard(combatantType, new StatCard { Health = 10, Attack = 5, Speed = 5 });
        }
    }
}
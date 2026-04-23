using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Skill;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Tests
{
    internal static class CombatantSkillCreationFactory
    {
        public static CombatantSkillCreation Create(SkillType skillType)
        {
            return Create(skillType, 25, 50);
        }
        
        public static CombatantSkillCreation Create(SkillType skillType, uint speed, uint damage)
        {
            return new CombatantSkillCreation
            {
                Information = new Information { Name = "", Description = "" },
                SkillType = skillType,
                Speed = speed,
                Damage = damage
            };
        }
    }
}
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Runtime.Entities;

namespace IdelPog.Combat.Runtime.System.Factory.Interface
{
    public interface ISkillEntityFactory
    {
        public SkillEntity CreateSkillEntity(CombatantSkillCreation combatantSkillCreation);
    }
}
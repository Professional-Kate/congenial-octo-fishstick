using IdelPog.Combat.Contracts.Skill;

namespace IdelPog.Combat.Runtime.System.Interface
{
    public interface ITargetFinder
    {
        public CombatantEntity FindBestTarget(CombatantEntity attackingEntity, SkillType skillType);
    }
}
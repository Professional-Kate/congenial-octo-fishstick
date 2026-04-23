using IdelPog.Combat.Contracts.Skill;

namespace IdelPog.Combat.Runtime.System.Interface
{
    public interface IEntityDamageMediator
    {
        public void ApplyDamage(byte attackingCombatantID, SkillType skillType);
    }
}
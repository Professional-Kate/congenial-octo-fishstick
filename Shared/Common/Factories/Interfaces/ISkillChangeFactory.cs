using IdelPog.Common.Commands;
using IdelPog.Common.Enums;

namespace IdelPog.Common.Factories
{
    public interface ISkillChangeFactory
    {
        public SetSkill CreateSkillChange(SkillID skillID);
    }
}
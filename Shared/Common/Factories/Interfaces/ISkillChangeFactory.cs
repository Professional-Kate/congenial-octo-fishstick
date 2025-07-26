using IdelPog.Common.Commands;
using IdelPog.Common.Enums;

namespace IdelPog.Common.Factories
{
    public interface ISkillChangeFactory
    {
        public SkillChange CreateSkillChange(SkillID skillID);
    }
}
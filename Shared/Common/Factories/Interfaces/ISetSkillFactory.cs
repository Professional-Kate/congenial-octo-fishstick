using IdelPog.Common.Commands;
using IdelPog.Common.Enums;

namespace IdelPog.Common.Factories
{
    public interface ISetSkillFactory
    {
        public SetSkill Create(SkillID skillID);
    }
}
using IdelPog.Common.Commands;
using IdelPog.Common.Responses;

namespace IdelPog.Common.Factories
{
    public interface ISkillChangeResponseFactory
    {
        public SkillChangeResponse Create(SkillChange skillChange);
    }
}
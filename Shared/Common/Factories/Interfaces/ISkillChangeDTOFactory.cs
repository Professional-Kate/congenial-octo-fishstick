using IdelPog.Common.Commands;
using IdelPog.Common.DTO;

namespace IdelPog.Common.Factories
{
    public interface ISkillChangeDTOFactory
    {
        public SkillChangeDTO Create(SkillChange skillChange);
    }
}
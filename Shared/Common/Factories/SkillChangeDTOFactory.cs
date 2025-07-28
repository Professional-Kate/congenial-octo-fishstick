using IdelPog.Common.Commands;
using IdelPog.Common.DTO;

namespace IdelPog.Common.Factories
{
    public class SkillChangeDTOFactory : ISkillChangeDTOFactory
    {
        public SkillChangeDTO Create(SkillChange skillChange)
        {
            return new SkillChangeDTO
            {
                SkillID = skillChange.SkillID
            };
        }
    }
}
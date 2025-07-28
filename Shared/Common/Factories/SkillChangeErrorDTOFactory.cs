using IdelPog.Common.Commands;
using IdelPog.Common.DTO.Error;

namespace IdelPog.Common.Factories
{
    public class SkillChangeErrorDTOFactory : IErrorFactory<SkillChangeErrorDTO, SkillChange>
    {
        private readonly IErrorDTOFactory _errorDTOFactory;
        private readonly ISkillChangeDTOFactory _skillChangeDTOFactory;

        public SkillChangeErrorDTOFactory(IErrorDTOFactory errorDTOFactory, ISkillChangeDTOFactory skillChangeDTOFactory)
        {
            _errorDTOFactory = errorDTOFactory;
            _skillChangeDTOFactory = skillChangeDTOFactory;
        }

        public SkillChangeErrorDTO Create<TException>(SkillChange context, TException exception) where TException : Exception
        {
            return new SkillChangeErrorDTO
            {
                ErrorDTO = _errorDTOFactory.Create(exception),
                SkillChangeDTO = _skillChangeDTOFactory.Create(context)
            };
        }
    }
}
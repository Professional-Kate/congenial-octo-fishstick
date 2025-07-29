using IdelPog.Common.Commands;
using IdelPog.Common.Errors;

namespace IdelPog.Common.Factories
{
    public class SkillChangeErrorDTOFactory : IErrorFactory<SkillChangeError, SkillChange>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;
        private readonly ISkillChangeResponseFactory _skillChangeResponseFactory;

        public SkillChangeErrorDTOFactory(IBaseErrorFactory baseErrorFactory, ISkillChangeResponseFactory skillChangeResponseFactory)
        {
            _baseErrorFactory = baseErrorFactory;
            _skillChangeResponseFactory = skillChangeResponseFactory;
        }

        public SkillChangeError Create<TException>(SkillChange context, TException exception) where TException : Exception
        {
            return new SkillChangeError
            {
                BaseError = _baseErrorFactory.Create(exception),
                SkillChangeResponse = _skillChangeResponseFactory.Create(context)
            };
        }
    }
}
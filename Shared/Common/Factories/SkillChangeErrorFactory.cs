using IdelPog.Common.Commands;
using IdelPog.Common.Errors;

namespace IdelPog.Common.Factories
{
    public class SkillChangeErrorFactory : IErrorFactory<SkillChangeError, SkillChange>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public SkillChangeErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public SkillChangeError Create<TException>(SkillChange context, TException exception) where TException : Exception
        {
            return new SkillChangeError
            {
                BaseError = _baseErrorFactory.Create(exception),
                SkillChange = context
            };
        }
    }
}
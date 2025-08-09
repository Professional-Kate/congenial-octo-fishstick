using IdelPog.Common.Commands;
using IdelPog.Common.Errors;

namespace IdelPog.Common.Factories
{
    public class SetSkillErrorFactory : IErrorFactory<SetSkillError, SetSkill>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public SetSkillErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public SetSkillError Create<TException>(SetSkill context, TException exception) where TException : Exception
        {
            return new SetSkillError
            {
                BaseError = _baseErrorFactory.Create(exception),
                SetSkill = context
            };
        }
    }
}
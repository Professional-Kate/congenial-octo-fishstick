using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Factory.Interface;

namespace IdelPog.Skill.Factory
{
    public class SetSkillErrorFactory : IErrorFactory<SetSkillError, SetSkill>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public SetSkillErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public SetSkillError Create<TException>(TException exception, SetSkill context) where TException : Exception
        {
            return new SetSkillError
            {
                BaseError = _baseErrorFactory.Create(exception),
                SetSkill = context
            };
        }
    }
}
using IdelPog.Core.Factory.Interface;
using IdelPog.Skill.Contracts.Command;
using IdelPog.Skill.Contracts.Error;

namespace IdelPog.Skill.Factory
{
    public class SkillUpdateErrorFactory : IErrorFactory<SkillUpdateError, IReadOnlyList<SkillUpdate>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public SkillUpdateErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public SkillUpdateError Create<TException>(TException exception, IReadOnlyList<SkillUpdate> context) where TException : Exception
        {
            return new SkillUpdateError
            {
                SkillUpdates = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}
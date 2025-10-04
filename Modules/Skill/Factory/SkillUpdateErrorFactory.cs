using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Factory.Interface;

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
using IdelPog.Core.Factory.Interface;
using IdelPog.Skill.Contracts.Command;
using IdelPog.Skill.Contracts.Error;

namespace IdelPog.Skill.Factory
{
    public sealed class SkillCreationErrorFactory : IErrorFactory<SkillCreationError, IReadOnlyList<SkillCreation>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public SkillCreationErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public SkillCreationError Create<TException>(TException exception, IReadOnlyList<SkillCreation> context) where TException : Exception
        {
            return new SkillCreationError
            {
                SkillCreations = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}
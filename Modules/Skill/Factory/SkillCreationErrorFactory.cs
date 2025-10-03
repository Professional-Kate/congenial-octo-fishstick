using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Factory.Interface;

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
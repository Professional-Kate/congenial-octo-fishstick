using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Error;
using IdelPog.Core.Factory.Interface;

namespace IdelPog.Combat.Factory
{
    public sealed class CombatantSkillCreationErrorFactory : IErrorFactory<CombatantSkillCreationError, IReadOnlyList<CombatantSkillCreation>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public CombatantSkillCreationErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public CombatantSkillCreationError Create<TException>(TException exception, IReadOnlyList<CombatantSkillCreation> context) where TException : Exception
        {
            return new CombatantSkillCreationError
            {
                CombatantSkillCreations = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}
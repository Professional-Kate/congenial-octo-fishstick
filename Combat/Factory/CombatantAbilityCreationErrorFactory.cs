using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Error;
using IdelPog.Core.Factory.Interface;

namespace IdelPog.Combat.Factory
{
    public sealed class CombatantAbilityCreationErrorFactory : IErrorFactory<CombatantAbilityCreationError, IReadOnlyList<CombatantAbilityCreation>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public CombatantAbilityCreationErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public CombatantAbilityCreationError Create<TException>(TException exception, IReadOnlyList<CombatantAbilityCreation> context) where TException : Exception
        {
            return new CombatantAbilityCreationError
            {
                CombatantAbilityCreations = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}
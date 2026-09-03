using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Combatant.Contracts.Error;
using IdelPog.Core.Factory.Interface;

namespace IdelPog.Combat.Factory
{
    public sealed class CombatantCreationErrorFactory : IErrorFactory<CombatantCreationError, IReadOnlyList<CombatantCreation>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public CombatantCreationErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public CombatantCreationError Create<TException>(TException exception, IReadOnlyList<CombatantCreation> context) where TException : Exception
        {
            return new CombatantCreationError
            {
                CombatantCreations = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}
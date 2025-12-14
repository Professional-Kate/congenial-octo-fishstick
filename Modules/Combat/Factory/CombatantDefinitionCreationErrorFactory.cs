using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Error;
using IdelPog.Core.Factory.Interface;

namespace IdelPog.Combat.Factory
{
    public sealed class CombatantDefinitionCreationErrorFactory : IErrorFactory<CombatantDefinitionCreationError, IReadOnlyList<CombatantDefinitionCreation>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public CombatantDefinitionCreationErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public CombatantDefinitionCreationError Create<TException>(TException exception, IReadOnlyList<CombatantDefinitionCreation> context) where TException : Exception
        {
            return new CombatantDefinitionCreationError
            {
                CombatantDefinitionsCreations = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}
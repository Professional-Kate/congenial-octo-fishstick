using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Error;
using IdelPog.Core.Factory.Interface;

namespace IdelPog.Combat.Factory
{
    public sealed class AbilityDefinitionCreationErrorFactory : IErrorFactory<AbilityDefinitionCreationError, IReadOnlyList<AbilityDefinitionCreation>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public AbilityDefinitionCreationErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public AbilityDefinitionCreationError Create<TException>(TException exception, IReadOnlyList<AbilityDefinitionCreation> context) where TException : Exception
        {
            return new AbilityDefinitionCreationError
            {
                AbilityDefinitionCreations = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}
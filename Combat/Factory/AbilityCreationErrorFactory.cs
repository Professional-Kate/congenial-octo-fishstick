using IdelPog.Combat.Ability.Contracts.Command;
using IdelPog.Combat.Ability.Contracts.Error;
using IdelPog.Core.Factory.Interface;

namespace IdelPog.Combat.Factory
{
    public sealed class AbilityCreationErrorFactory : IErrorFactory<AbilityCreationError, IReadOnlyList<AbilityCreation>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public AbilityCreationErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public AbilityCreationError Create<TException>(TException exception, IReadOnlyList<AbilityCreation> context) where TException : Exception
        {
            return new AbilityCreationError
            {
                AbilityCreations = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}
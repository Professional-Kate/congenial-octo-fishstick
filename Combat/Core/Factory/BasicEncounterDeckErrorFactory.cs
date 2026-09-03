using IdelPog.Combat.Core.Contracts.Command;
using IdelPog.Combat.Core.Contracts.Error;
using IdelPog.Core.Factory.Interface;

namespace IdelPog.Combat.Core.Factory
{
    public sealed class BasicEncounterDeckErrorFactory : IErrorFactory<BasicEncounterDeckError, IReadOnlyList<BasicEncounterDeck>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public BasicEncounterDeckErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public BasicEncounterDeckError Create<TException>(TException exception, IReadOnlyList<BasicEncounterDeck> context) where TException : Exception
        {
            return new BasicEncounterDeckError
            {
                BasicEncounterDecks = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}
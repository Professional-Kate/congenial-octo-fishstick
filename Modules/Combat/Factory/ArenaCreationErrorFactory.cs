using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Error;
using IdelPog.Core.Factory.Interface;

namespace IdelPog.Combat.Factory
{
    public sealed class ArenaCreationErrorFactory : IErrorFactory<ArenaCreationError, IReadOnlyList<ArenaCreation>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public ArenaCreationErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public ArenaCreationError Create<TException>(TException exception, IReadOnlyList<ArenaCreation> context) where TException : Exception
        {
            return new ArenaCreationError
            {
                ArenaCreations = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}
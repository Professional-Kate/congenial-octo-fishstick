using IdelPog.Core.Factory.Interface;
using IdelPog.Inventory.Contracts.Command;
using IdelPog.Inventory.Contracts.Error;

namespace IdelPog.Inventory.Factory
{
    public sealed class RecipeCreationErrorFactory : IErrorFactory<RecipeCreationError, IReadOnlyList<RecipeCreation>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public RecipeCreationErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public RecipeCreationError Create<TException>(TException exception, IReadOnlyList<RecipeCreation> context) where TException : Exception
        {
            return new RecipeCreationError
            {
                RecipeCreations = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}
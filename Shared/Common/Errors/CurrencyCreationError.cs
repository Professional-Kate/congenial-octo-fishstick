using IdelPog.Common.Commands;

namespace IdelPog.Common.Errors
{
    public readonly record struct CurrencyCreationError
    {
        public required CurrencyCreation[] CurrencyCreations { get; init; }
        public required BaseError BaseErrorDetails { get; init; }
    }
}
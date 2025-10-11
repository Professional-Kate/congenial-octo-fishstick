using IdelPog.Core.Contracts;
using IdelPog.Currency.Contracts.Command;

namespace IdelPog.Currency.Contracts.Error
{
    public readonly record struct CurrencyCreationError
    {
        public required CurrencyCreation[] CurrencyCreations { get; init; }
        public required BaseError BaseErrorDetails { get; init; }
    }
}
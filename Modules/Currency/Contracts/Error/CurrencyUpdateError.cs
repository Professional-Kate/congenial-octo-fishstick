using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;

namespace IdelPog.Currency.Contracts.Error
{
    public readonly record struct CurrencyUpdateError
    {
        public required CurrencyUpdate[] CurrencyUpdates { get; init; }
        public required BaseError BaseErrorDetails { get; init; }
    }
}
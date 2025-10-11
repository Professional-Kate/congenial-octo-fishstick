using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Error;

namespace IdelPog.Currency.Contracts.Error
{
    public readonly record struct CurrencyUpdateError
    {
        public required CurrencyUpdate[] CurrencyUpdates { get; init; }
        public required BaseError BaseErrorDetails { get; init; }
    }
}
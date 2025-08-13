using IdelPog.Core.Contracts.Command;

namespace IdelPog.Core.Contracts.Error
{
    public readonly record struct CurrencyUpdateError
    {
        public required CurrencyUpdate[] CurrencyUpdates { get; init; }
        public required BaseError BaseErrorDetails { get; init; }
    }
}
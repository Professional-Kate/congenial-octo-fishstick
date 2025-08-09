using IdelPog.Common.Commands;

namespace IdelPog.Common.Errors
{
    public readonly record struct CurrencyUpdateError
    {
        public required CurrencyUpdate[] CurrencyUpdates { get; init; }
        public required BaseError BaseErrorDetails { get; init; }
    }
}
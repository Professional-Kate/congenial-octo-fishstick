using IdelPog.Core.Contracts.Command;

namespace IdelPog.Core.Contracts.Response
{
    public readonly record struct CurrencyUpdateResponse
    {
        public required CurrencyUpdate[] CurrencyUpdates { get; init; }
    }
}
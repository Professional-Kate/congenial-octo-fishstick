using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Currency.Contracts.Response
{
    public readonly record struct CurrencyUpdateResponse
    {
        public required CurrencyType CurrencyType { get; init; }
        public required uint CurrencyAmount { get; init; }
    }
}
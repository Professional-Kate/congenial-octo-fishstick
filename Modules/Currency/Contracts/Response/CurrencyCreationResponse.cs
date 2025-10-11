using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Currency.Contracts.Response
{
    public readonly record struct CurrencyCreationResponse
    { 
        public required CurrencyType CurrencyType { get; init; }
        public required uint Amount { get; init; }
    }
}
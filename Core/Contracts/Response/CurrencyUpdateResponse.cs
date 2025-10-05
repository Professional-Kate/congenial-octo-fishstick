using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts.Response
{
    public readonly record struct CurrencyUpdateResponse
    {
        public required CurrencyType CurrencyType { get; init; }
        public required uint Amount { get; init; }
        public required ActionType ActionType { get; init; }
    }
}
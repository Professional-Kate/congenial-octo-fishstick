using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Console.Resolver.Currency
{
    public readonly record struct CurrencyUpdateArguments
    {
        public required ActionType ActionType { get; init; }
        public required uint Amount { get; init; }
        public required CurrencyType CurrencyType { get; init; }
    }
}
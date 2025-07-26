using IdelPog.Common.Enums;

namespace Console.Commands.Domains.Arguments
{
    public readonly record struct CurrencyUpdateArguments
    {
        public required ActionType ActionType { get; init; }
        public required int Amount { get; init; }
        public required CurrencyType CurrencyType { get; init; }
    }
}
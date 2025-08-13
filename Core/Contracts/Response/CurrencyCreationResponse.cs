using IdelPog.Core.Contracts.Command;

namespace IdelPog.Core.Contracts.Response
{
    public readonly record struct CurrencyCreationResponse
    {
        public required CurrencyCreation[] CurrencyCreations { get; init; }
    }
}
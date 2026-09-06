using IdelPog.Combat.Stat.Contracts.Command;

namespace IdelPog.Combat.Stat.Contracts.Response
{
    public readonly record struct StatConversionCreationResponse
    {
        public required byte StatConversionID { get; init; }
        public required StatConversionCreation StatConversionCreation { get; init; }
    }
}
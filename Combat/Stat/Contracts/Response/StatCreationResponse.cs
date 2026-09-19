using IdelPog.Combat.Stat.Contracts.Command;

namespace IdelPog.Combat.Stat.Contracts.Response
{
    public readonly record struct StatCreationResponse
    {
        public required byte StatID { get; init; }
        public required StatCreation StatCreation { get; init; }
    }
}
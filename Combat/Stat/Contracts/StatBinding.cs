using IdelPog.Combat.Stat.Contracts.Enum;

namespace IdelPog.Combat.Stat.Contracts
{
    public readonly record struct StatBinding
    {
        public required StatType StatType { get; init; }
        public required byte LinkedID { get; init; }
    }
}
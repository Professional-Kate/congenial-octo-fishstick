using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts.Command
{
    public readonly record struct HarvestNodeUpdate
    {
        public required ResourceID ResourceID { get; init; }
        public required SkillID SkillID { get; init; }
    }
}
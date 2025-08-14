using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Progression;

namespace IdelPog.Core.Contracts.Response
{
    public readonly record struct HarvestNodeUpdateResponse
    {
        public required ResourceID ResourceID { get; init; }
        public required LevelProgress LevelProgress { get; init; }
        public required bool HasLeveled { get; init; }
    }
}
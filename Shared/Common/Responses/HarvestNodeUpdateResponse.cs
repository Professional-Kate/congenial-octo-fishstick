using IdelPog.Common.Enums;
using IdelPog.Common.Structures;

namespace IdelPog.Common.Responses
{
    public readonly record struct HarvestNodeUpdateResponse
    {
        public required ResourceID ResourceID { get; init; }
        public required LevelProgress Levelable { get; init; }
        public required bool HasLeveled { get; init; }
    }
}
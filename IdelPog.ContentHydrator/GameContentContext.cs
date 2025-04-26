using System.Text.Json.Serialization;
using ContentHydrator.DTO;

namespace ContentHydrator
{
    [JsonSerializable(typeof(InformationDTO))]
    [JsonSerializable(typeof(JobDTO))]
    public partial class GameContentContext : JsonSerializerContext;
}
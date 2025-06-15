using System.Text.Json.Serialization;
using IdelPog.ContentHydrator.DTO;

namespace IdelPog.ContentHydrator
{
    [JsonSerializable(typeof(InformationDTO))]
    [JsonSerializable(typeof(JobDTO))]
    public partial class GameContentContext : JsonSerializerContext;
}
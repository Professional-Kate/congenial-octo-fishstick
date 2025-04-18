using System.Text.Json.Serialization;
using ContentHydrator.DTO;

namespace ContentHydrator.SerializerContexts
{
    [JsonSerializable(typeof(InformationDTO))]
    [JsonSerializable(typeof(JobDTO))]
    public partial class JobHydrationContext : JsonSerializerContext;
}
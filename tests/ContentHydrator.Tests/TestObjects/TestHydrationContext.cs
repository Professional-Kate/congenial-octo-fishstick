using System.Text.Json.Serialization;

namespace ContentHydratorTests.TestObjects
{
    [JsonSerializable(typeof(TestDTO))]
    internal partial class TestHydrationContext : JsonSerializerContext;
}
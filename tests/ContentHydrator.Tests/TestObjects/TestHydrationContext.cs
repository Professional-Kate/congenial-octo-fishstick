using System.Text.Json.Serialization;

namespace ContentHydratorTests.TestObjects
{
    [JsonSerializable(typeof(TestObject))]
    internal partial class TestHydrationContext : JsonSerializerContext;
}
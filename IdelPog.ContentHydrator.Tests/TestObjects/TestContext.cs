using System.Text.Json.Serialization;

namespace ContentHydratorTests.TestObjects
{
    [JsonSerializable(typeof(TestDTO))]
    public partial class TestContext : JsonSerializerContext;
}
using System.Text.Json.Serialization;

namespace ContentHydratorTests.TestObjects
{
    [JsonSerializable(typeof(TestObject))]
    public partial class TestContext : JsonSerializerContext;
}
using System.Text.Json.Serialization;

namespace IdelPog.Content.Tests.TestObjects
{
    [JsonSerializable(typeof(TestObject))]
    public partial class TestContext : JsonSerializerContext;
}
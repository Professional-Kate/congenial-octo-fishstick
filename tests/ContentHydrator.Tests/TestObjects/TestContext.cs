using System.Text.Json.Serialization;

namespace IdelPog.ContentHydrator.Tests.TestObjects
{
    [JsonSerializable(typeof(TestObject))]
    public partial class TestContext : JsonSerializerContext;
}
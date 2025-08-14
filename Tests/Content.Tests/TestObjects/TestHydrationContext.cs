using System.Text.Json.Serialization;

namespace IdelPog.Content.Tests.TestObjects
{
    [JsonSerializable(typeof(TestObject))]
    internal partial class TestHydrationContext : JsonSerializerContext;
}
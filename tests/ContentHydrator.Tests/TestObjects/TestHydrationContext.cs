using System.Text.Json.Serialization;

namespace IdelPog.ContentHydrator.Tests.TestObjects
{
    [JsonSerializable(typeof(TestObject))]
    internal partial class TestHydrationContext : JsonSerializerContext;
}
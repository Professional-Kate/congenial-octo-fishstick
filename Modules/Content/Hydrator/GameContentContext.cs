using System.Text.Json.Serialization;
using IdelPog.Core.Contracts.Command;

namespace IdelPog.Content.Hydrator
{
    [JsonSerializable(typeof(CurrencyCreation))]
    public partial class GameContentContext : JsonSerializerContext;
}
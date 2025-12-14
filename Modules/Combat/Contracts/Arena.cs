using IdelPog.Combat.Contracts.Enum;
using IdelPog.Core.Contracts;
using IdelPog.Core.Progression;

namespace IdelPog.Combat.Contracts
{
    public sealed record class Arena : ICloneable<Arena>
    {
        public required ArenaType ArenaType { get; init; }
        public required Levelable Levelable { get; init; }
        public required Information Information { get; init; }
        
        public Arena DeepClone()
        {
            return this with { Levelable = Levelable.DeepClone() };
        }
    }
}
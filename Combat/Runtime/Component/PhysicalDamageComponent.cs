using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component
{
    public readonly record struct PhysicalDamageComponent : IComponent
    {
        public required uint StrikeDamage { get; init; }
        public required uint SlashDamage { get; init; }
        public required uint ThrustDamage { get; init; }
        
        /// <summary>
        /// Gets the sum of all damage types
        /// </summary>
        public uint TotalDamage => StrikeDamage + SlashDamage + ThrustDamage;
    }
}
using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component
{
    public readonly record struct ElementalDamageComponent : IComponent
    {
        public required uint LightningDamage { get; init; }
        public required uint ColdDamage { get; init; }
        public required uint FireDamage { get; init; }
        
        /// <summary>
        /// Gets the sum of all damage types
        /// </summary>
        public uint TotalDamage => LightningDamage + ColdDamage + FireDamage;
    }
}
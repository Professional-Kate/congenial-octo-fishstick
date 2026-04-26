
using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component.Abilities.Interface
{
    public interface IAbilityComponent : IComponent
    {
        public uint Cooldown { get; }
    }
}

using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component.Skills.Interface
{
    public interface IAbilityComponent : IComponent
    {
        public uint Speed { get; }
    }
}
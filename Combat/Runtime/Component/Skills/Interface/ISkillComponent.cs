
using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component.Skills.Interface
{
    public interface ISkillComponent : IComponent
    {
        public uint Speed { get; }
    }
}
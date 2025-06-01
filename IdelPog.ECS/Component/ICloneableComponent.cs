using IdelPog.Infrastructure.Structures;

namespace IdelPog.ECS.Component
{
    public interface ICloneableComponent<out T> : IComponent, ICloneable<T> where T : ICloneableComponent<T>;
}
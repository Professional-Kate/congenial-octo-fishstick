using IdelPog.Infrastructure.Structures;

namespace IdelPog.ECS.Component
{
    public interface ICloneableComponent<T> : IComponent, ICloneable<T> where T : ICloneableComponent<T>
    {
    }
}
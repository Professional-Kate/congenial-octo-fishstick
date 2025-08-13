using IdelPog.Core.Contracts;

namespace IdelPog.ECS.Component
{
    public interface IComponent<out TComponent> : IComponent, ICloneable<TComponent>;

    public interface IComponent;
}
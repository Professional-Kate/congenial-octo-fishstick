namespace IdelPog.ECS.Component
{
    public interface IComponent<out TComponent> : IComponent
    {
        public TComponent CloneComponent();
    }

    public interface IComponent;
}
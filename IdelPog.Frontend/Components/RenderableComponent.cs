using Frontend.Rendering;
using IdelPog.ECS.Component;

namespace Frontend.Components
{
    public class RenderableComponent : IComponent
    {
        public readonly RenderEntity RenderEntity;

        public RenderableComponent(RenderEntity renderEntity)
        {
            RenderEntity = renderEntity;
        }
    }
}
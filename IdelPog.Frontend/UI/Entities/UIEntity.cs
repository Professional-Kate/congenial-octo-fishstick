using IdelPog.ECS;
using IdelPog.ECS.Component;
using IdelPog.Validation.Assertions.Handlers;

namespace Frontend.UI
{
    public sealed record UIEntity : Entity
    {
        public UIEntity(RenderableComponent[] renderableComponents) 
            : base(new ComponentStore<RenderableComponent>(renderableComponents, new ThrowHandler()))
        {
            
        }
    }
}
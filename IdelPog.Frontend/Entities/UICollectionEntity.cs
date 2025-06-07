using Frontend.Components;
using IdelPog.ECS;
using IdelPog.ECS.Component;
using IdelPog.Validation.Assertions.Handlers;

namespace Frontend.Entities
{
    public sealed record UICollectionEntity : Entity
    {
        public UICollectionEntity(RenderableComponent[] renderableComponents) 
            : base(new ComponentStore<RenderableComponent>(renderableComponents, new ThrowHandler()))
        {
            
        }
    }
}
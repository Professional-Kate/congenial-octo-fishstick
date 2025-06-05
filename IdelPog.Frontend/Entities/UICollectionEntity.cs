using System;
using Frontend.Components;
using Frontend.Rendering;
using IdelPog.ECS;
using IdelPog.ECS.Component;
using IdelPog.ECS.Component.Store;
using IdelPog.Infrastructure.Repository;
using IdelPog.Validation.Assertions.Handlers;

namespace Frontend.Entities
{
    public sealed record UICollectionEntity : Entity
    {
        public UICollectionEntity(IRepository<Type, IComponent> components, IHandler handler) : base(components, handler)
        {
            AddRequiredComponents();
        }

        protected override void AddRequiredComponents()
        {
            AddComponent(new ImmutableComponentStore<RenderableComponent>([], Handler));
        }
    }
}
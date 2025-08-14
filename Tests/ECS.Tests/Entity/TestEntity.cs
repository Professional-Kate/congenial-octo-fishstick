using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.ECS.Component;

namespace IdelPog.ECS.Tests.Entity
{
    internal record TestEntity : ECS.Entity.Entity
    {
        public TestEntity(IAssetRepository<Type, IComponent> components, IHandler handler)
            : base(components, handler)
        {
        }
    }
}
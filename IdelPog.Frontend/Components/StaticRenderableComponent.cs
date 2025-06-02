using Frontend.Rendering;
using IdelPog.ECS.Component;

namespace Frontend.Components
{
    public readonly struct StaticRenderableComponent(RenderEntity renderEntity) : ICloneableComponent<StaticRenderableComponent>
    {
        public RenderEntity GetRenderEntity => _cloneRenderEntity;
        
        public StaticRenderableComponent Clone()
        {
            return new StaticRenderableComponent(_cloneRenderEntity);
        }
        
        private RenderEntity _cloneRenderEntity => renderEntity with { };
    }
}
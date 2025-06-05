using Frontend.Enums;
using IdelPog.ECS.Component;

namespace Frontend.Components
{
    public readonly record struct RenderableComponent : IComponent<RenderableComponent>
    {
        public required TextureID TextureID { get; init; }
        
        public RenderableComponent CloneComponent()
        {
            return new RenderableComponent { TextureID = TextureID };
        }
    }
}
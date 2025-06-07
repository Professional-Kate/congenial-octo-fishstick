using Frontend.Rendering.Structures.Enums;
using IdelPog.ECS.Component;

namespace Frontend.UI
{
    public readonly record struct RenderableComponent : IComponent<RenderableComponent>
    {
        public required TextureID TextureID { get; init; }
        
        public RenderableComponent DeepClone()
        {
            return new RenderableComponent { TextureID = TextureID };
        }
    }
}
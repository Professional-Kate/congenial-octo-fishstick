using IdelPog.ECS.Component;
using IdelPog.Frontend.Rendering.Structures.Enums;

namespace IdelPog.Frontend.UI
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
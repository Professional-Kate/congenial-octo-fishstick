using Frontend.Enums;
using IdelPog.ECS.Component;

namespace Frontend.Components
{
    public readonly record struct RenderableComponent : IComponent
    {
        public required TextureID TextureID { get; init; }
    }
}
using IdelPog.Frontend.Rendering.Structures.Enums;

namespace IdelPog.Frontend.UI.Structures
{
    public readonly record struct RenderableDTO
    {
        public required TextureID TextureID { get; init; }
        public required Transform Transform { get; init; }
    }
}
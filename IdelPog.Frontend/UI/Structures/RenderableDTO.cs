using Frontend.Rendering.Structures.Enums;

namespace Frontend.UI.Structures
{
    public readonly record struct RenderableDTO
    {
        public required TextureID TextureID { get; init; }
        public required Transform Transform { get; init; }
    }
}
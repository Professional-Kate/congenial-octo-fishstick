using IdelPog.Common.Structures;

namespace IdelPog.Common.DTO
{
    public readonly record struct RenderableDTO
    {
        public required TextureID TextureID { get; init; }
        public required Transform Transform { get; init; }
    }
}
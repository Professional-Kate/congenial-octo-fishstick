using System.Numerics;

namespace Frontend.UI.Structures
{
    public readonly record struct Transform
    {
        public Vector2 Position { get; init; }
        public Vector2 Size { get; init; }
        public float Z { get; init; }
    }
}
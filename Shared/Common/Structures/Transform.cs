using System.Numerics;

namespace IdelPog.Common.Structures
{
    public readonly record struct Transform
    {
        public Vector2 Position { get; init; }
        public Vector2 Size { get; init; }
        public float Z { get; init; }
    }
}
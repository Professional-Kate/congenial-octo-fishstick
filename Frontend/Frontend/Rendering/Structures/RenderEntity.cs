using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = System.Numerics.Vector2;

namespace IdelPog.Frontend.Rendering.Structures
{
    public sealed record RenderEntity
    {
        public required Texture2D Texture { get; init; }
        public required Vector2 Position { get; init; }
        public required float Z { get; init; }
        
        /// <summary>
        /// Optional parameter to define a scale multiplier for the texture.
        /// If not provided, will default to <see cref="Vector2.One"/> 
        /// </summary>
        /// <remarks>
        /// Using <see cref="Vector2.One"/> as the default means the texture is drawn at its original width and height </remarks>
        public Vector2 Size { get; init; } = Vector2.One;
        
        public Color Tint { get; init; } = Color.White;
    }
}
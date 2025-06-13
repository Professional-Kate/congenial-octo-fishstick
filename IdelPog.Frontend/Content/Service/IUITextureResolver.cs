using Frontend.Rendering.Structures.Enums;
using SharpDX.Direct3D11;

namespace Frontend.Content.Service
{
    public interface IUITextureResolver
    {
        /// <summary>
        /// Returns a Texture using its linked <see cref="TextureID"/>
        /// </summary>
        /// <param name="textureID">The element you want should have this id</param>
        /// <returns>The Texture2D matching the passed id</returns>
        /// <remarks>
        /// This will not validate if the key is in the collection.
        /// </remarks>
        public Texture2D GetTexture(TextureID textureID);
    }
}
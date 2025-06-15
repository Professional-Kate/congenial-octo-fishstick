using IdelPog.ECS;
using IdelPog.Frontend.Rendering.Structures.Enums;
using IdelPog.Frontend.UI.Structures;

namespace IdelPog.Frontend.UI
{
    public sealed record UIEntity : Entity
    {
        public UIEntity(TextureID textureID, Transform transform) 
            : base(new RenderableComponent {TextureID = textureID}, new TransformComponent {Transform = transform})
        {
            
        }
    }
}
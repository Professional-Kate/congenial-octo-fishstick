using Frontend.Rendering.Structures.Enums;
using Frontend.UI.Structures;
using IdelPog.ECS;

namespace Frontend.UI
{
    public sealed record UIEntity : Entity
    {
        public UIEntity(TextureID textureID, Transform transform) 
            : base(new RenderableComponent {TextureID = textureID}, new TransformComponent {Transform = transform})
        {
            
        }
    }
}
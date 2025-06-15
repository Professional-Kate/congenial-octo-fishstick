using IdelPog.Common.Structures;
using IdelPog.ECS;

namespace IdelPog.SimulationEngine.UI
{
    public sealed record UIEntity : Entity
    {
        public UIEntity(TextureID textureID, Transform transform) 
            : base(new RenderableComponent {TextureID = textureID}, new TransformComponent {Transform = transform})
        {
            
        }
    }
}
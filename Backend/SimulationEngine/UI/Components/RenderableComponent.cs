using IdelPog.Common.Structures;
using IdelPog.ECS.Component;

namespace IdelPog.SimulationEngine.UI
{
    public readonly record struct RenderableComponent : IComponent<RenderableComponent>
    {
        public required TextureID TextureID { get; init; }
        
        public RenderableComponent DeepClone()
        {
            return new RenderableComponent { TextureID = TextureID };
        }
    }
}
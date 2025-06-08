using Frontend.UI.Structures;
using IdelPog.ECS.Component;

namespace Frontend.UI
{
    public readonly record struct TransformComponent : IComponent<TransformComponent>
    {
        public required Transform Transform { get; init; }
        
        public TransformComponent DeepClone()
        {
            return new TransformComponent { Transform = Transform };
        }
    }
}
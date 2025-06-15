using IdelPog.ECS.Component;
using IdelPog.Frontend.UI.Structures;

namespace IdelPog.Frontend.UI
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
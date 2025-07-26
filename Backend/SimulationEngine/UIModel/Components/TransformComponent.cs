using IdelPog.Common.Structures;
using IdelPog.ECS.Component;

namespace IdelPog.SimulationEngine.UIModel.Components
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
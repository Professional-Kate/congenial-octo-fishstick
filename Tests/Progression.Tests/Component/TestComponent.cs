using IdelPog.ECS.Component;

namespace IdelPog.Progression.Tests.Component
{
    public readonly record struct TestComponent : IComponent<TestComponent>
    { 
        public required int Index { get; init; }
        
        public TestComponent DeepClone()
        {
            return new TestComponent { Index = Index };
        }
    }
}
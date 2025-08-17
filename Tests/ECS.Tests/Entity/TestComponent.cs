using IdelPog.ECS.Component;

namespace IdelPog.ECS.Tests
{
    public readonly record struct TestComponent : IComponent<TestComponent>
    {
        public int TestNumber { get; init; }

        public TestComponent DeepClone()
        {
            return new TestComponent { TestNumber = TestNumber };
        }
    }
}
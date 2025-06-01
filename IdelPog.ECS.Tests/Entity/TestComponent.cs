using IdelPog.ECS.Component;

namespace IdelPog.ECS.Tests
{
    public class TestComponent : ICloneableComponent<TestComponent>
    {
        public int TestNumber { get; set; }
        
        public TestComponent Clone()
        {
            return new TestComponent { TestNumber = TestNumber };
        }
    }
}
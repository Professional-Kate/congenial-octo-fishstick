using IdelPog.Model;

namespace IdelPog.Validation.Assertions.Interfaces
{
    public interface IAssertUnderMaxLevel
    {
        public void AssertLevelIsUnderMax(Job levelable);
    }
}
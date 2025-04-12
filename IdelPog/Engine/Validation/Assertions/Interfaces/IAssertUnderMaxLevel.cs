using IdelPog.Engine.Structures.Models;

namespace IdelPog.Engine.Validation.Assertions
{
    public interface IAssertUnderMaxLevel
    {
        public void AssertLevelIsUnderMax(ILevelable levelable);
    }
}
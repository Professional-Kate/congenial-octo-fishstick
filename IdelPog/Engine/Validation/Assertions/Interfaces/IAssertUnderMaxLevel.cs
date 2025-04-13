using IdelPog.Engine.Models;

namespace IdelPog.Engine.Validation.Assertions
{
    public interface IAssertUnderMaxLevel
    {
        public void AssertLevelIsUnderMax(ILevelable levelable);
    }
}
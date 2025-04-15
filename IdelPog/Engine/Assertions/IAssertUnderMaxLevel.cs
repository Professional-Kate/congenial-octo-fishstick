using IdelPog.Engine.Models;

namespace IdelPog.Engine.Assertions
{
    public interface IAssertUnderMaxLevel
    {
        public void AssertLevelIsUnderMax(ILevelable levelable);
    }
}
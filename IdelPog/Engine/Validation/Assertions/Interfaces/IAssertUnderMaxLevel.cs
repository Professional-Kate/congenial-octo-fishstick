using IdelPog.Engine.Structures.Models.Levelable;

namespace IdelPog.Engine.Validation.Assertions.Interfaces
{
    public interface IAssertUnderMaxLevel
    {
        public void AssertLevelIsUnderMax(ILevelable levelable);
    }
}
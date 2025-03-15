using IdelPog.Structures.Models.Levelable;

namespace IdelPog.Validation.Assertions.Interfaces
{
    public interface IAssertUnderMaxLevel
    {
        public void AssertLevelIsUnderMax(ILevelable levelable);
    }
}
using IdelPog.Main.Structures.Models.Levelable;

namespace IdelPog.Main.Validation.Assertions.Interfaces
{
    public interface IAssertUnderMaxLevel
    {
        public void AssertLevelIsUnderMax(ILevelable levelable);
    }
}
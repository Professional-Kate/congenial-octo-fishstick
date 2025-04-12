using IdelPogTemp.Main.Structures.Models.Levelable;

namespace IdelPogTemp.Main.Validation.Assertions.Interfaces
{
    public interface IAssertUnderMaxLevel
    {
        public void AssertLevelIsUnderMax(ILevelable levelable);
    }
}
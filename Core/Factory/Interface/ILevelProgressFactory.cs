using IdelPog.Core.Progression;

namespace IdelPog.Core.Factory.Interface
{
    public interface ILevelProgressFactory
    {
        public ReadOnlyLevelable CreateLevelProgress(Levelable levelable);
    }
}
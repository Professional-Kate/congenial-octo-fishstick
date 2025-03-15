using IdelPog.Structures.Models.Levelable;

namespace IdelPog.Structures.Builders
{
    /// <summary>
    /// Builds a new <see cref="ILevelable"/>
    /// </summary>
    /// <seealso cref="Level"/>
    /// <seealso cref="Experience"/>
    /// <seealso cref="NextLevelExperience"/>
    /// <seealso cref="ExperiencePerAction"/>
    /// <seealso cref="Build"/>
    public interface ILevelableBuilder
    {
        public ILevelableBuilder Level(byte level);

        public ILevelableBuilder Experience(int experience);

        public ILevelableBuilder NextLevelExperience(int nextLevelExperience);

        public ILevelableBuilder ExperiencePerAction(int experiencePerAction);

        public ILevelable Build();
    }
}
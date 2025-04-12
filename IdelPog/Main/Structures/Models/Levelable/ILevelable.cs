namespace IdelPogTemp.Main.Structures.Models.Levelable
{
    /// <summary>
    /// This is the main progression object. 
    /// </summary>
    /// <seealso cref="LevelUp"/>
    /// <seealso cref="SetExperience"/>
    /// <seealso cref="SetExperiencePerAction"/>
    /// <seealso cref="SetNextLevelExperience"/>
    public interface ILevelable
    {
        public byte Level { get; }
        public int Experience { get; } 
        public int NextLevelExperience { get; }
        public int ExperiencePerAction { get; }

        /// <summary>
        /// Will increase the <see cref="ILevelable.Level"/> of this <see cref="ILevelable"/> by one
        /// </summary>
        public void LevelUp();

        public void SetExperience(int experience);
        
        public void SetExperiencePerAction(int experiencePerAction);
        
        public void SetNextLevelExperience(int nextLevelExperience);
    }
}
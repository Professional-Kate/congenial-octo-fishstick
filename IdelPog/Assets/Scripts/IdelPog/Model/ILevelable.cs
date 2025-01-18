namespace IdelPog.Model
{
    /// <seealso cref="Setup"/>
    /// <seealso cref="AddExperience"/>
    /// <seealso cref="SetExperiencePerAction"/>
    /// <seealso cref="LevelUp"/>
    public interface ILevelable
    {
        /// <summary>
        /// Set up an <see cref="ILevelable"/> Object. This can be used by the base <see cref="Job"/> by update on game start
        /// </summary>
        /// <param name="level">The new level</param>
        /// <param name="experience">The new experience amount</param>
        /// <param name="experienceToNextLevel">The new experience needed to reach next level</param>
        /// <param name="experiencePerAction">The new experience gained per action</param>
        public void Setup(byte level, int experience, int experienceToNextLevel, int experiencePerAction);
        
        /// <param name="experience">The amount of experience to add</param>
        public void AddExperience(int experience);

        /// <param name="experiencePerAction">The new experience gained per action</param>
        public void SetExperiencePerAction(int experiencePerAction);

        /// <summary>
        /// Increases this objects <see cref="Levelable.Level"/> and sets a new experience needed to reach the next level
        /// </summary>
        /// <param name="experienceToNextLevel">This new experience needed to reach the next level</param>
        public void LevelUp(int experienceToNextLevel);
    }
}
namespace IdelPog.Model
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="AddExperience"/>
    /// <seealso cref="SetExperiencePerAction"/>
    /// <seealso cref="LevelUp"/>
    public interface ILevelable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="experience"></param>
        /// <param name="experienceToNextLevel"></param>
        /// <param name="experiencePerAction"></param>
        public void Setup(byte level, int experience, int experienceToNextLevel, int experiencePerAction);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="experience"></param>
        public void AddExperience(int experience);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="experiencePerAction"></param>
        public void SetExperiencePerAction(int experiencePerAction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="experienceToNextLevel"></param>
        public void LevelUp(int experienceToNextLevel);
    }
}
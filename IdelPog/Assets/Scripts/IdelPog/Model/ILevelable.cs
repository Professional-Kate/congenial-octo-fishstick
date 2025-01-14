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
        public void Setup(byte level, uint experience, uint experienceToNextLevel, uint experiencePerAction);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="experience"></param>
        public void AddExperience(uint experience);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="experiencePerAction"></param>
        public void SetExperiencePerAction(uint experiencePerAction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="experienceToNextLevel"></param>
        public void LevelUp(uint experienceToNextLevel);
    }
}
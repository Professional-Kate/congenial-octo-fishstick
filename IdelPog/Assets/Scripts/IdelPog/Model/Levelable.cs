namespace IdelPog.Model
{
    public abstract class Levelable : ILevelable
    {
        public byte Level { get; private set; }

        public uint Experience { get; private set; } 
        public uint ExperienceToNextLevel { get; private set; } = 10;
        public uint ExperiencePerAction { get; private set; } = 1;

        public void Setup(byte level, uint experience, uint experienceToNextLevel, uint experiencePerAction)
        {
            Level = level;
            Experience = experience;
            ExperienceToNextLevel = experienceToNextLevel;
            ExperiencePerAction = experiencePerAction;
        }

        public void AddExperience(uint experience)
        {
            Experience += experience;
        }

        public void SetExperiencePerAction(uint experiencePerAction)
        {
            ExperiencePerAction = experiencePerAction;
        }

        public void LevelUp(uint experienceToNextLevel)
        {
            Level++;
            ExperienceToNextLevel = experienceToNextLevel;
        }
    }
}
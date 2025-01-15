namespace IdelPog.Model
{
    public abstract class Levelable : ILevelable
    {
        public byte Level { get; private set; }

        public int Experience { get; private set; } 
        public int ExperienceToNextLevel { get; private set; } = 10;
        public int ExperiencePerAction { get; private set; } = 1;

        public void Setup(byte level, int experience, int experienceToNextLevel, int experiencePerAction)
        {
            Level = level;
            Experience = experience;
            ExperienceToNextLevel = experienceToNextLevel;
            ExperiencePerAction = experiencePerAction;
        }

        public void AddExperience(int experience)
        {
            Experience += experience;
        }

        public void SetExperiencePerAction(int experiencePerAction)
        {
            ExperiencePerAction = experiencePerAction;
        }

        public void LevelUp(int experienceToNextLevel)
        {
            Level++;
            ExperienceToNextLevel = experienceToNextLevel;
        }
    }
}
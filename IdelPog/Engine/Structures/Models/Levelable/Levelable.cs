namespace IdelPog.Engine.Structures.Models.Levelable
{
    /// <inheritdoc cref="ILevelable"/>
    public class Levelable : ILevelable
    {
        public event Action<byte> OnLevelUp;
        
        public byte Level { get; private set; }
        public int Experience { get; private set; }
        public int NextLevelExperience { get; private set; }
        public int ExperiencePerAction { get; private set; }

        public Levelable(byte level, int experience, int nextLevelExperience, int experiencePerAction, Action<byte> onLevelUp)
        {
            Level = level;
            Experience = experience;
            NextLevelExperience = nextLevelExperience;
            ExperiencePerAction = experiencePerAction;
            OnLevelUp = onLevelUp;
        }

        public void LevelUp()
        {
            Level++;
            OnLevelUp?.Invoke(Level);
        }
       
        public void SetExperience(int experience)
        {
            Experience += experience;
        }

        public void SetExperiencePerAction(int experiencePerAction)
        {
            ExperiencePerAction += experiencePerAction;
        }

        public void SetNextLevelExperience(int nextLevelExperience)
        {
            NextLevelExperience += nextLevelExperience;
        }
    }
}
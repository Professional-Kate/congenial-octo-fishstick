namespace IdelPog.SimulationEngine.Models
{
    /// <inheritdoc cref="ILevelable"/>
    public class Levelable(byte level, int experience, int nextLevelExperience, int experiencePerAction)
        : ILevelable
    {
        public event Action<byte> OnLevelUp = delegate { };

        public byte Level { get; private set; } = level;
        public int Experience { get; private set; } = experience;
        public int NextLevelExperience { get; private set; } = nextLevelExperience;
        public int ExperiencePerAction { get; private set; } = experiencePerAction;

        public void LevelUp()
        {
            Level++;
            OnLevelUp(Level);
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
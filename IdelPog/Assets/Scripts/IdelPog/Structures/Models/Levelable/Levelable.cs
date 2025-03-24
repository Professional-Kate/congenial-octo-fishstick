using System;
using IdelPog.Structures.Models.Levelable;

namespace IdelPog.Structures
{
    /// <inheritdoc cref="ILevelable"/>
    public class Levelable : ILevelable
    {
        private readonly ILevelRewards _levelRewards;
        public event Action<byte> OnLevelUp;
        
        public byte Level { get; private set; }
        public int Experience { get; private set; }
        public int NextLevelExperience { get; private set; }
        public int ExperiencePerAction { get; private set; }


        public Levelable(ILevelRewards levelRewards, byte level, int experience, int nextLevelExperience, int experiencePerAction)
        {
            _levelRewards = levelRewards;
            Level = level;
            Experience = experience;
            NextLevelExperience = nextLevelExperience;
            ExperiencePerAction = experiencePerAction;
        }

        public void LevelUp()
        {
            Level++;
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
using IdelPog.Common.Structures;

namespace IdelPog.SimulationEngine.Models
{
    public sealed class Levelable : ICloneable<Levelable>
    {
        public byte Level { get; set; }
        public uint Experience { get; set; }
        public uint NextLevelExperience { get; set; }
        public uint ExperiencePerAction { get; set; }
        
        public Levelable(byte level, uint experience, uint nextLevelExperience, uint experiencePerAction)
        {
            Level = level;
            Experience = experience;
            NextLevelExperience = nextLevelExperience;
            ExperiencePerAction = experiencePerAction;
        }

        public Levelable DeepClone()
        {
            return new Levelable(Level, Experience, NextLevelExperience, ExperiencePerAction);
        }
    }
}
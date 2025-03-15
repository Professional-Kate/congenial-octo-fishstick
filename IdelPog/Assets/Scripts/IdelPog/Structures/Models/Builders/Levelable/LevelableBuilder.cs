using IdelPog.Structures.Models.Levelable;

namespace IdelPog.Structures.Builders
{
    /// <inheritdoc cref="ILevelableBuilder"/>
    public sealed class LevelableBuilder : ILevelableBuilder
    {
        private byte _level { get; set; }
        private int _experience { get; set; } 
        private int _nextLevelExperience { get; set; }
        private int _experiencePerAction { get; set; }

        public static ILevelableBuilder Builder() => new LevelableBuilder();

        public ILevelableBuilder Level(byte level)
        {
            // TODO assert level is good number
            _level = level;   
            
            return this;
        }

        public ILevelableBuilder Experience(int experience)
        {
            // TODO assert level is good number
            _experience = experience;
            
            return this;
        }

        public ILevelableBuilder NextLevelExperience(int nextLevelExperience)
        {
            // TODO assert level is good number
            _nextLevelExperience = nextLevelExperience;
            
            return this;
        }

        public ILevelableBuilder ExperiencePerAction(int experiencePerAction)
        {
            // TODO assert level is good number
            _experiencePerAction = experiencePerAction;
            
            return this;
        }

        public ILevelable Build()
        {
            // TODO: ensure each of these have a value, or set to default
            ILevelable levelable = new Levelable(_level, _experience, _nextLevelExperience, _experiencePerAction);
            
            return levelable;
        }
    }
}
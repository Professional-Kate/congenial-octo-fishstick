using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Factory.Interface;
using IdelPog.Core.Progression;

namespace IdelPog.Combat.Factory
{
    public sealed class ArenaFactory : IArenaFactory
    {
        public Arena Create(ArenaCreation arenaCreation)
        {
            ReadOnlyLevelable readOnlyLevelable = arenaCreation.ReadOnlyLevelable;

            return new Arena
            {
                ArenaType = arenaCreation.ArenaType,
                Information = arenaCreation.Information,
                Levelable = new Levelable(readOnlyLevelable.Level, readOnlyLevelable.Experience, readOnlyLevelable.NextLevelExperience, readOnlyLevelable.ExperiencePerAction)
            };
        }
    }
}
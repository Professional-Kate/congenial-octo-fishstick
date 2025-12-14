using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Factory;
using IdelPog.Core.Contracts;
using IdelPog.Core.Progression;

namespace IdelPog.Combat.Tests.Factory
{
    [TestFixture]
    public sealed class ArenaFactoryTest
    {
        private ArenaFactory _arenaFactory;
        private ArenaCreation _arenaCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _arenaFactory = new ArenaFactory();

            _arenaCreation = new ArenaCreation
            {
                ArenaType = ArenaType.FIELD,
                Information = new Information { Name = "", Description = "" },
                ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, ExperiencePerAction = 0, Level = 0, NextLevelExperience = 0 }
            };
        }
        
        private static void AssertArena(Arena arena, ArenaCreation arenaCreation)
        {
            Assert.Multiple(() =>
            {
                Assert.That(arena.ArenaType, Is.EqualTo(arenaCreation.ArenaType));
                Assert.That(arena.Information, Is.EqualTo(arenaCreation.Information));
                
                Assert.That(arena.Levelable.Experience, Is.EqualTo(arenaCreation.ReadOnlyLevelable.Experience));
                Assert.That(arena.Levelable.ExperiencePerAction, Is.EqualTo(arenaCreation.ReadOnlyLevelable.ExperiencePerAction));
                Assert.That(arena.Levelable.Level, Is.EqualTo(arenaCreation.ReadOnlyLevelable.Level));
                Assert.That(arena.Levelable.NextLevelExperience, Is.EqualTo(arenaCreation.ReadOnlyLevelable.NextLevelExperience));
            });
        }

        [Test]
        public void Positive_Create_CreatesArenaFromCreation()
        { 
            Arena arena = _arenaFactory.Create(_arenaCreation);
            
            AssertArena(arena, _arenaCreation);
        }
    }
}
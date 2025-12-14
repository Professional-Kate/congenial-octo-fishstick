using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Factory;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Tests.Factory
{
    [TestFixture]
    public sealed class CombatantDefinitionFactoryTest
    {
        private CombatantDefinitionFactory _combatantDefinitionFactory;
        
        private CombatantDefinitionCreation _combatantDefinitionCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        { 
            _combatantDefinitionFactory = new CombatantDefinitionFactory();

            _combatantDefinitionCreation = new CombatantDefinitionCreation
            {
                CombatantType = CombatantType.SLIME,
                CombatantStats = new CombatantStats { Attack = 1, Health = 1, Speed = 1 },
                Information = new Information { Name = "", Description = "" }
            };
        }

        private static void AssertDefinition(CombatantDefinition definitionCreation, CombatantDefinitionCreation combatantDefinitionCreation)
        {
            Assert.Multiple(() =>
            {
                Assert.That(definitionCreation.CombatantType, Is.EqualTo(combatantDefinitionCreation.CombatantType));
                Assert.That(definitionCreation.CombatantStats, Is.EqualTo(combatantDefinitionCreation.CombatantStats));
                Assert.That(definitionCreation.Information, Is.EqualTo(combatantDefinitionCreation.Information));
            });
        }

        [Test]
        public void Positive_Create_CreatesDefinition()
        {
            CombatantDefinition definitionCreation = _combatantDefinitionFactory.Create(_combatantDefinitionCreation);
            
            AssertDefinition(definitionCreation, _combatantDefinitionCreation);
        }
    }
}
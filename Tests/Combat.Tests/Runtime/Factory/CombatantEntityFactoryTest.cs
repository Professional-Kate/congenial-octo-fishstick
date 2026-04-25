using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Factory;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion;

namespace IdelPog.Combat.Tests.Runtime.Factory
{
    [TestFixture]
    public sealed class CombatantEntityFactoryTest
    {
        private CombatantEntityFactory _combatService;

        private CombatantCreation _wolfCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatService = new CombatantEntityFactory(new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion()));

            _wolfCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.WOLF, new StatCard { Health = 3, Attack = 5, Speed = 5 });
        }
        
        [Test]
        public void Positive_CreateEntity_CreatesEntityWithID()
        {
            const byte combatantID = 1;
            
            CombatantEntity combatantEntity = _combatService.CreateEntity(_wolfCreation, combatantID);
            
            Assert.Multiple(() =>
            {
                Assert.That(combatantEntity.CombatantID, Is.EqualTo(combatantID));
                Assert.That(combatantEntity.CombatantInformation, Is.EqualTo(_wolfCreation.Information));
            });
        }
    }
}
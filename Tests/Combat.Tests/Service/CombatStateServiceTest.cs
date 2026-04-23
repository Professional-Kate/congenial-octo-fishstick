using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Service;
using Moq;

namespace IdelPog.Combat.Tests.Service
{
    [TestFixture]
    public sealed class CombatStateServiceTest
    {
        private CombatStateService _combatStateService;
        private Mock<ICombatantFilters> _combatantFiltersMock;

        private CombatantEntity _friendlyEntity;
        private CombatantEntity _enemyEntity;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantFiltersMock = new Mock<ICombatantFilters>();
            
            _combatStateService = new CombatStateService(_combatantFiltersMock.Object);
            
            StatCard entityStats = new() { Health = 10, Attack = 10,  Speed = 3 };
            CombatantCard entityCard = CombatantCardFactory.CreateCombatantCard(CombatantType.BEAR, entityStats);
            
            _friendlyEntity = CombatantEntityFactory.CreateCombatantEntity(1, true, entityCard);
            _enemyEntity = CombatantEntityFactory.CreateCombatantEntity(2, false, entityCard with { CombatantType = CombatantType.GOBLIN });
        }

        [SetUp]
        public void Setup()
        {
            _combatantFiltersMock.Reset();
        }

        private void SetupFriendlyCombatantFilter(params CombatantEntity[] combatants)
        {
            _combatantFiltersMock.Setup(library => library.GetFriendlies()).Returns(combatants).Verifiable();
        }
        
        private void SetupEnemyCombatantFilter(params CombatantEntity[] combatants)
        {
            _combatantFiltersMock.Setup(library => library.GetEnemies()).Returns(combatants).Verifiable();
        }

        private void VerifyIsCombatOver(bool expected)
        {
            Assert.That(_combatStateService.IsCombatOver, Is.EqualTo(expected));
        }

        private void VerifyFriendlyVictory(bool expected)
        {
            Assert.That(_combatStateService.FriendlyVictory, Is.EqualTo(expected));
        }

        private void VerifyCombatantFilter()
        {
            _combatantFiltersMock.Verify();
            _combatantFiltersMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_Evaluate_CombatantsExist_CombatNotEnded()
        {
            SetupFriendlyCombatantFilter(_friendlyEntity);
            SetupEnemyCombatantFilter(_enemyEntity);
            
            Assert.DoesNotThrow(() => _combatStateService.Evaluate(_friendlyEntity));

            VerifyIsCombatOver(false);
            VerifyCombatantFilter();
        }

        [Test]
        public void Positive_Evaluate_NoEnemies_CombatEnded()
        {
            SetupEnemyCombatantFilter();
            
            Assert.DoesNotThrow(() => _combatStateService.Evaluate(_enemyEntity));

            VerifyIsCombatOver(true);
            VerifyFriendlyVictory(true);
            VerifyCombatantFilter();
        }
        
        [Test]
        public void Positive_Evaluate_NoFriendly_CombatEnded()
        {
            SetupFriendlyCombatantFilter();
            SetupEnemyCombatantFilter(_enemyEntity);
            
            Assert.DoesNotThrow(() => _combatStateService.Evaluate(_friendlyEntity));

            VerifyIsCombatOver(true);
            VerifyFriendlyVictory(false);
            VerifyCombatantFilter();
        }
    }
}
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Core.Service;
using Moq;

namespace IdelPog.Combat.Tests.Service
{
    [TestFixture]
    public sealed class CombatStateServiceTest
    {
        private CombatStateService _combatStateService;
        private Mock<ICombatantFilters> _combatantFiltersMock;


        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantFiltersMock = new Mock<ICombatantFilters>();
            
            _combatStateService = new CombatStateService(_combatantFiltersMock.Object);
        }

        [SetUp]
        public void Setup()
        {
            _combatantFiltersMock.Reset();
        }

        private void SetupHasValidCombatants(TargetingType targetingType, bool hasValidCombatants)
        {
            _combatantFiltersMock.Setup(library => library.HasValidCombatants(targetingType)).Returns(hasValidCombatants).Verifiable();
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
            SetupHasValidCombatants(TargetingType.FRIENDLY, true);
            SetupHasValidCombatants(TargetingType.ENEMY, true);
            
            Assert.DoesNotThrow(() => _combatStateService.Evaluate());

            VerifyIsCombatOver(false);
            VerifyCombatantFilter();
        }

        [Test]
        public void Positive_Evaluate_NoEnemies_CombatEnded()
        {
            SetupHasValidCombatants(TargetingType.ENEMY, false);
            
            Assert.DoesNotThrow(() => _combatStateService.Evaluate());

            VerifyIsCombatOver(true);
            VerifyFriendlyVictory(true);
            VerifyCombatantFilter();
        }
        
        [Test]
        public void Positive_Evaluate_NoFriendly_CombatEnded()
        {
            SetupHasValidCombatants(TargetingType.FRIENDLY, false);
            SetupHasValidCombatants(TargetingType.ENEMY, true);
            
            Assert.DoesNotThrow(() => _combatStateService.Evaluate());

            VerifyIsCombatOver(true);
            VerifyFriendlyVictory(false);
            VerifyCombatantFilter();
        }
    }
}
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion;

namespace IdelPog.Combat.Tests.Runtime
{
    [TestFixture]
    public sealed class CombatantEntityTest
    {
        private CombatantEntity _combatantEntity;
        private RepositoryAsserter _repositoryAsserter;
        private StatCard _statCard;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryAsserter = new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion());
            
            _statCard = new StatCard { Health = 10, Attack = 5, Speed = 4 };
        }

        [SetUp]
        public void Setup()
        { 
            _combatantEntity = new CombatantEntity(_repositoryAsserter, _statCard, 0);
        }

        private void UpdateCombatantStats(StatCard statCard)
        { 
            _combatantEntity.UpdateCombatantStats(statCard);
        }

        private CombatantStatsComponent GetComponent()
        { 
            return _combatantEntity.GetComponent<CombatantStatsComponent>();
        }

        private static void VerifyComponent(StatCard expectedStatCard, CombatantStatsComponent component)
        {
            Assert.That(component.StatCard, Is.EqualTo(expectedStatCard));
        }

        [Test]
        public void Positive_UpdateCombatantStats_UpdatesStats()
        { 
            UpdateCombatantStats(_statCard with { Health = 5 });
            
            CombatantStatsComponent component = GetComponent();

            VerifyComponent(_statCard with { Health = 5 }, component);
        }

        [Test]
        public void Positive_UpdateCombatantStats_MultipleTimes_UpdatesStats()
        {
            UpdateCombatantStats(_statCard with { Health = 5 });
            VerifyComponent(_statCard with { Health = 5 }, GetComponent());
            
            UpdateCombatantStats(_statCard with { Health = 22, Attack = 15 });
            VerifyComponent(_statCard with { Health = 22, Attack = 15 }, GetComponent());
        }
    }
}
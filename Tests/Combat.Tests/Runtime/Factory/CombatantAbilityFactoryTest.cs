using IdelPog.Combat.Factory;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Tests.TestFactory;

namespace IdelPog.Combat.Tests.Runtime.Factory
{
    [TestFixture]
    public sealed class CombatantAbilityFactoryTest
    {
        private CombatantAbilityFactory _combatantAbilityFactory;

        private CombatantAbilityEntity _combatantAbilityEntity;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantAbilityFactory = new CombatantAbilityFactory();
        }

        [SetUp]
        public void Setup()
        {
            _combatantAbilityEntity = TestCombatantAbilityEntityFactory.Create(1, 1);
        }

        private static void AssertCombatantAbility(byte combatantAbilityID, CombatantAbilityEntity sourceEntity)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(combatantAbilityID, Is.EqualTo(sourceEntity.AbilityID));
            }
        }
        
        [Test]
        public void Positive_CreateCombatantAbilities_ConvertsAll()
        {
            byte[] combatantAbilityIDs = _combatantAbilityFactory.GetCombatantAbilityIDs([_combatantAbilityEntity, _combatantAbilityEntity]);

            foreach (byte combatantAbilityID in combatantAbilityIDs)
            {
                AssertCombatantAbility(combatantAbilityID, _combatantAbilityEntity);
            }
        }
        
        [Test]
        public void Positive_CreateCombatantAbilities_EmptyInput_ReturnsNothing()
        {
            byte[] combatantAbilityIDs = _combatantAbilityFactory.GetCombatantAbilityIDs([]);
            
            Assert.That(combatantAbilityIDs, Is.Empty);
        }
    }
}
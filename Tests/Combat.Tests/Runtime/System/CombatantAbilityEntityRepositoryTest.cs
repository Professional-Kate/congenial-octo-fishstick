using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Repository;
using IdelPog.Combat.Tests.TestFactory;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class CombatantAbilityEntityRepositoryTest
    {
        private CombatantAbilityEntityRepository _combatantAbilityEntityRepository;

        private CombatantAbilityEntity _combatantAbilityEntity;
        
        [SetUp]
        public void Setup()
        { 
            _combatantAbilityEntityRepository = new CombatantAbilityEntityRepository(new CollectionAssertion(), new FoundAssertion());
            _combatantAbilityEntity = TestCombatantAbilityEntityFactory.Create(1, 1);
        }

        private void RepositoryAddAbilities(byte combatantID, params CombatantAbilityEntity[] combatantAbilities)
        { 
            Assert.DoesNotThrow(() => _combatantAbilityEntityRepository.AddAbilities(combatantID, combatantAbilities));
        }

        [Test]
        public void Positive_AddAbilities_AddsNewEntity()
        { 
            RepositoryAddAbilities(_combatantAbilityEntity.CombatantID, _combatantAbilityEntity);
        }

        [Test]
        public void Positive_AddAbilities_SameCombatantID_AddsMoreAbilities()
        {
            CombatantAbilityEntity otherEntity = TestCombatantAbilityEntityFactory.Create(1, 2);
            
            RepositoryAddAbilities(_combatantAbilityEntity.CombatantID, _combatantAbilityEntity);
            RepositoryAddAbilities(otherEntity.CombatantID, otherEntity);
            
            IReadOnlyList<CombatantAbilityEntity> combatantAbilityEntities = _combatantAbilityEntityRepository.GetAll(_combatantAbilityEntity.CombatantID);
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(combatantAbilityEntities, Has.Count.EqualTo(2));
                Assert.That(combatantAbilityEntities[0], Is.EqualTo(_combatantAbilityEntity));
                Assert.That(combatantAbilityEntities[1], Is.EqualTo(otherEntity));
            }
        }

        [Test]
        public void Positive_GetAbilityEntity_ReturnsMatchingEntity()
        {
            RepositoryAddAbilities(_combatantAbilityEntity.CombatantID, _combatantAbilityEntity);
            
            CombatantAbilityEntity combatantAbilityEntity = _combatantAbilityEntityRepository.Get(_combatantAbilityEntity.CombatantID, _combatantAbilityEntity.AbilityID);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(combatantAbilityEntity.AbilityID, Is.EqualTo(_combatantAbilityEntity.AbilityID));
                Assert.That(combatantAbilityEntity.CombatantID, Is.EqualTo(_combatantAbilityEntity.CombatantID));
            }
        }
        
        [Test]
        public void Negative_Add_BadAbilitiesCollection_Throws()
        { 
            Assert.Throws<EmptyCollectionException>(() => _combatantAbilityEntityRepository.AddAbilities(_combatantAbilityEntity.CombatantID, []));
            Assert.Throws<ArgumentNullException>(() => _combatantAbilityEntityRepository.AddAbilities(_combatantAbilityEntity.CombatantID, null!));
        }

        [Test]
        public void Negative_GetAbilityEntity_EntityNotFound_Throws()
        {
            Assert.Throws<NotFoundException<byte>>(() => _combatantAbilityEntityRepository.Get(_combatantAbilityEntity.CombatantID, _combatantAbilityEntity.AbilityID));
        }

        [Test]
        public void Negative_GetAbilityEntities_EntityFound_NoMatchingAbility_Throws()
        {
            RepositoryAddAbilities(_combatantAbilityEntity.CombatantID, _combatantAbilityEntity);
            
            Assert.Throws<KeyNotFoundException>(() => _combatantAbilityEntityRepository.Get(_combatantAbilityEntity.CombatantID, 23));
        }
    }
}
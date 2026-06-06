using IdelPog.Combat.Contracts.Enum;
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
            _combatantAbilityEntity = TestCombatantAbilityEntityFactory.Create(1, AbilityType.SLASH);
        }

        private void RepositoryAdd(byte combatantID, params CombatantAbilityEntity[] combatantAbilities)
        { 
            Assert.DoesNotThrow(() => _combatantAbilityEntityRepository.Add(combatantID, combatantAbilities));
        }

        [Test]
        public void Positive_Add_AddsNewEntity()
        { 
            RepositoryAdd(_combatantAbilityEntity.CombatantID, _combatantAbilityEntity);
        }

        [Test]
        public void Positive_GetAbilityEntity_ReturnsMatchingEntity()
        {
            RepositoryAdd(_combatantAbilityEntity.CombatantID, _combatantAbilityEntity);
            
            CombatantAbilityEntity combatantAbilityEntity = _combatantAbilityEntityRepository.Get(_combatantAbilityEntity.CombatantID, _combatantAbilityEntity.AbilityType);
            
            Assert.Multiple(() =>
            {
                Assert.That(combatantAbilityEntity.AbilityType, Is.EqualTo(_combatantAbilityEntity.AbilityType));
                Assert.That(combatantAbilityEntity.CombatantID, Is.EqualTo(_combatantAbilityEntity.CombatantID));
            });
        }

        [Test]
        public void Negative_Add_BadAbilitiesCollection_Throws()
        { 
            Assert.Throws<EmptyCollectionException>(() => _combatantAbilityEntityRepository.Add(_combatantAbilityEntity.CombatantID, []));
            Assert.Throws<ArgumentNullException>(() => _combatantAbilityEntityRepository.Add(_combatantAbilityEntity.CombatantID, null!));
        }

        [Test]
        public void Negative_GetAbilityEntity_EntityNotFound_Throws()
        {
            Assert.Throws<NotFoundException<byte>>(() => _combatantAbilityEntityRepository.Get(_combatantAbilityEntity.CombatantID, _combatantAbilityEntity.AbilityType));
        }

        [Test]
        public void Negative_GetAbilityEntities_EntityFound_NoMatchingAbility_Throws()
        {
            RepositoryAdd(_combatantAbilityEntity.CombatantID, _combatantAbilityEntity);
            
            Assert.Throws<KeyNotFoundException>(() => _combatantAbilityEntityRepository.Get(_combatantAbilityEntity.CombatantID, (AbilityType) 1));
        }
    }
}
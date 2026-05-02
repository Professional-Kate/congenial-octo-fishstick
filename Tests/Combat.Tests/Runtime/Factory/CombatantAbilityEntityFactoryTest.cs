using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Factory;
using IdelPog.Core.Contracts;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.Factory
{
    [TestFixture]
    public sealed class CombatantAbilityEntityFactoryTest
    {
        private CombatantAbilityEntityFactory _combatantAbilityEntityFactory;
        private Mock<IAssetRepository<AbilityType, AbilityEntity>> _repositoryMock;

        private CombatantAbilityEquip _combatantAbilityEquip;
        private AbilityEntity _abilityEntity;
        private AbilityCard _abilityCard;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<IAssetRepository<AbilityType, AbilityEntity>>();
            
            _combatantAbilityEntityFactory = new CombatantAbilityEntityFactory(_repositoryMock.Object);

            _abilityCard = new AbilityCard { AbilityType = AbilityType.BASIC_ATTACK, StrategyCard = new StrategyCard { TargetingType = TargetingType.HIGH_ATTACK }};
            _combatantAbilityEquip = new CombatantAbilityEquip { CombatantID = 1, AbilityCards = [_abilityCard] };
            _abilityEntity = new AbilityEntity(new CooldownComponent { Cooldown = 10 }, new DamageComponent { PhysicalDamage = 20, LightningDamage = 0, ColdDamage = 0, FireDamage = 0 })
            {
                AbilityType = AbilityType.BASIC_ATTACK, 
                Information = new Information { Name = "", Description = "" },
                AbilitySlots = 1
            };
        }

        [SetUp]
        public void SetUp()
        {
            _repositoryMock.Reset();
        }

        private void VerifyRepository()
        {
            _repositoryMock.Verify();
            _repositoryMock.VerifyNoOtherCalls();
        }

        private void SetupRepositoryGet(AbilityEntity abilityEntity)
        {
            _repositoryMock.Setup(library => library.Get(abilityEntity.AbilityType)).Returns(abilityEntity).Verifiable();
        }

        private static void AssertCollectionCount(int count, IReadOnlyList<CombatantAbilityEntity> combatantAbilityEntities)
        {
            Assert.That(combatantAbilityEntities, Has.Count.EqualTo(count));
        }

        private static void AssertCombatantAbility(CombatantAbilityEntity combatantAbilityEntity, AbilityEntity abilityEntity, byte combatantID, TargetingType targetingType)
        {
            Assert.Multiple(() =>
            {
                Assert.That(combatantAbilityEntity.AbilityType, Is.EqualTo(abilityEntity.AbilityType));
                Assert.That(combatantAbilityEntity.CombatantID, Is.EqualTo(combatantID));
                Assert.That(combatantAbilityEntity.GetComponent<DamageComponent>(), Is.EqualTo(abilityEntity.GetComponent<DamageComponent>()));
                Assert.That(combatantAbilityEntity.GetComponent<CooldownComponent>(), Is.EqualTo(abilityEntity.GetComponent<CooldownComponent>()));
                Assert.That(combatantAbilityEntity.GetComponent<TargetingTypeComponent>().TargetingType, Is.EqualTo(targetingType));
            });
        }

        [Test]
        public void Positive_Create_CreatesNewEntity_AddsExpectedComponents()
        {
            SetupRepositoryGet(_abilityEntity);
            
            IReadOnlyList<CombatantAbilityEntity> combatantAbilityEntities = _combatantAbilityEntityFactory.Create(_combatantAbilityEquip);
            
            AssertCollectionCount(1,  combatantAbilityEntities);
            AssertCombatantAbility(combatantAbilityEntities[0], _abilityEntity, _combatantAbilityEquip.CombatantID, _abilityCard.StrategyCard.TargetingType);
            VerifyRepository();
        }
        
        [Test]
        public void Positive_Create_DuplicateEquip_ReturnsTwoEntities()
        {
            CombatantAbilityEquip doubleEquip = _combatantAbilityEquip with { AbilityCards = [_abilityCard, _abilityCard] };
            
            SetupRepositoryGet(_abilityEntity);
            
            IReadOnlyList<CombatantAbilityEntity> combatantAbilityEntities = _combatantAbilityEntityFactory.Create(doubleEquip);
            
            AssertCollectionCount(2,  combatantAbilityEntities);
            AssertCombatantAbility(combatantAbilityEntities[0], _abilityEntity, doubleEquip.CombatantID, _abilityCard.StrategyCard.TargetingType);
            AssertCombatantAbility(combatantAbilityEntities[1], _abilityEntity, doubleEquip.CombatantID, _abilityCard.StrategyCard.TargetingType);
            VerifyRepository();
        }
        
        [Test]
        public void Positive_Create_NoAbilityCards_ReturnsEmptyCollection()
        {
            CombatantAbilityEquip noCards = _combatantAbilityEquip with { AbilityCards = [] };
            
            IReadOnlyList<CombatantAbilityEntity> combatantAbilityEntities = _combatantAbilityEntityFactory.Create(noCards);
            
            AssertCollectionCount(0,  combatantAbilityEntities);
            VerifyRepository();
        }

        [Test]
        public void Negative_Create_AbilityNotFound_Throws()
        {
            _repositoryMock.Setup(library => library.Get(_abilityCard.AbilityType))
                .Throws(new NotFoundException<AbilityType>(_abilityEntity.AbilityType)).Verifiable();
            
            Assert.Throws<NotFoundException<AbilityType>>(() => _combatantAbilityEntityFactory.Create(_combatantAbilityEquip));
            
            VerifyRepository();
        }
    }
}
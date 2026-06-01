using IdelPog.Combat.Contracts.Card;
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
        private CombatantAbilityCard _combatantAbilityCard;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<IAssetRepository<AbilityType, AbilityEntity>>();
            
            _combatantAbilityEntityFactory = new CombatantAbilityEntityFactory(_repositoryMock.Object);

            _combatantAbilityCard = new CombatantAbilityCard { AbilityType = AbilityType.BASIC_ATTACK, StrategyCard = new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH }};
            _combatantAbilityEquip = new CombatantAbilityEquip { CombatantID = 1, AbilityCards = [_combatantAbilityCard] };
        }

        [SetUp]
        public void SetUp()
        {
            _repositoryMock.Reset();
            _abilityEntity = new AbilityEntity(new CooldownComponent { Cooldown = 10 }, new ElementalDamageComponent { LightningDamage = 0, ColdDamage = 0, FireDamage = 0 }, new PhysicalDamageComponent { SlashDamage = 20, StrikeDamage = 0, ThrustDamage = 0 })
            {
                AbilityType = AbilityType.BASIC_ATTACK, 
                Information = new Information { Name = "", Description = "" },
                AbilitySlots = 1
            };
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

        private static void AssertCombatantAbility(CombatantAbilityEntity combatantAbilityEntity, AbilityEntity abilityEntity, byte combatantID, TargetingPreference targetingPreference)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(combatantAbilityEntity.AbilityType, Is.EqualTo(abilityEntity.AbilityType));
                Assert.That(combatantAbilityEntity.CombatantID, Is.EqualTo(combatantID));
                Assert.That(combatantAbilityEntity.GetComponent<ElementalDamageComponent>(), Is.EqualTo(abilityEntity.GetComponent<ElementalDamageComponent>()));
                Assert.That(combatantAbilityEntity.GetComponent<CooldownComponent>(), Is.EqualTo(abilityEntity.GetComponent<CooldownComponent>()));
                Assert.That(combatantAbilityEntity.GetComponent<TargetingPreferenceComponent>().TargetingPreference, Is.EqualTo(targetingPreference));
            }
        }

        private static void AddCastTimeComponent(AbilityEntity abilityEntity, uint castTime)
        {
            abilityEntity.AddComponent(new CastTimeComponent { CastTime = castTime });
        }

        private static void AssertCastTimeComponent(CombatantAbilityEntity combatantAbilityEntity, AbilityEntity abilityEntity) 
        {
            if (abilityEntity.TryGetComponent(out CastTimeComponent castTimeComponent))
            {
                Assert.That(combatantAbilityEntity.GetComponent<CastTimeComponent>().CastTime, Is.EqualTo(castTimeComponent.CastTime));
                return;
            }
            
            Assert.That(combatantAbilityEntity.ContainsComponent<CastTimeComponent>(), Is.False);
        }
 
        [Test]
        public void Positive_Create_CreatesNewEntity_AddsExpectedComponents()
        {
            SetupRepositoryGet(_abilityEntity);
            
            IReadOnlyList<CombatantAbilityEntity> combatantAbilityEntities = _combatantAbilityEntityFactory.Create(_combatantAbilityEquip);
            
            AssertCollectionCount(1,  combatantAbilityEntities);
            AssertCombatantAbility(combatantAbilityEntities[0], _abilityEntity, _combatantAbilityEquip.CombatantID, _combatantAbilityCard.StrategyCard.TargetingPreference);
            AssertCastTimeComponent(combatantAbilityEntities[0], _abilityEntity);
            VerifyRepository();
        }
        
        [Test]
        public void Positive_Create_DuplicateEquip_ReturnsTwoEntities()
        {
            CombatantAbilityEquip doubleEquip = _combatantAbilityEquip with { AbilityCards = [_combatantAbilityCard, _combatantAbilityCard] };
            
            SetupRepositoryGet(_abilityEntity);
            
            IReadOnlyList<CombatantAbilityEntity> combatantAbilityEntities = _combatantAbilityEntityFactory.Create(doubleEquip);
            
            AssertCollectionCount(2,  combatantAbilityEntities);
            AssertCombatantAbility(combatantAbilityEntities[0], _abilityEntity, doubleEquip.CombatantID, _combatantAbilityCard.StrategyCard.TargetingPreference);
            AssertCombatantAbility(combatantAbilityEntities[1], _abilityEntity, doubleEquip.CombatantID, _combatantAbilityCard.StrategyCard.TargetingPreference);
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
        public void Positive_Create_CreatesEntity_WithCastTime()
        {
            AddCastTimeComponent(_abilityEntity, 100u);
            SetupRepositoryGet(_abilityEntity);
            
            IReadOnlyList<CombatantAbilityEntity> combatantAbilityEntities = _combatantAbilityEntityFactory.Create(_combatantAbilityEquip);
            
            AssertCollectionCount(1,  combatantAbilityEntities);
            AssertCombatantAbility(combatantAbilityEntities[0], _abilityEntity, _combatantAbilityEquip.CombatantID, _combatantAbilityCard.StrategyCard.TargetingPreference);
            AssertCastTimeComponent(combatantAbilityEntities[0], _abilityEntity);
            VerifyRepository();
        }

        [Test]
        public void Negative_Create_AbilityNotFound_Throws()
        {
            _repositoryMock.Setup(library => library.Get(_combatantAbilityCard.AbilityType))
                .Throws(new NotFoundException<AbilityType>(_abilityEntity.AbilityType)).Verifiable();
            
            Assert.Throws<NotFoundException<AbilityType>>(() => _combatantAbilityEntityFactory.Create(_combatantAbilityEquip));
            
            VerifyRepository();
        }
    }
}
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Combatant.Contracts;
using IdelPog.Combat.Combatant.Model;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Core.Arena;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Service.Interface;
using IdelPog.Combat.Tests.TestFactory;
using Moq;

namespace IdelPog.Combat.Tests.Core
{
    [TestFixture]
    public sealed class CombatArenaTest
    {
        private CombatArena _combatArena;
        
        private Mock<ICombatantEntityFactory> _combatantEntityFactoryMock;
        private Mock<ICombatantRepository> _combatantRepositoryMock;
        private Mock<IDictionary<byte, EquippedAbilityDefinition>> _equippedAbilityDictionaryMock;
        private Mock<IAbilityEntityFactory> _abilityEntityFactoryMock;
        private Mock<IAbilityEntityRepository> _abilityEntityRepositoryMock;
        private Mock<IInitialAbilityScheduler> _initialAbilitySchedulerMock;
        private Mock<ICombatQueueRunner> _combatQueueRunnerMock;

        private readonly EquippedAbilityDefinition _abilityDefinition = new()
        {
            CombatantID = 1,
            EquippedAbilities = [new EquippedAbility { AbilityID = 1, StrategyCards = [ new StrategyCard
            {
                CombatantStatType = CombatantStatType.ABILITY_DAMAGE,
                TargetingPreference = TargetingPreference.HIGHEST, 
                TargetingType = TargetingType.ENEMY,
                Priority = 0
            }]}]
        };
        
        private readonly CombatantDefinition _wolfDefinition = TestCombatantDefinitionFactory.Create(1, CombatantType.WOLF);
        private readonly CombatantEntity _wolfEntity = TestCombatantEntityFactory.Create(1, TargetingType.FRIENDLY);
        private readonly AbilityEntity _wolfAbility = TestAbilityEntityFactory.Create(1, 1);
        
        private readonly CombatantDefinition _goblinDefinition = TestCombatantDefinitionFactory.Create(2, CombatantType.GOBLIN);
        private readonly CombatantEntity _goblinEntity = TestCombatantEntityFactory.Create(2, TargetingType.ENEMY);
        private readonly AbilityEntity _goblinAbility = TestAbilityEntityFactory.Create(2, 1);

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantEntityFactoryMock = new Mock<ICombatantEntityFactory>();
            _combatantRepositoryMock = new Mock<ICombatantRepository>();
            _equippedAbilityDictionaryMock = new Mock<IDictionary<byte, EquippedAbilityDefinition>>();
            _abilityEntityFactoryMock = new Mock<IAbilityEntityFactory>();
            _abilityEntityRepositoryMock = new Mock<IAbilityEntityRepository>();
            _initialAbilitySchedulerMock = new Mock<IInitialAbilityScheduler>();
            _combatQueueRunnerMock = new Mock<ICombatQueueRunner>();
            
            _combatArena = new CombatArena(_combatantEntityFactoryMock.Object, _combatantRepositoryMock.Object, _equippedAbilityDictionaryMock.Object, _abilityEntityFactoryMock.Object, _abilityEntityRepositoryMock.Object, _initialAbilitySchedulerMock.Object, _combatQueueRunnerMock.Object);
        }

        [SetUp]
        public void Setup()
        {
            _combatantEntityFactoryMock.Reset();
            _combatantRepositoryMock.Reset();
            _equippedAbilityDictionaryMock.Reset();
            _abilityEntityFactoryMock.Reset();
            _abilityEntityRepositoryMock.Reset();
            _initialAbilitySchedulerMock.Reset();
            _combatQueueRunnerMock.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _combatantEntityFactoryMock.Verify();
            _combatantEntityFactoryMock.VerifyNoOtherCalls();
            _combatantRepositoryMock.Verify();
            _combatantRepositoryMock.VerifyNoOtherCalls();
            _equippedAbilityDictionaryMock.Verify();
            _equippedAbilityDictionaryMock.VerifyNoOtherCalls();
            _abilityEntityFactoryMock.Verify();
            _abilityEntityFactoryMock.VerifyNoOtherCalls();
            _abilityEntityRepositoryMock.Verify();
            _abilityEntityRepositoryMock.VerifyNoOtherCalls();
            _initialAbilitySchedulerMock.Verify();
            _initialAbilitySchedulerMock.VerifyNoOtherCalls();
            _combatQueueRunnerMock.Verify();
            _combatQueueRunnerMock.VerifyNoOtherCalls();
        }

        private void SetupCombatantEntityCreate(IReadOnlyList<CombatantDefinition> definitions, TargetingType targetingType, CombatantEntity[] combatantEntities)
        {
            _combatantEntityFactoryMock.Setup(library => library.Create(definitions, targetingType)).Returns(combatantEntities).Verifiable();
        }

        private void VerifyCombatantsSeeded(CombatantEntity[] friendlyCombatants, CombatantEntity[] enemyCombatants)
        {
            _combatantRepositoryMock.Verify(library => library.SeedFriendlyCombatants(friendlyCombatants), Times.Once);
            _combatantRepositoryMock.Verify(library => library.SeedEnemyCombatants(enemyCombatants), Times.Once);
        }

        private void SetupCombatantRepositoryEnumerate(CombatantEntity[] combatantEntities)
        { 
            _combatantRepositoryMock.Setup(library => library.Enumerate()).Returns(combatantEntities).Verifiable();
        }

        private void SetupEquippedAbilityGet(byte combatantID, EquippedAbilityDefinition equippedAbilityDefinition, bool contains)
        { 
            _equippedAbilityDictionaryMock.Setup(library => library.TryGetValue(combatantID, out equippedAbilityDefinition)).Returns(contains).Verifiable();
        }

        private void SetupAbilityEntityCreate(EquippedAbilityDefinition equippedAbilityDefinition, byte instanceID, AbilityEntity abilityEntity)
        {
            _abilityEntityFactoryMock.Setup(library => library.Create(equippedAbilityDefinition, instanceID)).Returns([abilityEntity]).Verifiable();
        }

        private void VerifyAbilitiesSeeded(AbilityEntity[] abilityEntities)
        {
            _abilityEntityRepositoryMock.Verify(library => library.SeedAbilities(abilityEntities), Times.Once);
        }

        private void VerifyScheduleRegisteredAbilities()
        { 
            _initialAbilitySchedulerMock.Verify(library => library.ScheduleRegisteredAbilities(0), Times.Once);
        }

        private void VerifyRunCombat()
        {
            _combatQueueRunnerMock.Verify(library => library.RunCombat(), Times.Once);
        }

        private void VerifyStateCleared()
        {
            _combatantRepositoryMock.Verify(library => library.Clear(), Times.Once);
            _abilityEntityRepositoryMock.Verify(library => library.Clear(), Times.Once);
        }

        [Test]
        public void Positive_RunCombatSimulation_SimulatesCombatFully()
        {
            SetupCombatantEntityCreate([_wolfDefinition], TargetingType.FRIENDLY, [_wolfEntity]);
            SetupCombatantEntityCreate([_goblinDefinition], TargetingType.ENEMY, [_goblinEntity]);
            SetupCombatantRepositoryEnumerate([_wolfEntity, _goblinEntity]);
            SetupEquippedAbilityGet(_wolfEntity.CombatantID, _abilityDefinition, true);
            SetupEquippedAbilityGet(_goblinDefinition.CombatantID, _abilityDefinition, true);
            SetupAbilityEntityCreate(_abilityDefinition, 1, _wolfAbility);
            SetupAbilityEntityCreate(_abilityDefinition, 2, _goblinAbility);
            
            Assert.DoesNotThrow(() => _combatArena.RunCombatSimulation([_wolfDefinition], [_goblinDefinition]));

            VerifyCombatantsSeeded([_wolfEntity], [_goblinEntity]);
            VerifyAbilitiesSeeded([_wolfAbility, _goblinAbility]);
            VerifyScheduleRegisteredAbilities();
            VerifyRunCombat();
            VerifyStateCleared();
        }

        [Test]
        public void Positive_RunCombatSimulation_CombatantWithNoAbilities_CreatesNoEntity()
        {
            SetupCombatantEntityCreate([_wolfDefinition], TargetingType.FRIENDLY, [_wolfEntity]);
            SetupCombatantEntityCreate([_goblinDefinition], TargetingType.ENEMY, [_goblinEntity]);
            SetupCombatantRepositoryEnumerate([_wolfEntity, _goblinEntity]);
            SetupEquippedAbilityGet(_wolfEntity.CombatantID, _abilityDefinition, true);
            SetupEquippedAbilityGet(_goblinDefinition.CombatantID, _abilityDefinition, false);
            SetupAbilityEntityCreate(_abilityDefinition, 1, _wolfAbility);
            
            Assert.DoesNotThrow(() => _combatArena.RunCombatSimulation([_wolfDefinition], [_goblinDefinition]));

            VerifyCombatantsSeeded([_wolfEntity], [_goblinEntity]);
            VerifyAbilitiesSeeded([_wolfAbility]);
            VerifyScheduleRegisteredAbilities();
            VerifyRunCombat();
            VerifyStateCleared();
        }
    }
}
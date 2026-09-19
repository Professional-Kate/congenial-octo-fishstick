using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Core.Logging;
using IdelPog.Combat.Core.Logging.Contracts;
using IdelPog.Combat.Core.Logging.Interface;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Tests.TestFactory;
using Moq;

namespace IdelPog.Combat.Tests.Core
{
    [TestFixture]
    public sealed class InitialStateLoggerTest
    {
        private InitialStateLogger _initialStateLogger;
        private Mock<IReadOnlyCombatantFactory> _combatantFactoryMock;
        private Mock<IReadOnlyAbilityFactory> _abilityFactoryMock;

        private readonly CombatantEntity _friendlyEntity = TestCombatantEntityFactory.Create(1, TargetingType.FRIENDLY);
        private readonly ReadOnlyCombatant _friendlyCombatant = new()
        {
            HealthCard = new HealthCard { Health = 10, BaseHealth = 10 },
            AgilityCard = new AgilityCard { Initiative = 5, Speed = 5 },
            IsAlive = true,
            TargetingType = TargetingType.FRIENDLY,
            CombatantID = 1,
            InstanceID = 4
        };
        
        private readonly CombatantEntity _enemyEntity = TestCombatantEntityFactory.Create(2, TargetingType.ENEMY);
        private readonly ReadOnlyCombatant _enemyCombatant = new()
        {
            HealthCard = new HealthCard { Health = 16, BaseHealth = 20 },
            AgilityCard = new AgilityCard { Initiative = 4, Speed = 10 },
            IsAlive = true,
            TargetingType = TargetingType.ENEMY,
            CombatantID = 2,
            InstanceID = 5
        };

        private readonly AbilityEntity _abilityEntity = TestAbilityEntityFactory.Create(4, 1);
        private readonly ReadOnlyAbility _ability = new()
            {
                AbilityCard = new AbilityCard { AbilitySlots = 1, Cooldown = 4 },
                ReadOnlyAbilityStages = 
                    [ 
                        new ReadOnlyAbilityStage
                        {
                            AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.HOLY, MaxTargets = 1, CastTime = 4, Value = 10,
                            ReadOnlyStrategy = new ReadOnlyStrategy { TargetingPreference = TargetingPreference.HIGHEST, StatType = StatType.HEALTH, TargetingType = TargetingType.ENEMY }
                        } 
                    ],
                AbilityID = 1,
                InstanceID = 4
            };
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantFactoryMock = new Mock<IReadOnlyCombatantFactory>();
            _abilityFactoryMock = new Mock<IReadOnlyAbilityFactory>();
        }

        [SetUp]
        public void Setup()
        { 
            _combatantFactoryMock.Reset();
            _abilityFactoryMock.Reset();
            
            _initialStateLogger = new InitialStateLogger(_combatantFactoryMock.Object, _abilityFactoryMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _combatantFactoryMock.Verify();
            _combatantFactoryMock.VerifyNoOtherCalls(); 
            _abilityFactoryMock.Verify();
            _abilityFactoryMock.VerifyNoOtherCalls();
        }

        private void SetupReadOnlyCombatantFactory(CombatantEntity[] combatantEntities, ReadOnlyCombatant[] readOnlyCombatants)
        {
            _combatantFactoryMock.Setup(library => library.Create(combatantEntities)).Returns(readOnlyCombatants).Verifiable();
        }

        private void SetupReadOnlyAbilityFactory(AbilityEntity[] abilityEntities, ReadOnlyAbility[] readOnlyAbilities)
        {
            _abilityFactoryMock.Setup(library => library.Create(abilityEntities)).Returns(readOnlyAbilities).Verifiable();
        }

        private static void VerifyInitialEntities(InitialEntities initialEntities, ReadOnlyAbility[] readOnlyAbilities, ReadOnlyCombatant[] friendlyCombatants, ReadOnlyCombatant[] enemyCombatants)
        {
            Assert.That(initialEntities, Is.Not.Default);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(initialEntities.Abilities, Is.EqualTo(readOnlyAbilities));
                Assert.That(initialEntities.FriendlyCombatants, Is.EqualTo(friendlyCombatants));
                Assert.That(initialEntities.EnemyCombatants, Is.EqualTo(enemyCombatants));
            }
        }

        [Test]
        public void Positive_LogInitialState_CreatesInitialState()
        {
            SetupReadOnlyCombatantFactory([_friendlyEntity], [_friendlyCombatant]);
            SetupReadOnlyCombatantFactory([_enemyEntity], [_enemyCombatant]);
            SetupReadOnlyAbilityFactory([_abilityEntity], [_ability]);
            
            InitialEntities initialEntities = _initialStateLogger.LogInitialState([_friendlyEntity], [_enemyEntity], [_abilityEntity]);

            VerifyInitialEntities(initialEntities, [_ability], [_friendlyCombatant], [_enemyCombatant]);
        }

        [Test]
        public void Positive_LogInitialState_MultipleTimes_ReplacesState()
        {
            SetupReadOnlyCombatantFactory([_friendlyEntity], [_friendlyCombatant]);
            SetupReadOnlyCombatantFactory([_enemyEntity], [_enemyCombatant]);
            SetupReadOnlyAbilityFactory([_abilityEntity], [_ability]);
            
            _initialStateLogger.LogInitialState([_friendlyEntity], [_enemyEntity], [_abilityEntity]);
            InitialEntities initialEntities = _initialStateLogger.LogInitialState([_enemyEntity], [_friendlyEntity], [_abilityEntity]);

            VerifyInitialEntities(initialEntities, [_ability], [_enemyCombatant], [_friendlyCombatant]);
        }

        [Test]
        public void Positive_GetInitialEntities_NoLoggedEntities_ReturnsDefault()
        {
            SetupReadOnlyCombatantFactory([], []);
            SetupReadOnlyAbilityFactory([], []);
            
            InitialEntities initialEntities = _initialStateLogger.LogInitialState([], [], []);
            VerifyInitialEntities(initialEntities, [], [], []);
        }
    }
}
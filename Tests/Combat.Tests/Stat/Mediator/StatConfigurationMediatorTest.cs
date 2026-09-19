using System.Collections.Immutable;
using IdelPog.Combat.Ability.Contracts;
using IdelPog.Combat.Combatant.Contracts;
using IdelPog.Combat.Stat.Contracts;
using IdelPog.Combat.Stat.Contracts.Command;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Contracts.Response;
using IdelPog.Combat.Stat.Mediator;
using IdelPog.Combat.Stat.Service.Interface;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Stat.Mediator
{
    [TestFixture]
    public sealed class StatConfigurationMediatorTest
    {
        private StatConfigurationMediator _statConfigurationMediator;
        private Mock<IStatConfigurationSetter> _statConfigurationSetterMock;
        private Mock<IDispatchMany<StatConfigurationResponse>> _responseDispatcherMock;

        private readonly StatConfiguration _statConfiguration = new()
        {
            CombatantStatLinks = new CombatantStatLinks
            {
                BaseHealthID = 0,
                HealthID = 1,
                SpeedID = 2,
                InitiativeID = 3
            },
            AbilityStatLinks = new AbilityStatLinks
            {
                AbilityDamageID = 4,
                AbilityHealingID = 5,
                RetaliationDamageID = 6,
                CastTimeID = 7,
                CooldownID = 8,
                AbilitySlotsID = 9
            }
        };

        private readonly ImmutableArray<StatBinding> _statBindings =
        [
            new() { StatType = StatType.BASE_HEALTH, LinkedID = 0 },
            new() { StatType = StatType.INITIATIVE, LinkedID = 3 },
            new() { StatType = StatType.ABILITY_DAMAGE, LinkedID = 4 },
            new() { StatType = StatType.ABILITY_SLOTS, LinkedID = 9 }
        ];

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _statConfigurationSetterMock = new Mock<IStatConfigurationSetter>();
            _responseDispatcherMock = new Mock<IDispatchMany<StatConfigurationResponse>>();
            
            _statConfigurationMediator = new StatConfigurationMediator(_statConfigurationSetterMock.Object, _responseDispatcherMock.Object, new CollectionAssertion());
        }

        [SetUp]
        public void Setup()
        {
            _statConfigurationSetterMock.Reset();
            _responseDispatcherMock.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _statConfigurationSetterMock.Verify();
            _statConfigurationSetterMock.VerifyNoOtherCalls();
            _responseDispatcherMock.Verify();
            _responseDispatcherMock.VerifyNoOtherCalls();
        }

        private void SetupGetStatBindings(ImmutableArray<StatBinding> statBindings)
        {
            _statConfigurationSetterMock.Setup(library => library.GetStatBindings()).Returns(statBindings).Verifiable();
        }

        private void VerifySetStatConfiguration(StatConfiguration statConfiguration)
        { 
            _statConfigurationSetterMock.Verify(library => library.SetStatConfiguration(statConfiguration), Times.Once);
        }

        private void VerifyDispatch(ImmutableArray<StatBinding> statBindings, params StatConfiguration[] commands)
        {
            StatConfigurationResponse[] statConfigurationResponses = new StatConfigurationResponse[commands.Length];
            for (int i = 0; i < commands.Length; i++)
            {
                StatConfiguration statConfiguration = commands[i];
                statConfigurationResponses[i] = new StatConfigurationResponse {  StatConfiguration = statConfiguration, StatBindings =  statBindings };
            }
            
            _responseDispatcherMock.Verify(library => library.Dispatch(statConfigurationResponses), Times.Once);
        }

        [Test]
        public void Positive_HandleMessages_AddsStatBindings_DispatchesResponse()
        { 
            SetupGetStatBindings(_statBindings);
            
            Assert.DoesNotThrow(() => _statConfigurationMediator.HandleMessages([_statConfiguration]));
            
            VerifySetStatConfiguration(_statConfiguration);
            VerifyDispatch(_statBindings, _statConfiguration);
        }

        [Test]
        public void Positive_HandleMessages_MultipleCommands_DispatchesResponses()
        {
            SetupGetStatBindings(_statBindings);
            
            Assert.DoesNotThrow(() => _statConfigurationMediator.HandleMessages([_statConfiguration, _statConfiguration]));
            
            _statConfigurationSetterMock.Verify(library => library.SetStatConfiguration(_statConfiguration), Times.Exactly(2));
            VerifyDispatch(_statBindings, _statConfiguration, _statConfiguration);
        }

        [Test]
        public void Negative_HandleMessages_BadCollection_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _statConfigurationMediator.HandleMessages([]));
            Assert.Throws<ArgumentNullException>(() => _statConfigurationMediator.HandleMessages(null!));
        }
    }
}
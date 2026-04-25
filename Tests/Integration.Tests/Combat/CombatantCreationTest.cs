using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Error;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Exceptions;
using IdelPog.Core.Contracts;

namespace IdelPog.Integration.Tests.Combat
{
    [TestFixture]
    public sealed class CombatantCreationTest : ManagedTestBuffer
    {
        private ManagedResponseListener<CombatantCreationResponse> _responseListener;
        private ManagedErrorListener<CombatantCreationError> _errorListener;
        
        private CombatantCreation _humanCombatantCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _humanCombatantCreation = new CombatantCreation
            {
                CombatantType = CombatantType.HUMAN,
                Information = new Information { Name = "Human", Description = "Man" },
                StatCard = new StatCard { Attack = 10, Speed = 5, Health = 20 }
            };
        }

        [SetUp]
        public void Setup()
        {
            _responseListener = new ManagedResponseListener<CombatantCreationResponse>();
            _errorListener = new ManagedErrorListener<CombatantCreationError>();
            
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
        }

        private static void AssertResponse(CombatantCreationResponse response, CombatantCreation source)
        {
            Assert.Multiple(() =>
            {
                Assert.That(response.CombatantType, Is.EqualTo(source.CombatantType));
                Assert.That(response.Information, Is.EqualTo(source.Information));
                Assert.That(response.StatCard, Is.EqualTo(source.StatCard));
            });
        }

        private void AssertErrorLength(int length)
        { 
            Assert.That(_errorListener.Error.CombatantCreations, Has.Length.EqualTo(length));
        }

        private void AssertErrorCollection(params CombatantCreation[] combatantCreations)
        {
            Assert.That(_errorListener.Error.CombatantCreations, Is.EqualTo(combatantCreations));
        }

        [Test]
        public void Positive_DispatchMessage_CreatesCombatant()
        {
            Assert.DoesNotThrow(() => DispatchMessage(_humanCombatantCreation));
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], _humanCombatantCreation);
            Assert.That(_responseListener.Responses[0].CombatantID, Is.EqualTo(0));
        }

        [Test]
        public void Positive_DispatchMessages_CreatesMultipleCombatants()
        {
            Assert.DoesNotThrow(() => DispatchMessage(_humanCombatantCreation with { CombatantType = CombatantType.GOBLIN }, _humanCombatantCreation));
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(2);
            AssertResponse(_responseListener.Responses[0], _humanCombatantCreation with { CombatantType = CombatantType.GOBLIN });
            AssertResponse(_responseListener.Responses[1], _humanCombatantCreation);
            Assert.That(_responseListener.Responses[0].CombatantID, Is.Not.EqualTo(_responseListener.Responses[1].CombatantID));
        }

        [Test]
        public void Positive_DispatchMessages_AcceptsDuplicateCreations()
        {
            Assert.DoesNotThrow(() => DispatchMessage(_humanCombatantCreation, _humanCombatantCreation));
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(2);
            AssertResponse(_responseListener.Responses[0], _humanCombatantCreation);
            AssertResponse(_responseListener.Responses[1], _humanCombatantCreation);
            Assert.That(_responseListener.Responses[0].CombatantID, Is.Not.EqualTo(_responseListener.Responses[1].CombatantID));
        }

        [Test]
        public void Positive_DispatchMessage_CombatantHasZeroAttack()
        {
            StatCard zeroAttackStatCard = new() { Speed = 52, Attack = 0, Health = 11 };
            CombatantCreation zeroAttackCombatant = _humanCombatantCreation with { StatCard = zeroAttackStatCard };
            
            Assert.DoesNotThrow(() => DispatchMessage(zeroAttackCombatant));
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], zeroAttackCombatant);
        }

        [Test]
        public void Negative_DispatchMessage_CombatantHasZeroSpeed_DispatchesError()
        {
            StatCard zeroSpeedStatCard = new() { Speed = 0, Attack = 10, Health = 11 };
            CombatantCreation zeroSpeedCombatant = _humanCombatantCreation with { StatCard = zeroSpeedStatCard };
            
            Assert.DoesNotThrow(() => DispatchMessage(zeroSpeedCombatant));
            
            _responseListener.AssertWasCalled(false);
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertBaseError<NumberZeroException>(_errorListener.Error.BaseError);
            AssertErrorCollection(zeroSpeedCombatant);
        }
        
        [Test]
        public void Negative_DispatchMessage_CombatantHasZeroHealth_DispatchesError()
        {
            StatCard zeroHealthStatCard = new() { Speed = 20, Attack = 10, Health = 0 };
            CombatantCreation zeroHealthCombatant = _humanCombatantCreation with { StatCard = zeroHealthStatCard };
            
            Assert.DoesNotThrow(() => DispatchMessage(zeroHealthCombatant));
            
            _responseListener.AssertWasCalled(false);
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertBaseError<NumberZeroException>(_errorListener.Error.BaseError);
            AssertErrorCollection(zeroHealthCombatant);
        }
    }
}
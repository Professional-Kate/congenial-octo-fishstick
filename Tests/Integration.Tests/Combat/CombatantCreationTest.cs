using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Combatant.Contracts.Enum;
using IdelPog.Combat.Combatant.Contracts.Error;
using IdelPog.Combat.Combatant.Contracts.Response;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Exceptions;

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
                HealthCard = new HealthCard { Health = 20, BaseHealth = 20 },
                AgilityCard = new AgilityCard { Speed = 5, Initiative = 1 }
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
                Assert.That(response.HealthCard, Is.EqualTo(source.HealthCard));
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
            Assert.That(_responseListener.Responses[0].CombatantID, Is.Zero);
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
        public void Negative_DispatchMessage_CombatantHasZeroSpeed_DispatchesError()
        {
            AgilityCard zeroSpeedCard = new() { Speed = 0, Initiative = 1 };
            CombatantCreation zeroSpeedCombatant = _humanCombatantCreation with { AgilityCard = zeroSpeedCard };
            
            Assert.DoesNotThrow(() => DispatchMessage(zeroSpeedCombatant));
            
            _responseListener.AssertWasCalled(false);
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertBaseError<NumberZeroException>(_errorListener.Error.BaseError);
            AssertErrorCollection(zeroSpeedCombatant);
        }
        
        [Test]
        public void Negative_DispatchMessage_CombatantHasZeroInitiative_DispatchesError()
        {
            AgilityCard zeroInitiativeCard = new() { Speed = 1, Initiative = 0 };
            CombatantCreation zeroSpeedCombatant = _humanCombatantCreation with { AgilityCard = zeroInitiativeCard };
            
            Assert.DoesNotThrow(() => DispatchMessage(zeroSpeedCombatant));
            
            _responseListener.AssertWasCalled(false);
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertBaseError<NumberZeroException>(_errorListener.Error.BaseError);
            AssertErrorCollection(zeroSpeedCombatant);
        }
        
        [Test]
        public void Positive_DispatchMessage_CombatantHasZeroBaseHealth_DispatchesResponse()
        {
            HealthCard zeroBaseHealthHealthCard = new() { Health = 1, BaseHealth = 0 };
            CombatantCreation zeroHealthCombatant = _humanCombatantCreation with { HealthCard = zeroBaseHealthHealthCard };
            
            Assert.DoesNotThrow(() => DispatchMessage(zeroHealthCombatant));
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], zeroHealthCombatant);
        }
        
        [Test]
        public void Negative_DispatchMessage_CombatantHasZeroHealth_DispatchesError()
        {
            HealthCard zeroHealthHealthCard = new() { Health = 0, BaseHealth = 20 };
            CombatantCreation zeroHealthCombatant = _humanCombatantCreation with { HealthCard = zeroHealthHealthCard };
            
            Assert.DoesNotThrow(() => DispatchMessage(zeroHealthCombatant));
            
            _responseListener.AssertWasCalled(false);
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertBaseError<NumberZeroException>(_errorListener.Error.BaseError);
            AssertErrorCollection(zeroHealthCombatant);
        }
    }
}
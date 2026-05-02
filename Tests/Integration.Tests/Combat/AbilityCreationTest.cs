using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Error;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Exceptions;
using IdelPog.Core.Contracts;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Integration.Tests.Combat
{
    [TestFixture]
    public sealed class AbilityCreationTest : ManagedTestBuffer
    {
        private ManagedResponseListener<AbilityCreationResponse> _responseListener;
        private ManagedErrorListener<AbilityCreationError> _errorListener;

        private AbilityCreation _basicAttackCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _basicAttackCreation = new AbilityCreation
            {
                Information = new Information { Name = "Basic attack", Description = "Attack an enemy but kinda basically" },
                AbilityType = AbilityType.BASIC_ATTACK,
                Cooldown = 9,
                Damage = 3,
                AbilitySlots = 1
            };
        }

        [SetUp]
        public void Setup()
        {
            _responseListener = new ManagedResponseListener<AbilityCreationResponse>();
            _errorListener = new ManagedErrorListener<AbilityCreationError>();
            
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
        }
        
        private void DispatchCombatantSkillCreation(params AbilityCreation[] combatantSkillCreations)
        {
            IBuffer<AbilityCreation> buffer = BufferManager.RequestBuffer<AbilityCreation>(new BufferRequest(combatantSkillCreations.Length));
            buffer.Assign(combatantSkillCreations);
            buffer.MarkReady();
        }
        
        private void AssertResponseListenerCalled(bool wasCalled)
        {
            Assert.That(_responseListener.WasCalled, Is.EqualTo(wasCalled));
        }

        private void AssertResponseLength(int length)
        {
            Assert.That(_responseListener.Responses, Has.Length.EqualTo(length));
        }

        private static void AssertResponse(AbilityCreationResponse basicEncounterDeck, AbilityCreation expected)
        { 
            Assert.Multiple(() =>
            {
                Assert.That(basicEncounterDeck.Information, Is.EqualTo(expected.Information));
                Assert.That(basicEncounterDeck.AbilityType, Is.EqualTo(expected.AbilityType));
                Assert.That(basicEncounterDeck.Cooldown, Is.EqualTo(expected.Cooldown));
                Assert.That(basicEncounterDeck.Damage, Is.EqualTo(expected.Damage));
            });
        }
        
        private void AssertErrorListenerCalled(bool wasCalled)
        {
            Assert.That(_errorListener.WasCalled, Is.EqualTo(wasCalled));
        }

        private void AssertErrorLength(int length)
        {
            Assert.That(_errorListener.Error.AbilityCreations, Has.Length.EqualTo(length));
        }

        private void AssertError<TException>(params AbilityCreation[] combatantSkillCreations) where TException : Exception
        {
            AbilityCreationError abilityCreationError = _errorListener.Error;
            
            Assert.Multiple(() =>
            {
                Assert.That(abilityCreationError.BaseError.Exception, Is.TypeOf<ControllerThrownException>());
                Assert.That(abilityCreationError.BaseError.Exception.GetBaseException(), Is.TypeOf<TException>());
                Assert.That(abilityCreationError.AbilityCreations, Is.EqualTo(combatantSkillCreations));
            });
        }

        [Test]
        public void Positive_DispatchCommands_CreatesNewSkill()
        { 
            Assert.DoesNotThrow(() => DispatchCombatantSkillCreation(_basicAttackCreation));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], _basicAttackCreation);
        }

        [Test]
        public void Positive_DispatchCommands_CreatesMultipleSkills()
        {
            AbilityCreation abilityCreation = _basicAttackCreation with { AbilityType = (AbilityType) 2, Damage = 1, Cooldown = 4 };
            Assert.DoesNotThrow(() => DispatchCombatantSkillCreation(_basicAttackCreation, abilityCreation));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(2);
            AssertResponse(_responseListener.Responses[0], _basicAttackCreation);
            AssertResponse(_responseListener.Responses[1], abilityCreation);
        }

        [Test]
        public void Negative_DispatchCommands_ZeroSpeed_DispatchesError()
        {
            AbilityCreation zeroSpeedAbility = _basicAttackCreation with { Cooldown = 0 };
            Assert.DoesNotThrow(() => DispatchCombatantSkillCreation(zeroSpeedAbility));
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError<NumberZeroException>(zeroSpeedAbility);
        }

        [Test]
        public void Negative_DispatchCommands_DuplicateSkillType_DispatchesError()
        {
            Assert.DoesNotThrow(() => DispatchCombatantSkillCreation(_basicAttackCreation, _basicAttackCreation));
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(2);
            AssertError<DuplicateEntityException>(_basicAttackCreation, _basicAttackCreation);
        }
    }
}
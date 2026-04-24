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
    public sealed class CombatantAbilityCreationTest : ManagedTestBuffer
    {
        private ManagedResponseListener<CombatantAbilityCreationResponse> _responseListener;
        private ManagedErrorListener<CombatantAbilityCreationError> _errorListener;

        private CombatantAbilityCreation _basicAttackCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _basicAttackCreation = new CombatantAbilityCreation
            {
                Information = new Information { Name = "Basic attack", Description = "Attack an enemy but kinda basically" },
                AbilityType = AbilityType.BASIC_ATTACK,
                Speed = 9,
                Damage = 3
            };
        }

        [SetUp]
        public void Setup()
        {
            _responseListener = new ManagedResponseListener<CombatantAbilityCreationResponse>();
            _errorListener = new ManagedErrorListener<CombatantAbilityCreationError>();
            
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
        }
        
        private void DispatchCombatantSkillCreation(params CombatantAbilityCreation[] combatantSkillCreations)
        {
            IBuffer<CombatantAbilityCreation> buffer = BufferManager.RequestBuffer<CombatantAbilityCreation>(new BufferRequest(combatantSkillCreations.Length));
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

        private static void AssertResponse(CombatantAbilityCreationResponse basicEncounterDeck, CombatantAbilityCreation expected)
        { 
            Assert.Multiple(() =>
            {
                Assert.That(basicEncounterDeck.Information, Is.EqualTo(expected.Information));
                Assert.That(basicEncounterDeck.AbilityType, Is.EqualTo(expected.AbilityType));
                Assert.That(basicEncounterDeck.Speed, Is.EqualTo(expected.Speed));
                Assert.That(basicEncounterDeck.Damage, Is.EqualTo(expected.Damage));
            });
        }
        
        private void AssertErrorListenerCalled(bool wasCalled)
        {
            Assert.That(_errorListener.WasCalled, Is.EqualTo(wasCalled));
        }

        private void AssertErrorLength(int length)
        {
            Assert.That(_errorListener.Error.CombatantAbilityCreations, Has.Length.EqualTo(length));
        }

        private void AssertError<TException>(params CombatantAbilityCreation[] combatantSkillCreations) where TException : Exception
        {
            CombatantAbilityCreationError combatantAbilityCreationError = _errorListener.Error;
            
            Assert.Multiple(() =>
            {
                Assert.That(combatantAbilityCreationError.BaseError.Exception, Is.TypeOf<ControllerThrownException>());
                Assert.That(combatantAbilityCreationError.BaseError.Exception.GetBaseException(), Is.TypeOf<TException>());
                Assert.That(combatantAbilityCreationError.CombatantAbilityCreations, Is.EqualTo(combatantSkillCreations));
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
            CombatantAbilityCreation combatantAbilityCreation = _basicAttackCreation with { AbilityType = (AbilityType) 2, Damage = 1, Speed = 4 };
            Assert.DoesNotThrow(() => DispatchCombatantSkillCreation(_basicAttackCreation, combatantAbilityCreation));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(2);
            AssertResponse(_responseListener.Responses[0], _basicAttackCreation);
            AssertResponse(_responseListener.Responses[1], combatantAbilityCreation);
        }

        [Test]
        public void Negative_DispatchCommands_ZeroSpeed_DispatchesError()
        {
            CombatantAbilityCreation zeroSpeedAbility = _basicAttackCreation with { Speed = 0 };
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
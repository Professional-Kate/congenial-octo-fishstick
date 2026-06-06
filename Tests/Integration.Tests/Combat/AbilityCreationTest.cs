using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Error;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Event;
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
                AbilityCard = new AbilityCard {  AbilityType = AbilityType.SLASH, EventType = EventType.DIRECT_DAMAGE, Cooldown = 9, AbilitySlots = 1, CastTime = 0},
                ElementalDamageCard = new ElementalDamageCard { ColdDamage = 0, LightningDamage = 0, FireDamage = 0 },
                PhysicalDamageCard = new PhysicalDamageCard { SlashDamage = 3, StrikeDamage = 0, ThrustDamage = 0 },
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
                Assert.That(basicEncounterDeck.AbilityType, Is.EqualTo(expected.AbilityCard.AbilityType));
                Assert.That(basicEncounterDeck.ElementalDamageCard, Is.EqualTo(expected.ElementalDamageCard));
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
            ElementalDamageCard oneElementalDamageCard = _basicAttackCreation.ElementalDamageCard with { FireDamage = 1 };
            
            AbilityCard abilityCard = _basicAttackCreation.AbilityCard with { Cooldown = 4, AbilityType = AbilityType.STAB };
            AbilityCreation abilityCreation = _basicAttackCreation with { AbilityCard = abilityCard, ElementalDamageCard = oneElementalDamageCard };
            Assert.DoesNotThrow(() => DispatchCombatantSkillCreation(_basicAttackCreation, abilityCreation));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(2);
            AssertResponse(_responseListener.Responses[0], _basicAttackCreation);
            AssertResponse(_responseListener.Responses[1], abilityCreation);
        }

        [Test]
        public void Positive_CanCreateAbility_AtMax_AndMin_Damage()
        {
            ElementalDamageCard minElementalDamageCard = new() { LightningDamage = uint.MinValue, ColdDamage = uint.MinValue, FireDamage = uint.MinValue };
            PhysicalDamageCard minPhysicalDamageCard = new() { SlashDamage = uint.MinValue, StrikeDamage = uint.MinValue, ThrustDamage = uint.MinValue };

            AbilityCard abilityCard = _basicAttackCreation.AbilityCard with { AbilityType = AbilityType.STAB };
            AbilityCreation strongAttackCreation = _basicAttackCreation with { AbilityCard = abilityCard, ElementalDamageCard = minElementalDamageCard, PhysicalDamageCard = minPhysicalDamageCard };
            
            ElementalDamageCard maxElementalDamageCard = new() { LightningDamage = uint.MaxValue, ColdDamage = uint.MaxValue, FireDamage = uint.MaxValue };
            PhysicalDamageCard maxPhysicalDamageCard = new() { SlashDamage = uint.MaxValue, StrikeDamage = uint.MaxValue, ThrustDamage = uint.MaxValue };
            AbilityCreation basicAttackCreation = _basicAttackCreation with { ElementalDamageCard = maxElementalDamageCard, PhysicalDamageCard = maxPhysicalDamageCard };
            
            Assert.DoesNotThrow(() => DispatchCombatantSkillCreation(basicAttackCreation, strongAttackCreation));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(2);
            AssertResponse(_responseListener.Responses[0], basicAttackCreation);
            AssertResponse(_responseListener.Responses[1], strongAttackCreation);
        }

        [Test]
        public void Negative_DispatchCommands_ZeroSpeed_DispatchesError()
        {
            AbilityCard abilityCard = _basicAttackCreation.AbilityCard with { Cooldown = 0 };
            AbilityCreation zeroSpeedAbility = _basicAttackCreation with { AbilityCard = abilityCard };
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
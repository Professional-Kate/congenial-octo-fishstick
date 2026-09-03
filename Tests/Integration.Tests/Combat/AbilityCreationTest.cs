using IdelPog.Combat.Ability.Contracts.Command;
using IdelPog.Combat.Ability.Contracts.Error;
using IdelPog.Combat.Ability.Contracts.Response;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Runtime.Event;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Integration.Tests.Combat.Tools;

namespace IdelPog.Integration.Tests.Combat
{
    [TestFixture]
    public sealed class AbilityCreationTest : ManagedTestBuffer
    {
        private ManagedResponseListener<AbilityCreationResponse> _responseListener;
        private ManagedErrorListener<AbilityCreationError> _errorListener;

        [SetUp]
        public void Setup()
        {
            _responseListener = new ManagedResponseListener<AbilityCreationResponse>();
            _errorListener = new ManagedErrorListener<AbilityCreationError>();
            
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
        }
        
        private void AssertResponseListenerCalled(bool wasCalled)
        {
            Assert.That(_responseListener.WasCalled, Is.EqualTo(wasCalled));
        }

        private void AssertResponseLength(int length)
        {
            Assert.That(_responseListener.Responses, Has.Length.EqualTo(length));
        }

        private static void AssertResponse(AbilityCreationResponse abilityCreationResponse, AbilityCreation abilityCreationSource, byte id = 0)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(abilityCreationResponse.AbilityID, Is.EqualTo(id));
                Assert.That(abilityCreationResponse.AbilityCard, Is.EqualTo(abilityCreationSource.AbilityCard));
                Assert.That(abilityCreationResponse.TriggerCard, Is.EqualTo(abilityCreationSource.TriggerCard));
            }
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

            using (Assert.EnterMultipleScope())
            {
                Assert.That(abilityCreationError.BaseError.Exception, Is.TypeOf<ControllerThrownException>());
                Assert.That(abilityCreationError.BaseError.Exception.GetBaseException(), Is.TypeOf<TException>());
                Assert.That(abilityCreationError.AbilityCreations, Is.EqualTo(combatantSkillCreations));
            }
        }

        [Test]
        public void Positive_SingleCommand_CreatesNewAbility()
        { 
            Assert.DoesNotThrow(() => DispatchMessage(StaticCombatCommands.SlashAttackCreation));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], StaticCombatCommands.SlashAttackCreation);
        }

        [Test]
        public void Positive_MultipleCommands_CreatesMultipleAbilities()
        {
            Assert.DoesNotThrow(() => DispatchMessage(StaticCombatCommands.SlashAttackCreation, StaticCombatCommands.StabAttackCreation));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(2);
            AssertResponse(_responseListener.Responses[0], StaticCombatCommands.SlashAttackCreation);
            AssertResponse(_responseListener.Responses[1], StaticCombatCommands.StabAttackCreation, 1);
        }

        [Test]
        public void Positive_CanCreateAbility_AtMaxAndMin_Damage()
        {
            AbilityCreation maxAttackDamage = new()
            {
                AbilityCard = new AbilityCard { Cooldown = 10, AbilitySlots = 1 },
                TriggerCard = StaticCombatCommands.AbilityReadyTrigger,
                AbilityStageCards = [ new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.HOLY, CastTime = 0, MaxTargets = 1, Value = uint.MaxValue, Priority = 0 } ]
            };
            
            AbilityCreation minAttackDamage = new()
            {
                AbilityCard = new AbilityCard { Cooldown = 10, AbilitySlots = 1 },
                TriggerCard = StaticCombatCommands.AbilityReadyTrigger,
                AbilityStageCards = [ new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.FIRE, CastTime = 0,  MaxTargets = 1, Value = uint.MinValue, Priority = 0 } ]
            };
            
            Assert.DoesNotThrow(() => DispatchMessage(maxAttackDamage, minAttackDamage));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(2);
            AssertResponse(_responseListener.Responses[0], maxAttackDamage);
            AssertResponse(_responseListener.Responses[1], minAttackDamage, 1);
        }

        [Test]
        public void Negative_ZeroCooldown_DispatchesError()
        {
            AbilityCreation zeroCooldownAbility = StaticCombatCommands.SlashAttackCreation with { AbilityCard = new AbilityCard { Cooldown = 0, AbilitySlots = 1 }};
            
            Assert.DoesNotThrow(() => DispatchMessage(zeroCooldownAbility));
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError<NumberZeroException>(zeroCooldownAbility);
        }

        private static IEnumerable<TriggerCard> BadAbilityReadyTriggers()
        {
            yield return new TriggerCard
            {
                TriggerEventType = TriggerEventType.ABILITY_READY,
                TargetingType = TargetingType.ENEMY,
                MinTriggerValue = 0,
                MaxTriggerValue = 0
            };
            yield return new TriggerCard
            {
                TriggerEventType = TriggerEventType.ABILITY_READY,
                TargetingType = TargetingType.SELF,
                MinTriggerValue = 1,
                MaxTriggerValue = 0
            };
            yield return new TriggerCard
            {
                TriggerEventType = TriggerEventType.ABILITY_READY,
                TargetingType = TargetingType.SELF,
                MinTriggerValue = 0,
                MaxTriggerValue = 1
            };
        }
        
        [TestCaseSource(nameof(BadAbilityReadyTriggers))]
        public void Negative_BadAbilityReadyTriggerCard_DispatchesError(TriggerCard triggerCard)
        {
            AbilityCreation badTriggerCreation = StaticCombatCommands.SlashAttackCreation with { TriggerCard = triggerCard };
            
            DispatchMessage(badTriggerCreation);
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError<AbilityReadyException>(badTriggerCreation);
        }
    }
}
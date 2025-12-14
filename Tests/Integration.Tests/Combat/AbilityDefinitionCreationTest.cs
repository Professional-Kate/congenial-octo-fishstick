using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Error;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Core.Contracts;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Integration.Tests.Combat
{
    [TestFixture]
    public sealed class AbilityDefinitionCreationTest : ManagedTestBuffer
    {
        private ManagedResponseListener<AbilityDefinitionCreationResponse> _responseListener;
        private ManagedErrorListener<AbilityDefinitionCreationError> _errorListener;
        
        private AbilityDefinitionCreation _stabDefinition;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _stabDefinition = new AbilityDefinitionCreation
            {
                AbilityType = AbilityType.STAB,
                TargetingInformation = new TargetingInformation { TargetingType = TargetingType.SINGLE, MaxTargets = 1 },
                Information = new Information { Name = "Stab!", Description = "Stab a single enemy!" },
                Cooldown = 1,
                Damage = 3
            };
        }
        
        [SetUp]
        public void Setup()
        {
            _responseListener = new ManagedResponseListener<AbilityDefinitionCreationResponse>();
            _errorListener = new ManagedErrorListener<AbilityDefinitionCreationError>();
            
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
        }
        
        private void DispatchAbilityDefinitionCreation(params AbilityDefinitionCreation[] abilityDefinitionCreations)
        {
            IBuffer<AbilityDefinitionCreation> buffer = BufferManager.RequestBuffer<AbilityDefinitionCreation>(new BufferRequest(abilityDefinitionCreations.Length));
            buffer.Assign(abilityDefinitionCreations);
            buffer.MarkReady();
        }

        private void AssertResponseListenerCalled(bool called)
        {
            Assert.That(_responseListener.WasCalled, Is.EqualTo(called));
        }

        private void AssertResponseLength(int length)
        {
            Assert.That(_responseListener.Responses, Has.Length.EqualTo(length));
        }

        private static void AssertResponse(AbilityDefinitionCreationResponse abilityDefinitionCreationResponse, AbilityDefinitionCreation abilityDefinitionCreation)
        {
            Assert.Multiple(() =>
            {
                Assert.That(abilityDefinitionCreationResponse.AbilityType, Is.EqualTo(abilityDefinitionCreation.AbilityType));
                Assert.That(abilityDefinitionCreationResponse.TargetingInformation, Is.EqualTo(abilityDefinitionCreation.TargetingInformation));
                Assert.That(abilityDefinitionCreationResponse.Information, Is.EqualTo(abilityDefinitionCreation.Information));
                Assert.That(abilityDefinitionCreationResponse.Cooldown, Is.EqualTo(abilityDefinitionCreation.Cooldown));
                Assert.That(abilityDefinitionCreationResponse.Damage, Is.EqualTo(abilityDefinitionCreation.Damage));
            });
        }
        
        private void AssertErrorListenerCalled(bool called)
        {
            Assert.That(_errorListener.WasCalled, Is.EqualTo(called));
        }

        private void AssertErrorLength(int length)
        {
            Assert.That(_errorListener.Error.AbilityDefinitionCreations, Has.Length.EqualTo(length));
        }

        private void AssertError<TException>(params AbilityDefinitionCreation[] arenaCreations) where TException : Exception
        {
            BaseError baseError = _errorListener.Error.BaseError;
            Assert.Multiple(() =>
            {
                Assert.That(_errorListener.Error.AbilityDefinitionCreations, Is.EqualTo(arenaCreations));
                Assert.That(baseError.Exception, Is.TypeOf<ControllerThrownException>());
                Assert.That(baseError.Exception.InnerException, Is.TypeOf<TException>());
            });
        }

        [Test]
        public void Positive_DispatchSingleCreation_CreatesDefinition_DispatchesResponse()
        { 
            DispatchAbilityDefinitionCreation(_stabDefinition);
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], _stabDefinition);
        }
        
        [Test]
        public void Positive_DispatchMultipleCreations_CreatesDefinitions_DispatchesResponses()
        { 
            AbilityDefinitionCreation slashDefinition = _stabDefinition with { AbilityType =  AbilityType.SLASH };
            
            DispatchAbilityDefinitionCreation(_stabDefinition, slashDefinition);
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(2);
            AssertResponse(_responseListener.Responses[0], _stabDefinition);
            AssertResponse(_responseListener.Responses[1], slashDefinition);
        }
        
        [Test]
        public void Positive_DispatchSingleCreation_ZeroCooldown_DispatchesResponse()
        { 
            AbilityDefinitionCreation zeroCooldownDefinition = _stabDefinition with { Cooldown = 0 };
            
            DispatchAbilityDefinitionCreation(zeroCooldownDefinition);
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], zeroCooldownDefinition);
        }

        [Test]
        public void Negative_DispatchMultipleCreations_DuplicateAbilityType_DispatchesError()
        {
            DispatchAbilityDefinitionCreation(_stabDefinition, _stabDefinition);
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(2);
            AssertError<DuplicateEntityException>(_stabDefinition, _stabDefinition);
        }

        [Test]
        public void Negative_DispatchSingleCreation_ZeroDamage_DispatchesError()
        {
            AbilityDefinitionCreation zeroDamageDefinition = _stabDefinition with { Damage = 0 };
            
            DispatchAbilityDefinitionCreation(zeroDamageDefinition);
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError<AmountZeroException>(zeroDamageDefinition);
        }
        
        [Test]
        public void Negative_DispatchSingleCreation_ZeroTargets_DispatchesError()
        {
            AbilityDefinitionCreation zeroTargetsDefinition = _stabDefinition with { TargetingInformation = new TargetingInformation { TargetingType = TargetingType.SINGLE, MaxTargets = 0 }};
            
            DispatchAbilityDefinitionCreation(zeroTargetsDefinition);
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError<AmountZeroException>(zeroTargetsDefinition);
        }
    }
}
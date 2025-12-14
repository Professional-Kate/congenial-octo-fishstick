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
    public sealed class CombatantDefinitionCreationTest : ManagedTestBuffer
    {
        private ManagedResponseListener<CombatantDefinitionCreationResponse> _responseListener;
        private ManagedErrorListener<CombatantDefinitionCreationError> _errorListener;

        private CombatantDefinitionCreation _wolfDefinitionCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _wolfDefinitionCreation = new CombatantDefinitionCreation
            { 
                CombatantType = CombatantType.WOLF,
                CombatantStats = new CombatantStats { Attack = 5, Health = 2, Speed = 7 },
                Information = new Information { Name = "Fierce Wolf", Description = "Very scary!!" } 
            };
        }
        
        [SetUp]
        public void Setup()
        {
            _responseListener = new ManagedResponseListener<CombatantDefinitionCreationResponse>();
            _errorListener = new ManagedErrorListener<CombatantDefinitionCreationError>();
            
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
        }
        
        private void DispatchCombatantDefinitionCreations(params CombatantDefinitionCreation[] combatantDefinitionCreations)
        { 
            IBuffer<CombatantDefinitionCreation> buffer = BufferManager.RequestBuffer<CombatantDefinitionCreation>(new BufferRequest(combatantDefinitionCreations.Length));
            buffer.Assign(combatantDefinitionCreations);
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

        private static void AssertResponse(CombatantDefinitionCreationResponse combatantDefinitionCreationResponse, CombatantDefinitionCreation combatantDefinitionCreation)
        {
            Assert.Multiple(() =>
            {
                Assert.That(combatantDefinitionCreationResponse.CombatantType, Is.EqualTo(combatantDefinitionCreation.CombatantType));
                Assert.That(combatantDefinitionCreationResponse.CombatantStats, Is.EqualTo(combatantDefinitionCreation.CombatantStats));
                Assert.That(combatantDefinitionCreationResponse.Information, Is.EqualTo(combatantDefinitionCreation.Information));
            });
        }
        
        private void AssertErrorListenerCalled(bool called)
        {
            Assert.That(_errorListener.WasCalled, Is.EqualTo(called));
        }

        private void AssertErrorLength(int length)
        {
            Assert.That(_errorListener.Error.CombatantDefinitionsCreations, Has.Length.EqualTo(length));
        }

        private void AssertError<TException>(params CombatantDefinitionCreation[] combatantDefinitionsCreations) where TException : Exception
        {
            BaseError baseError = _errorListener.Error.BaseError;
            Assert.Multiple(() =>
            {
                Assert.That(_errorListener.Error.CombatantDefinitionsCreations, Is.EqualTo(combatantDefinitionsCreations));
                Assert.That(baseError.Exception, Is.TypeOf<ControllerThrownException>());
                Assert.That(baseError.Exception.InnerException, Is.TypeOf<TException>());
            });
        }

        [Test]
        public void Positive_DispatchSingleCreation_DispatchesResponse()
        {
            DispatchCombatantDefinitionCreations(_wolfDefinitionCreation);

            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], _wolfDefinitionCreation);
        }
        
        [Test]
        public void Positive_DispatchMultipleCreations_DispatchesResponse()
        {
            CombatantDefinitionCreation slimeCreation = _wolfDefinitionCreation with { CombatantType = CombatantType.SLIME };
            
            DispatchCombatantDefinitionCreations(_wolfDefinitionCreation, slimeCreation);

            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(2);
            AssertResponse(_responseListener.Responses[0], _wolfDefinitionCreation);
            AssertResponse(_responseListener.Responses[1], slimeCreation);
        }

        [Test]
        public void Negative_DispatchMultipleCreations_DuplicateCombatantType_DispatchesError()
        {
            DispatchCombatantDefinitionCreations(_wolfDefinitionCreation, _wolfDefinitionCreation);
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(2);
            AssertError<DuplicateEntityException>(_wolfDefinitionCreation, _wolfDefinitionCreation);
        }

        [Test]
        public void Negative_DispatchSingleCreation_ZeroStats_DispatchesError()
        {
            CombatantDefinitionCreation zeroStatCreation = _wolfDefinitionCreation with { CombatantStats = new CombatantStats { Attack = 0, Health = 0, Speed = 0 }};
            
            DispatchCombatantDefinitionCreations(zeroStatCreation);
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError<AmountZeroException>(zeroStatCreation);
        }
    }
}
using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.HarvestNode.Contracts.Command;
using IdelPog.HarvestNode.Contracts.Error;
using IdelPog.HarvestNode.Contracts.Response;

namespace IdelPog.Integration.Tests.HarvestNode.Unlock.Creation
{
    [TestFixture]
    public sealed class NodeRequirementsCreationTest : ManagedTestBuffer
    {
        private HarvestNodeRequirementsCreation _miningCreation;
        private ManagedResponseListener<HarvestNodeRequirementsCreationResponse> _responseListener;
        private ManagedErrorListener<HarvestNodeRequirementsCreationError> _errorListener;
        private HarvestNodeUnlockDispatcher _harvestNodeUnlockDispatcher;

        [SetUp]
        public void Setup()
        {
            _harvestNodeUnlockDispatcher = new HarvestNodeUnlockDispatcher(BufferManager);
            _responseListener = new ManagedResponseListener<HarvestNodeRequirementsCreationResponse>();
            _errorListener = new ManagedErrorListener<HarvestNodeRequirementsCreationError>();
            
            _miningCreation = _harvestNodeUnlockDispatcher.MiningCreation;
            
            ManagedSubscribe(_errorListener);
            ManagedSubscribe(_responseListener);
        }
        
        private void AssertResponseListenerCalled(bool wasCalled)
        { 
            Assert.That(_responseListener.WasCalled, Is.EqualTo(wasCalled));
        }

        private void AssertResponseLength(int expectedLength)
        {
            Assert.That(_responseListener.Responses, Has.Length.EqualTo(expectedLength));
        }

        private static void AssertResponse(HarvestNodeRequirementsCreation creation, HarvestNodeRequirementsCreationResponse response)
        {
            Assert.Multiple(() =>
            {
                Assert.That(response.SkillID, Is.EqualTo(creation.SkillID));
                Assert.That(response.HarvestNodeRequirements, Is.EqualTo(creation.HarvestNodeRequirements));
            });
        }

        private void AssertErrorListenerCalled(bool wasCalled)
        {
            Assert.That(_errorListener.WasCalled, Is.EqualTo(wasCalled));
        }
        
        private void AssertErrorLength(int expectedLength)
        {
            Assert.That(_errorListener.Error.HarvestNodeRequirementsCreations, Has.Length.EqualTo(expectedLength));
        }

        private void AssertError(Type exception, HarvestNodeRequirementsCreation[] creations)
        {
            HarvestNodeRequirementsCreationError error = _errorListener.Error;

            BaseError baseError = error.BaseError;
            Assert.Multiple(() =>
            {
                Assert.That(baseError.Exception, Is.TypeOf<ControllerThrownException>());
                Assert.That(baseError.Exception.InnerException, Is.TypeOf(exception));
            });
            
            Assert.That(error.HarvestNodeRequirementsCreations, Is.EqualTo(creations));
        }

        [Test]
        public void Positive_SendCreationCommand_DispatchesResponse_NormalResponse()
        { 
            Assert.DoesNotThrow(() => _harvestNodeUnlockDispatcher.DispatchCreations(_miningCreation));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            
            AssertResponse(_miningCreation, _responseListener.Responses[0]);
        }

        [Test]
        public void Positive_SendCreationBuffers_MultipleCommands_NormalResponse()
        {
            HarvestNodeRequirementsCreation foragingCreation = _miningCreation with { SkillID = SkillID.FORAGING };
            Assert.DoesNotThrow(() => _harvestNodeUnlockDispatcher.DispatchCreations(_miningCreation, foragingCreation));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(2);
            
            AssertResponse(_miningCreation, _responseListener.Responses[0]);
            AssertResponse(foragingCreation, _responseListener.Responses[1]);
        }

        [Test]
        public void Negative_SendCreationCommand_DuplicateSkillID_ErrorResponse()
        {
            Assert.DoesNotThrow(() => _harvestNodeUnlockDispatcher.DispatchCreations(_miningCreation, _miningCreation));
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(2);
            
            AssertError(typeof(DuplicateEntityException), [_miningCreation, _miningCreation]);
        }

        [Test]
        public void Negative_SendMultipleBuffers_ThirdIsDuplicate_ErrorResponse()
        {
            Assert.DoesNotThrow(() => _harvestNodeUnlockDispatcher.DispatchCreations(_miningCreation));
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_miningCreation, _responseListener.Responses[0]);
            
            Assert.DoesNotThrow(() => _harvestNodeUnlockDispatcher.DispatchCreations(_miningCreation with { SkillID = SkillID.FORAGING }));
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_miningCreation with { SkillID = SkillID.FORAGING }, _responseListener.Responses[0]);
            
            Assert.DoesNotThrow(() => _harvestNodeUnlockDispatcher.DispatchCreations(_miningCreation));
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError(typeof(DuplicateEntityException), [_miningCreation]);
        }
    }
}
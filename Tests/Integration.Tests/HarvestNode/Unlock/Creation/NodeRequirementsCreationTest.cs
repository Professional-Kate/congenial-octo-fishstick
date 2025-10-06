using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Integration.Tests.HarvestNode.Unlock.Creation
{
    [TestFixture]
    public sealed class NodeRequirementsCreationTest : ManagedTestBuffer
    {
        private HarvestNodeRequirementsCreation _miningCreation;
        private RequirementsCreationErrorListener _errorListener;
        private RequirementsCreationResponseListener _responseListener;
        private HarvestNodeUnlockDispatcher _harvestNodeUnlockDispatcher;

        [SetUp]
        public void Setup()
        {
            _harvestNodeUnlockDispatcher = new HarvestNodeUnlockDispatcher(BufferManager);
            _errorListener = new RequirementsCreationErrorListener();
            _responseListener = new RequirementsCreationResponseListener();
            
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
            Assert.That(_responseListener.HarvestNodeRequirementsCreationResponses, Has.Length.EqualTo(expectedLength));
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
            Assert.That(_errorListener.HarvestNodeRequirementsCreationError.HarvestNodeRequirementsCreations, Has.Length.EqualTo(expectedLength));
        }

        private void AssertError(Type exception, HarvestNodeRequirementsCreation[] creations)
        {
            HarvestNodeRequirementsCreationError error = _errorListener.HarvestNodeRequirementsCreationError;

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
            
            AssertResponse(_miningCreation, _responseListener.HarvestNodeRequirementsCreationResponses[0]);
        }

        [Test]
        public void Positive_SendCreationBuffers_MultipleCommands_NormalResponse()
        {
            HarvestNodeRequirementsCreation foragingCreation = _miningCreation with { SkillID = SkillID.FORAGING };
            Assert.DoesNotThrow(() => _harvestNodeUnlockDispatcher.DispatchCreations(_miningCreation, foragingCreation));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(2);
            
            AssertResponse(_miningCreation, _responseListener.HarvestNodeRequirementsCreationResponses[0]);
            AssertResponse(foragingCreation, _responseListener.HarvestNodeRequirementsCreationResponses[1]);
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
            AssertResponse(_miningCreation, _responseListener.HarvestNodeRequirementsCreationResponses[0]);
            
            Assert.DoesNotThrow(() => _harvestNodeUnlockDispatcher.DispatchCreations(_miningCreation with { SkillID = SkillID.FORAGING }));
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_miningCreation with { SkillID = SkillID.FORAGING }, _responseListener.HarvestNodeRequirementsCreationResponses[0]);
            
            Assert.DoesNotThrow(() => _harvestNodeUnlockDispatcher.DispatchCreations(_miningCreation));
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError(typeof(DuplicateEntityException), [_miningCreation]);
        }
    }
}
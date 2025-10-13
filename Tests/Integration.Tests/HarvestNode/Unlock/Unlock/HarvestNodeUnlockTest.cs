using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.HarvestNode.Contracts.Command;
using IdelPog.HarvestNode.Contracts.Error;
using IdelPog.HarvestNode.Contracts.Response;

namespace IdelPog.Integration.Tests.HarvestNode.Unlock.Unlock
{
    [TestFixture]
    public sealed class HarvestNodeUnlockTest : ManagedTestBuffer
    {
        private HarvestNodeUnlock _miningUnlock;
        private ManagedResponseListener<HarvestNodeUnlockResponse> _responseListener;
        private ManagedErrorListener<HarvestNodeUnlockError> _errorListener;
        private HarvestNodeUnlockDispatcher _harvestNodeUnlockDispatcher;

        [SetUp]
        public void Setup()
        {
            _harvestNodeUnlockDispatcher = new HarvestNodeUnlockDispatcher(BufferManager);
            _responseListener = new ManagedResponseListener<HarvestNodeUnlockResponse>();
            _errorListener = new ManagedErrorListener<HarvestNodeUnlockError>();

            _miningUnlock = _harvestNodeUnlockDispatcher.MiningUnlock;
            
            ManagedSubscribe(_errorListener);
            ManagedSubscribe(_responseListener);
        }

        private void DispatchCreation()
        {
            _harvestNodeUnlockDispatcher.DispatchCreations(_harvestNodeUnlockDispatcher.MiningCreation);
        }
        
        private void AssertResponseListenerCalled(bool wasCalled)
        { 
            Assert.That(_responseListener.WasCalled, Is.EqualTo(wasCalled));
        }

        private void AssertResponseLength(int expectedLength)
        {
            Assert.That(_responseListener.Responses, Has.Length.EqualTo(expectedLength));
        }

        private static void AssertResponse(SkillID skillID, ResourceID resourceID, HarvestNodeUnlockResponse response)
        {
            Assert.Multiple(() =>
            {
                Assert.That(response.SkillID, Is.EqualTo(skillID));
                Assert.That(response.ResourceID, Is.EqualTo(resourceID));
            });
        }

        private void AssertErrorListenerCalled(bool wasCalled)
        {
            Assert.That(_errorListener.WasCalled, Is.EqualTo(wasCalled));
        }
        
        private void AssertErrorLength(int expectedLength)
        {
            Assert.That(_errorListener.Error.HarvestNodeUnlocks, Has.Length.EqualTo(expectedLength));
        }

        private void AssertError(Type exception, HarvestNodeUnlock[] unlocks)
        {
            HarvestNodeUnlockError error = _errorListener.Error;

            BaseError baseError = error.BaseError;
            Assert.Multiple(() =>
            {
                Assert.That(baseError.Exception, Is.TypeOf<ControllerThrownException>());
                Assert.That(baseError.Exception.InnerException, Is.TypeOf(exception));
            });
            
            Assert.That(error.HarvestNodeUnlocks, Is.EqualTo(unlocks));
        }

        [Test]
        public void Positive_SendUnlockCommand_UnlocksHarvestNode()
        {
            DispatchCreation();
            
            Assert.DoesNotThrow(() => _harvestNodeUnlockDispatcher.DispatchUnlocks(_miningUnlock));

            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_miningUnlock.SkillID, ResourceID.STONE, _responseListener.Responses[0]);
        }

        [Test]
        public void Positive_SendUnlockCommand_UnlocksMultipleHarvestNodes()
        {
            DispatchCreation();
            
            Assert.DoesNotThrow(() => _harvestNodeUnlockDispatcher.DispatchUnlocks(_miningUnlock with { SkillLevel = 2 }));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(2);
            AssertResponse(_miningUnlock.SkillID, ResourceID.STONE, _responseListener.Responses[0]);
            AssertResponse(_miningUnlock.SkillID, ResourceID.IRON_CLUSTER, _responseListener.Responses[1]);
        }

        [Test]
        public void Positive_SendUnlockCommand_UnlocksNothing_NoResponse()
        {
            DispatchCreation();
            
            Assert.DoesNotThrow(() => _harvestNodeUnlockDispatcher.DispatchUnlocks(_miningUnlock with { SkillLevel = 0 }));
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(false);
        }

        [Test]
        public void Negative_SendUnlockCommand_SkillIDNotFound_DispatchesError()
        {
            Assert.DoesNotThrow(() => _harvestNodeUnlockDispatcher.DispatchUnlocks(_miningUnlock));
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError(typeof(NotFoundException<SkillID>), [_miningUnlock]);
        }
    }
}
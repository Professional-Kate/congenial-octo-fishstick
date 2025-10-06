using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Integration.Tests.HarvestNode.Unlock.Unlock
{
    [TestFixture]
    public sealed class HarvestNodeUnlockTest : ManagedTestBuffer
    {
        private HarvestNodeUnlock _miningUnlock;
        private HarvestNodeUnlockErrorListener _errorListener;
        private HarvestNodeUnlockResponseListener _responseListener;
        private HarvestNodeUnlockDispatcher _harvestNodeUnlockDispatcher;

        [SetUp]
        public void Setup()
        {
            _harvestNodeUnlockDispatcher = new HarvestNodeUnlockDispatcher(BufferManager);
            _errorListener = new HarvestNodeUnlockErrorListener();
            _responseListener = new HarvestNodeUnlockResponseListener();

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
            Assert.That(_responseListener.HarvestNodeRequirementsCreationResponses, Has.Length.EqualTo(expectedLength));
        }

        private static void AssertResponse(SkillID skillID, ItemID itemID, HarvestNodeUnlockResponse response)
        {
            Assert.Multiple(() =>
            {
                Assert.That(response.SkillID, Is.EqualTo(skillID));
                Assert.That(response.ItemID, Is.EqualTo(itemID));
            });
        }

        private void AssertErrorListenerCalled(bool wasCalled)
        {
            Assert.That(_errorListener.WasCalled, Is.EqualTo(wasCalled));
        }
        
        private void AssertErrorLength(int expectedLength)
        {
            Assert.That(_errorListener.HarvestNodeRequirementsCreationError.HarvestNodeUnlocks, Has.Length.EqualTo(expectedLength));
        }

        private void AssertError(Type exception, HarvestNodeUnlock[] unlocks)
        {
            HarvestNodeUnlockError error = _errorListener.HarvestNodeRequirementsCreationError;

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
            AssertResponse(_miningUnlock.SkillID, ItemID.STONE, _responseListener.HarvestNodeRequirementsCreationResponses[0]);
        }

        [Test]
        public void Positive_SendUnlockCommand_UnlocksMultipleHarvestNodes()
        {
            DispatchCreation();
            
            Assert.DoesNotThrow(() => _harvestNodeUnlockDispatcher.DispatchUnlocks(_miningUnlock with { SkillLevel = 2 }));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(2);
            AssertResponse(_miningUnlock.SkillID, ItemID.STONE, _responseListener.HarvestNodeRequirementsCreationResponses[0]);
            AssertResponse(_miningUnlock.SkillID, ItemID.IRON, _responseListener.HarvestNodeRequirementsCreationResponses[1]);
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
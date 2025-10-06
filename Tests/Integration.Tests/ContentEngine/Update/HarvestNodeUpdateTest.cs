using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Information.Contracts;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Progression;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.HarvestNode.Exceptions;
using IdelPog.Integration.Tests.ContentEngine.Unlock;

namespace IdelPog.Integration.Tests.ContentEngine
{
    [TestFixture]
    public sealed class HarvestNodeUpdateTest : ManagedTestBuffer
    {
        private HarvestNodeUpdate _nodeUpdate;
        private HarvestNodeCreation _harvestNodeCreation;
        private UpdateNodeErrorListener _updateNodeErrorListener;
        private UpdateNodeResponseListener _updateNodeResponseListener;

        [SetUp]
        public void Setup()
        {
            _nodeUpdate = new HarvestNodeUpdate
            {
                ItemID = ItemID.IRON,
                SkillID = SkillID.MINING
            };
            
            _harvestNodeCreation = new HarvestNodeCreation
            {
                ReadOnlyHarvestNodes =
                [
                    new ReadOnlyHarvestNode { ItemID =  ItemID.IRON, ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }, HarvestNodeID = HarvestNodeID.IRON_CLUSTER}
                ],
                LinkedSkill = SkillID.MINING
            };
            
            _updateNodeErrorListener = new UpdateNodeErrorListener();
            _updateNodeResponseListener = new UpdateNodeResponseListener();
            ManagedSubscribe(_updateNodeErrorListener);
            ManagedSubscribe(_updateNodeResponseListener);
        }
        
        private void DispatchNodeCreation(params HarvestNodeCreation[] nodeCreations)
        {
            IBuffer<HarvestNodeCreation> buffer = BufferManager.RequestBuffer<HarvestNodeCreation>(new BufferRequest(nodeCreations.Length));
            buffer.Assign(nodeCreations);
            buffer.MarkReady();
        }

        private void DispatchNodeUpdate(params HarvestNodeUpdate[] nodeUpdates)
        {
            IBuffer<HarvestNodeUpdate> buffer = BufferManager.RequestBuffer<HarvestNodeUpdate>(new BufferRequest(nodeUpdates.Length));
            buffer.Assign(nodeUpdates);
            buffer.MarkReady();
        }

        private void AssertResponseLength(int length)
        {
            Assert.That(_updateNodeResponseListener.HarvestNodeUpdateResponses, Has.Length.EqualTo(length));
        }
        
        private static void AssertResponseListener(HarvestNodeUpdate nodeUpdate, HarvestNodeUpdateResponse response)
        { 
            Assert.That(response.ItemID, Is.EqualTo(nodeUpdate.ItemID));
        }

        private void AssertErrorLength(int length)
        {
            Assert.That(_updateNodeErrorListener.HarvestNodeUpdateError.HarvestNodeUpdates, Has.Length.EqualTo(length));
        }

        private static void AssertErrorListener<TException>(HarvestNodeUpdate[] nodeUpdates, HarvestNodeUpdateError error)
        {
            Assert.That(error.BaseError.Exception.InnerException, Is.Not.Null);
            
            Assert.Multiple(() =>
            {
                Assert.That(error.BaseError.Exception.InnerException.GetType(), Is.EqualTo(typeof(TException)));
                Assert.That(nodeUpdates, Is.EqualTo(error.HarvestNodeUpdates));
            });
        }

        [Test]
        public void Positive_SendCommand_DispatchesResponse_NoError()
        {
            DispatchNodeCreation(_harvestNodeCreation);
            Assert.DoesNotThrow(() => DispatchNodeUpdate(_nodeUpdate));
            
            Assert.Multiple(() =>
            {
                Assert.That(_updateNodeErrorListener.WasCalled, Is.EqualTo(false));
                Assert.That(_updateNodeResponseListener.WasCalled, Is.EqualTo(true));
            });

            AssertResponseLength(1);
            AssertResponseListener(_nodeUpdate, _updateNodeResponseListener.HarvestNodeUpdateResponses[0]);
        }

        [Test]
        public void Positive_SendMultipleCommands_DispatchesResponses_NoError()
        {
            HarvestNodeCreation foragingCreation = new()
            {
                ReadOnlyHarvestNodes =
                [
                    new ReadOnlyHarvestNode { ItemID =  ItemID.STONE, ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }, HarvestNodeID = HarvestNodeID.ROCK}
                ],
                LinkedSkill = SkillID.FORAGING
            };

            HarvestNodeUpdate foragingUpdate = new() { ItemID = ItemID.STONE, SkillID = SkillID.FORAGING };
            
            DispatchNodeCreation(_harvestNodeCreation, foragingCreation);
            Assert.DoesNotThrow(() => DispatchNodeUpdate(_nodeUpdate, foragingUpdate));
            
            Assert.Multiple(() =>
            {
                Assert.That(_updateNodeErrorListener.WasCalled, Is.EqualTo(false));
                Assert.That(_updateNodeResponseListener.WasCalled, Is.EqualTo(true));
            });

            AssertResponseLength(2);
            AssertResponseListener(_nodeUpdate, _updateNodeResponseListener.HarvestNodeUpdateResponses[0]);
            AssertResponseListener(foragingUpdate, _updateNodeResponseListener.HarvestNodeUpdateResponses[1]);
        }

        [Test]
        public void Negative_SendCommand_SkillNotFound_NoUpdate_DispatchesError()
        {
            Assert.DoesNotThrow(() => DispatchNodeUpdate(_nodeUpdate with { SkillID = SkillID.FORAGING }));
            
            Assert.Multiple(() =>
            {
                Assert.That(_updateNodeErrorListener.WasCalled, Is.EqualTo(true));
                Assert.That(_updateNodeResponseListener.WasCalled, Is.EqualTo(false));
            });

            AssertErrorLength(1);
            AssertErrorListener<NotFoundException<SkillID>>([_nodeUpdate with { SkillID = SkillID.FORAGING }], _updateNodeErrorListener.HarvestNodeUpdateError);
        } 
        
        [Test]
        public void Negative_SendCommand_SkillDoesNotAllowResource_NoUpdate_DispatchesError()
        {
            DispatchNodeCreation(_harvestNodeCreation);
            
            Assert.DoesNotThrow(() => DispatchNodeUpdate(_nodeUpdate with { ItemID = ItemID.HERBS }));
            
            Assert.Multiple(() =>
            {
                Assert.That(_updateNodeErrorListener.WasCalled, Is.EqualTo(true));
                Assert.That(_updateNodeResponseListener.WasCalled, Is.EqualTo(false));
            });
            
            AssertErrorLength(1);
            AssertErrorListener<NotFoundException<ItemID>>([_nodeUpdate with { ItemID = ItemID.HERBS }],  _updateNodeErrorListener.HarvestNodeUpdateError);
        }

        [Test]
        public void Negative_SendCommand_NodeIsLocked_NoUpdate_DispatchesError()
        {
            // Creates a HarvestNodeRequirement for Mining:Iron
            HarvestNodeUnlockDispatcher dispatcher = new(BufferManager);
            dispatcher.DispatchCreations(dispatcher.MiningCreation);
            
            DispatchNodeCreation(_harvestNodeCreation);
            Assert.DoesNotThrow(() => DispatchNodeUpdate(_nodeUpdate));
            
            Assert.Multiple(() =>
            {
                Assert.That(_updateNodeErrorListener.WasCalled, Is.EqualTo(true));
                Assert.That(_updateNodeResponseListener.WasCalled, Is.EqualTo(false));
            });
            
            AssertErrorLength(1);
            AssertErrorListener<HarvestNodeLockedException>([_nodeUpdate],  _updateNodeErrorListener.HarvestNodeUpdateError);
        }
    }
}
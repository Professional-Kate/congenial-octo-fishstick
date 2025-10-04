using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Information.Contracts;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Progression;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Integration.Tests.ContentEngine
{
    [TestFixture]
    public class HarvestNodeUpdateTest : ManagedTestBuffer
    {
        private HarvestNodeUpdate _nodeUpdate;
        private NodeCreation _nodeCreation;
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
            
            _nodeCreation = new NodeCreation
            {
                ReadOnlyHarvestNodes =
                [
                    new ReadOnlyHarvestNode { ItemID =  ItemID.IRON, ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }}
                ],
                LinkedSkill = SkillID.MINING
            };
            
            _updateNodeErrorListener = new UpdateNodeErrorListener();
            _updateNodeResponseListener = new UpdateNodeResponseListener();
            ManagedSubscribe(_updateNodeErrorListener);
            ManagedSubscribe(_updateNodeResponseListener);
        }
        
        private void DispatchNodeCreation(NodeCreation nodeCreation)
        {
            IBuffer<NodeCreation> buffer = BufferManager.RequestBuffer<NodeCreation>(new BufferRequest(1));
            buffer.Assign([nodeCreation]);
            buffer.MarkReady();
        }

        private void DispatchNodeUpdate(HarvestNodeUpdate nodeUpdate)
        {
            IBuffer<HarvestNodeUpdate> buffer = BufferManager.RequestBuffer<HarvestNodeUpdate>(new BufferRequest(1));
            buffer.Assign([nodeUpdate]);
            buffer.MarkReady();
        }
        
        private static void AssertResponseListener(HarvestNodeUpdate nodeUpdate, HarvestNodeUpdateResponse response)
        { 
            Assert.That(response.ItemID, Is.EqualTo(nodeUpdate.ItemID));
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
            DispatchNodeCreation(_nodeCreation);
            Assert.DoesNotThrow(() => DispatchNodeUpdate(_nodeUpdate));
            
            Assert.Multiple(() =>
            {
                Assert.That(_updateNodeErrorListener.WasCalled, Is.EqualTo(false));
                Assert.That(_updateNodeResponseListener.WasCalled, Is.EqualTo(true));
            });
            
            AssertResponseListener(_nodeUpdate, _updateNodeResponseListener.HarvestNodeUpdateResponses[0]);
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
            
            AssertErrorListener<NotFoundException<SkillID>>([_nodeUpdate with { SkillID = SkillID.FORAGING }], _updateNodeErrorListener.HarvestNodeUpdateError);
        } 
        
        [Test]
        public void Negative_SendCommand_SkillDoesNotAllowResource_NoUpdate_DispatchesError()
        {
            DispatchNodeCreation(_nodeCreation);
            
            Assert.DoesNotThrow(() => DispatchNodeUpdate(_nodeUpdate with { ItemID = ItemID.HERBS }));
            
            Assert.Multiple(() =>
            {
                Assert.That(_updateNodeErrorListener.WasCalled, Is.EqualTo(true));
                Assert.That(_updateNodeResponseListener.WasCalled, Is.EqualTo(false));
            });
            
            AssertErrorListener<NotFoundException<ItemID>>([_nodeUpdate with { ItemID = ItemID.HERBS }],  _updateNodeErrorListener.HarvestNodeUpdateError);
        } 
    }
}
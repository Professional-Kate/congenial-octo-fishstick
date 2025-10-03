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
        private SkillUpdateResponse _skillUpdateResponse;
        private NodeCreation _nodeCreation;
        private UpdateNodeErrorListener _updateNodeErrorListener;
        private UpdateNodeResponseListener _updateNodeResponseListener;

        [SetUp]
        public void Setup()
        {
            _skillUpdateResponse = new SkillUpdateResponse
            {
                SkillID = SkillID.MINING, 
                ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, ExperiencePerAction = 0, Level = 0, NextLevelExperience = 0 },
                HasLeveled = false
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

        private void DispatchSkillUpdate(SkillUpdateResponse skillUpdateResponse)
        {
            IBuffer<SkillUpdateResponse> buffer = BufferManager.RequestBuffer<SkillUpdateResponse>(new BufferRequest(1));
            buffer.Assign([skillUpdateResponse]);
            buffer.MarkReady();
        }
        
        private void AssertResponseListener()
        {
        }

        private void AssertErrorListener<TException>(SkillUpdateResponse skillUpdateResponse)
        {
            HarvestNodeUpdateError error = _updateNodeErrorListener.HarvestNodeUpdateError;
            Assert.That(error.BaseError.Exception.InnerException, Is.Not.Null);
            
            Assert.Multiple(() =>
            {
                Assert.That(error.BaseError.Exception.InnerException.GetType(), Is.EqualTo(typeof(TException)));
                Assert.That(error.SkillUpdateResponse.SkillID, Is.EqualTo(skillUpdateResponse.SkillID));
            });
        }

        [Test]
        public void Positive_SendCommand_DispatchesResponse_NoError()
        {
            DispatchNodeCreation(_nodeCreation);
            Assert.DoesNotThrow(() => DispatchSkillUpdate(_skillUpdateResponse));
            
            Assert.Multiple(() =>
            {
                Assert.That(_updateNodeErrorListener.WasCalled, Is.EqualTo(false));
                Assert.That(_updateNodeResponseListener.WasCalled, Is.EqualTo(true));
            });
            AssertResponseListener();
        }

        [Test]
        public void Negative_SendCommand_SkillNotFound_NoUpdate_DispatchesError()
        {
            Assert.DoesNotThrow(() => DispatchSkillUpdate(_skillUpdateResponse with { SkillID = SkillID.FORAGING }));
            
            Assert.Multiple(() =>
            {
                Assert.That(_updateNodeErrorListener.WasCalled, Is.EqualTo(true));
                Assert.That(_updateNodeResponseListener.WasCalled, Is.EqualTo(false));
            });
            AssertErrorListener<NotFoundException<SkillID>>(_skillUpdateResponse with { SkillID = SkillID.FORAGING });
        } 
        
        [Test]
        public void Negative_SendCommand_SkillDoesNotAllowResource_NoUpdate_DispatchesError()
        {
            DispatchNodeCreation(_nodeCreation);
            
            Assert.DoesNotThrow(() => DispatchSkillUpdate(_skillUpdateResponse));
            
            Assert.Multiple(() =>
            {
                Assert.That(_updateNodeErrorListener.WasCalled, Is.EqualTo(true));
                Assert.That(_updateNodeResponseListener.WasCalled, Is.EqualTo(false));
            });
            
            AssertErrorListener<NotFoundException<ItemID>>(_skillUpdateResponse);
        } 
    }
}
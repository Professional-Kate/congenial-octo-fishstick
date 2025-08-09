using System.Diagnostics;
using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Common.Errors;
using IdelPog.Common.Responses;
using IdelPog.Common.Structures;
using IdelPog.Messaging.Buffer;
using IdelPog.Validation.Exceptions;

namespace Integration.Tests.ContentEngine.Update
{
    [TestFixture]
    public class HarvestNodeUpdateTest : ManagedBuffer
    {
        private SkillUpdateResponse _skillUpdateResponse;
        private SetHarvestNode _setHarvestNode;
        private UpdateNodeErrorListener _updateNodeErrorListener;
        private UpdateNodeResponseListener _updateNodeResponseListener;

        [SetUp]
        public void Setup()
        {
            _skillUpdateResponse = new SkillUpdateResponse
            {
                SkillID = SkillID.MINING, 
                LevelProgress = new LevelProgress { Experience = 0, ExperiencePerAction = 0, Level = 0, NextLevelExperience = 0 },
                HasLeveled = false
            };
            
            _setHarvestNode = new SetHarvestNode
            {
                ResourceID = ResourceID.IRON,
                SkillID = SkillID.MINING
            };
            
            _updateNodeErrorListener = new UpdateNodeErrorListener();
            _updateNodeResponseListener = new UpdateNodeResponseListener();
            ManagedSubscribe(_updateNodeErrorListener);
            ManagedSubscribe(_updateNodeResponseListener);
        }

        private void DispatchSkillUpdate(SkillUpdateResponse skillUpdateResponse)
        {
            IBuffer<SkillUpdateResponse> buffer = BufferManager.RequestBuffer<SkillUpdateResponse>(new BufferRequest(1));
            buffer.Assign([skillUpdateResponse]);
            buffer.MarkReady();
        }
        
        private void DispatchSetHarvestNode(SetHarvestNode setHarvestNode)
        {
            IBuffer<SetHarvestNode> buffer = BufferManager.RequestBuffer<SetHarvestNode>(new BufferRequest(1));
            buffer.Assign([setHarvestNode]);
            buffer.MarkReady();
        }

        private void AssertResponseListener()
        {
            HarvestNodeUpdateResponse response = _updateNodeResponseListener.HarvestNodeUpdateResponse;
            Assert.Multiple(() =>
            {
                Assert.That(response.ResourceID, Is.EqualTo(_setHarvestNode.ResourceID));
                Assert.That(response.LevelProgress, Is.Not.EqualTo(_skillUpdateResponse.LevelProgress));
            });
        }

        private void AssertErrorListener<TException>()
        {
            HarvestNodeUpdateError error = _updateNodeErrorListener.HarvestNodeUpdateError;
            Debug.Assert(error.BaseError.Exception.InnerException != null, "error.BaseError.Exception.InnerException != null");
            
            Assert.Multiple(() =>
            {
                Assert.That(error.BaseError.Exception.InnerException.GetType(), Is.EqualTo(typeof(TException)));
            });
        }

        [Test]
        public void Positive_SendCommand_DispatchesResponse_NoError()
        {
            DispatchSetHarvestNode(_setHarvestNode);
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
            DispatchSetHarvestNode(_setHarvestNode with { SkillID = SkillID.FARMING });
            
            Assert.DoesNotThrow(() => DispatchSkillUpdate(_skillUpdateResponse with { SkillID = SkillID.FARMING }));
            
            Assert.Multiple(() =>
            {
                Assert.That(_updateNodeErrorListener.WasCalled, Is.EqualTo(true));
                Assert.That(_updateNodeResponseListener.WasCalled, Is.EqualTo(false));
            });
            AssertErrorListener<NotFoundException<SkillID>>();
        } 
        
        [Test]
        public void Negative_SendCommand_SkillDoesNotAllowResource_NoUpdate_DispatchesError()
        {
            DispatchSetHarvestNode(_setHarvestNode with { ResourceID = ResourceID.COPPER });
            
            Assert.DoesNotThrow(() => DispatchSkillUpdate(_skillUpdateResponse));
            
            Assert.Multiple(() =>
            {
                Assert.That(_updateNodeErrorListener.WasCalled, Is.EqualTo(true));
                Assert.That(_updateNodeResponseListener.WasCalled, Is.EqualTo(false));
            });
            
            AssertErrorListener<NotFoundException<ResourceID>>();
        } 
    }
}
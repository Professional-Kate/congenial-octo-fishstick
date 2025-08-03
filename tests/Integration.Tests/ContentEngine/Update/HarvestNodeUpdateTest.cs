using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Common.Responses;
using IdelPog.Common.Structures;
using IdelPog.Messaging.Buffer;

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

        private void DispatchSkillUpdate()
        {
            IBuffer<SkillUpdateResponse> buffer = BufferManager.RequestBuffer<SkillUpdateResponse>(new BufferRequest(1));
            buffer.Assign([_skillUpdateResponse]);
            buffer.MarkReady();
        }
        
        private void DispatchSetHarvestNode(SetHarvestNode setHarvestNode)
        {
            IBuffer<SetHarvestNode> buffer = BufferManager.RequestBuffer<SetHarvestNode>(new BufferRequest(1));
            buffer.Assign([setHarvestNode]);
            buffer.MarkReady();
        }

        private void AssertResponseListener(bool wasCalled)
        {
            Assert.That(_updateNodeResponseListener.WasCalled, Is.EqualTo(wasCalled));
            HarvestNodeUpdateResponse response = _updateNodeResponseListener.HarvestNodeUpdateResponse;
            Assert.Multiple(() =>
            {
                Assert.That(response.ResourceID, Is.EqualTo(_setHarvestNode.ResourceID));
            });
        }

        private void AssertErrorListener(bool wasCalled)
        {
            Assert.That(_updateNodeErrorListener.WasCalled, Is.EqualTo(wasCalled));
        }

        [Test]
        public void Positive_SendCommand_DispatchesResponse_NoError()
        {
            DispatchSetHarvestNode(_setHarvestNode);
            Assert.DoesNotThrow(DispatchSkillUpdate);
            AssertResponseListener(true);
            AssertErrorListener(false);
        }
    }
}
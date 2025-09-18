using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Exceptions;

namespace IdelPog.Integration.Tests.ContentEngine
{
    [TestFixture]
    public class SetHarvestNodeFlowTest : ManagedTestBuffer
    {
        private SetHarvestNode _setHarvestNode;
        private NodeCreation _nodeCreation;
        private HarvestNodeChangeResponseListener _harvestNodeChangeResponseListener;
        private HarvestNodeErrorListener  _harvestNodeErrorListener;
        
        [SetUp]
        public void Setup()
        {
            _nodeCreation = new NodeCreation
            {
                LinkedSkill = SkillID.MINING,
                ItemIDs = [ItemID.COPPER, ItemID.GOLD, ItemID.IRON, ItemID.STONE]
            };
            
            _setHarvestNode = new SetHarvestNode
            {
                ItemID = ItemID.IRON,
                SkillID = SkillID.MINING
            };
            
            _harvestNodeChangeResponseListener = new HarvestNodeChangeResponseListener();
            _harvestNodeErrorListener = new  HarvestNodeErrorListener();
            ManagedSubscribe(_harvestNodeChangeResponseListener);
            ManagedSubscribe(_harvestNodeErrorListener);
        }
        
        private void DispatchNodeCreation(NodeCreation nodeCreation)
        {
            IBuffer<NodeCreation> buffer = BufferManager.RequestBuffer<NodeCreation>(new BufferRequest(1));
            buffer.Assign([nodeCreation]);
            buffer.MarkReady();
        }

        private void DispatchSetHarvestNode(SetHarvestNode setHarvestNode)
        {
            IBuffer<SetHarvestNode> buffer = BufferManager.RequestBuffer<SetHarvestNode>(new BufferRequest(1));
            buffer.Assign([setHarvestNode]);
            buffer.MarkReady();
        }

        private void AssertListenerWasCalled(SetHarvestNode setHarvestNode)
        {
            Assert.That(_harvestNodeChangeResponseListener.WasCalled, Is.True);
            SetHarvestNodeResponse setHarvestNodeResponse = _harvestNodeChangeResponseListener.SetHarvestNodeResponse;
            
            Assert.That(setHarvestNodeResponse.SetHarvestNode, Is.EqualTo(setHarvestNode));
        }

        private void AssertErrorListenerWasCalled(SetHarvestNode expected, Type expectedExceptionType)
        {
            Assert.That(_harvestNodeErrorListener.WasCalled, Is.True);
            SetHarvestNode result = _harvestNodeErrorListener.SetHarvestNodeError.SetHarvestNode;
            Assert.Multiple(() =>
            {
                Assert.That(result.ItemID, Is.EqualTo(expected.ItemID));
                Assert.That(result.SkillID, Is.EqualTo(expected.SkillID));
            });
            
            BaseError baseError = _harvestNodeErrorListener.SetHarvestNodeError.BaseError;
            Assert.That(baseError.Exception.GetType(), Is.EqualTo(expectedExceptionType));
        }
        
        private void AssertListenerWasNotCalled()
        {
            Assert.That(_harvestNodeChangeResponseListener.WasCalled, Is.False);
        }

        [Test]
        public void Positive_SendCommand_SetsCurrentHarvestNode_NoThrow()
        {
            DispatchNodeCreation(_nodeCreation);
            Assert.DoesNotThrow(() => DispatchSetHarvestNode(_setHarvestNode));
            
            AssertListenerWasCalled(_setHarvestNode);
        }

        [Test]
        public void Positive_SendSameCommandMultipleTimes_SendsSameResponse()
        {
            DispatchNodeCreation(_nodeCreation);
            
            const int times = 5;
            for (int i = 0; i < times; i++)
            {
                Assert.DoesNotThrow(() => DispatchSetHarvestNode(_setHarvestNode));
                AssertListenerWasCalled(_setHarvestNode);
            }
        }

        [Test]
        public void Negative_SendMissingSkillID_SendsError()
        {
            SetHarvestNode missingSkill = new() { ItemID = ItemID.GOLD, SkillID = SkillID.WOOD_CUTTING };
            Assert.DoesNotThrow(() => DispatchSetHarvestNode(missingSkill));
            AssertListenerWasNotCalled();
            AssertErrorListenerWasCalled(missingSkill, typeof(ControllerThrownException));
        } 
        
        [Test]
        public void Negative_SendMissingResourceID_SendsError()
        {
            SetHarvestNode missingResourceCommand = new() { ItemID = ItemID.GOLD, SkillID = SkillID.MINING };
            Assert.DoesNotThrow(() => DispatchSetHarvestNode(missingResourceCommand));
            AssertListenerWasNotCalled();
            AssertErrorListenerWasCalled(missingResourceCommand, typeof(ControllerThrownException));
        } 
    }
}
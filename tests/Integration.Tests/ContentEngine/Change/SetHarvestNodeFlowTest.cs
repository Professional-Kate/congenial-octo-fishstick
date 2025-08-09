using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Common.Errors;
using IdelPog.Common.Responses;
using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Exceptions;

namespace Integration.Tests.ContentEngine
{
    [TestFixture]
    public class SetHarvestNodeFlowTest : ManagedBuffer
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
                ResourceIDs = [ResourceID.COPPER, ResourceID.GOLD, ResourceID.IRON, ResourceID.STONE]
            };
            
            _setHarvestNode = new SetHarvestNode
            {
                ResourceID = ResourceID.IRON,
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
                Assert.That(result.ResourceID, Is.EqualTo(expected.ResourceID));
                Assert.That(result.SkillID, Is.EqualTo(expected.SkillID));
            });
            
            BaseError baseError = _harvestNodeErrorListener.SetHarvestNodeError.BaseError;
            Assert.That(baseError.Exception.GetType(), Is.EqualTo(expectedExceptionType));
        }
        
        private void AssertListenerWasNotCalled()
        {
            Assert.That(_harvestNodeChangeResponseListener.WasCalled, Is.False);
        }

        private void AssertCurrentResourceProvider_Equals(ResourceID expected)
        {
            Assert.That(CurrentResourceProvider.GetCurrentResource(), Is.EqualTo(expected));
        }

        private void AssertCurrencyResourceProvider_DoesNotEqual(ResourceID expected)
        {
            Assert.That(CurrentResourceProvider.GetCurrentResource(), Is.Not.EqualTo(expected));
        }
        
        [Test]
        public void Positive_SendCommand_SetsCurrentHarvestNode_NoThrow()
        {
            DispatchNodeCreation(_nodeCreation);
            Assert.DoesNotThrow(() => DispatchSetHarvestNode(_setHarvestNode));
            
            AssertListenerWasCalled(_setHarvestNode);
            AssertCurrentResourceProvider_Equals(_setHarvestNode.ResourceID);
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
                AssertCurrentResourceProvider_Equals(_setHarvestNode.ResourceID);
            }
        }

        [Test]
        public void Negative_SendMissingSkillID_SendsError()
        {
            SetHarvestNode missingSkill = new() { ResourceID = ResourceID.GOLD, SkillID = SkillID.WOOD_CUTTING };
            Assert.DoesNotThrow(() => DispatchSetHarvestNode(missingSkill));
            AssertListenerWasNotCalled();
            AssertCurrencyResourceProvider_DoesNotEqual(ResourceID.GOLD);
            AssertErrorListenerWasCalled(missingSkill, typeof(ControllerThrownException));
        } 
        
        [Test]
        public void Negative_SendMissingResourceID_SendsError()
        {
            SetHarvestNode missingResourceCommand = new() { ResourceID = ResourceID.GOLD, SkillID = SkillID.MINING };
            Assert.DoesNotThrow(() => DispatchSetHarvestNode(missingResourceCommand));
            AssertListenerWasNotCalled();
            AssertCurrencyResourceProvider_DoesNotEqual(ResourceID.GOLD);
            AssertErrorListenerWasCalled(missingResourceCommand, typeof(ControllerThrownException));
        } 
    }
}
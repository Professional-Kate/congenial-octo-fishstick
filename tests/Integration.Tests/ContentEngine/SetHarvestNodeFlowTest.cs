using ContentEngine;
using ContentEngine.Services;
using IdelPog.Common.Commands;
using IdelPog.Common.DTO;
using IdelPog.Common.Enums;
using IdelPog.Messaging.Buffer;

namespace Integration.Tests.ContentEngine
{
    [TestFixture]
    public class SetHarvestNodeFlowTest : ManagedBuffer
    {
        private SetHarvestNode _setHarvestNode;
        private HarvestNodeChangeDTOListener _harvestNodeChangeDTOListener;
        private ICurrentResourceSetter _currentResourceSetter;
        private ICurrentResourceProvider _currentResourceProvider;
        
        [SetUp]
        public void Setup()
        {
            CurrentResourceProvider currentResourceProvider = new();
            _currentResourceSetter = currentResourceProvider;
            _currentResourceProvider = currentResourceProvider;
            
            new EngineBootstrapper().Initialize(BufferMessenger, BufferManager, _currentResourceSetter);


            _setHarvestNode = new SetHarvestNode
            {
                ResourceID = ResourceID.STONE,
                SkillID = SkillID.MINING
            };
            
            _harvestNodeChangeDTOListener = new HarvestNodeChangeDTOListener();
            ManagedSubscribe(_harvestNodeChangeDTOListener);
        }

        private void DispatchSetHarvestNode(SetHarvestNode setHarvestNode)
        {
            IBuffer<SetHarvestNode> buffer = BufferManager.RequestBuffer<SetHarvestNode>(new BufferRequest(1));
            buffer.Assign([setHarvestNode]);
            buffer.MarkReady();
        }

        private void AssertListenerWasCalled(SetHarvestNode setHarvestNode)
        {
            Assert.That(_harvestNodeChangeDTOListener.WasCalled, Is.True);
            ResourceChangeDTO resourceChangeDTO = _harvestNodeChangeDTOListener.ResourceChangeDTO;
            
            Assert.That(resourceChangeDTO.ResourceID, Is.EqualTo(setHarvestNode.ResourceID));
        }
        
        private void AssertListenerWasNotCalled()
        {
            Assert.That(_harvestNodeChangeDTOListener.WasCalled, Is.False);
        }

        private void AssertCurrentResourceProvider_Equals(ResourceID expected)
        {
            Assert.That(_currentResourceProvider.GetCurrentResource(), Is.EqualTo(expected));
        }

        private void AssertCurrencyResourceProvider_DoesNotEqual(ResourceID expected)
        {
            Assert.That(_currentResourceProvider.GetCurrentResource(), Is.Not.EqualTo(expected));
        }
        
        [Test]
        public void Positive_SendCommand_SetsCurrentHarvestNode_NoThrow()
        {
            Assert.DoesNotThrow(() => DispatchSetHarvestNode(_setHarvestNode));
            
            AssertListenerWasCalled(_setHarvestNode);
            AssertCurrentResourceProvider_Equals(_setHarvestNode.ResourceID);
        }

        [Test]
        public void Positive_SendSameCommandMultipleTimes_SendsSameDTO()
        {
            const int times = 5;
            for (int i = 0; i < times; i++)
            {
                Assert.DoesNotThrow(() => DispatchSetHarvestNode(_setHarvestNode));
                AssertListenerWasCalled(_setHarvestNode);
                AssertCurrentResourceProvider_Equals(_setHarvestNode.ResourceID);
            }
        }

        [Test]
        public void Negative_SendMissingSkillID_SendsErrorDTO()
        {
            Assert.DoesNotThrow(() => DispatchSetHarvestNode(new SetHarvestNode { ResourceID = ResourceID.GOLD, SkillID = SkillID.WOOD_CUTTING }));
            AssertListenerWasNotCalled();
            AssertCurrencyResourceProvider_DoesNotEqual(ResourceID.GOLD);
            
            // TODO: add tests for the ErrorDTO listener
        } 
        
        [Test]
        public void Negative_SendMissingResourceID_SendsErrorDTO()
        {
            Assert.DoesNotThrow(() => DispatchSetHarvestNode(new SetHarvestNode { ResourceID = ResourceID.GOLD, SkillID = SkillID.MINING }));
            AssertListenerWasNotCalled();
            AssertCurrencyResourceProvider_DoesNotEqual(ResourceID.GOLD);
            
            // TODO: add tests for the ErrorDTO listener
        } 
    }
}
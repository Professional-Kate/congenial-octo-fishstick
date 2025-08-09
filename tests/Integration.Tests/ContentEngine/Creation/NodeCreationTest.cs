using System.Diagnostics;
using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Common.Errors;
using IdelPog.Common.Responses;
using IdelPog.Messaging.Buffer;
using IdelPog.Validation.Exceptions;

namespace Integration.Tests.ContentEngine
{
    [TestFixture]
    public class NodeCreationTest : ManagedBuffer
    {
        private NodeCreation _nodeCreation;
        private NodeCreationResponseListener _nodeCreationResponseListener;
        private NodeCreationErrorListener _nodeCreationErrorListener;

        [SetUp]
        public void Setup()
        {
            _nodeCreation = new NodeCreation
            {
                LinkedSkill = SkillID.MINING,
                ResourceIDs = [ResourceID.COPPER, ResourceID.GOLD, ResourceID.IRON, ResourceID.STONE]
            };
            
            _nodeCreationResponseListener = new NodeCreationResponseListener();
            _nodeCreationErrorListener = new NodeCreationErrorListener();
            ManagedSubscribe(_nodeCreationResponseListener);
            ManagedSubscribe(_nodeCreationErrorListener);
        }
        
        private void DispatchNodeCreation(NodeCreation nodeCreation)
        {
            IBuffer<NodeCreation> buffer = BufferManager.RequestBuffer<NodeCreation>(new BufferRequest(1));
            buffer.Assign([nodeCreation]);
            buffer.MarkReady();
        }

        private void AssertResponseListener(NodeCreation nodeCreation)
        {
            Assert.Multiple(() =>
            {
                NodeCreationResponse response = _nodeCreationResponseListener.NodeCreationResponse;
                Assert.That(response.NodeCreations.Count, Is.EqualTo(1));
                Assert.That(response.NodeCreations[0], Is.EqualTo(nodeCreation));
            });
        }

        private void AssertErrorListener<TException>(NodeCreation nodeCreation)
        {
            
            Assert.Multiple(() =>
            {
                NodeCreationError error = _nodeCreationErrorListener.NodeCreationError;
                Debug.Assert(error.BaseError.Exception.InnerException != null, "error.BaseError.Exception.InnerException != null");
            
                Assert.That(error.BaseError.Exception.InnerException.GetType(), Is.EqualTo(typeof(TException)));
                Assert.That(error.NodeCreations, Has.Length.EqualTo(1));
                Assert.That(error.NodeCreations[0], Is.EqualTo(nodeCreation));
            });
        }

        [Test]
        public void Positive_SendCommand_CreatesEachNode_DispatchesResponse()
        {
            Assert.DoesNotThrow(() => DispatchNodeCreation(_nodeCreation));
            
            Assert.Multiple(() =>
            {
                Assert.That(_nodeCreationResponseListener.WasCalled, Is.True);
                Assert.That(_nodeCreationErrorListener.WasCalled, Is.False);
            });
            AssertResponseListener(_nodeCreation);
        }

        [Test]
        public void Negative_SendCommand_DuplicateSkillID_NoUpdate_DispatchesError()
        {
            DispatchNodeCreation(_nodeCreation);
            NodeCreation duplicateNodeCreation = _nodeCreation with { ResourceIDs = [ResourceID.STONE] };
            Assert.DoesNotThrow(() => DispatchNodeCreation(duplicateNodeCreation));
            
            Assert.Multiple(() =>
            {
                Assert.That(_nodeCreationResponseListener.WasCalled, Is.True);
                Assert.That(_nodeCreationErrorListener.WasCalled, Is.True);
            });
            
            AssertResponseListener(_nodeCreation);
            AssertErrorListener<DuplicateEntityException>(duplicateNodeCreation);
        }

        [Test]
        public void Negative_SendCommand_EmptyResourceIDs_NoUpdate_DispatchesError()
        {
            NodeCreation emptyArrayCreation = _nodeCreation with { ResourceIDs = [] };
            Assert.DoesNotThrow(() => DispatchNodeCreation(emptyArrayCreation));
            
            Assert.Multiple(() =>
            {
                Assert.That(_nodeCreationResponseListener.WasCalled, Is.False);
                Assert.That(_nodeCreationErrorListener.WasCalled, Is.True);
            });
            AssertErrorListener<EmptyCollectionException>(emptyArrayCreation);
        }

        [Test]
        public void Negative_SendCommand_DuplicateResource_NoUpdate_DispatchesError()
        {
            DispatchNodeCreation(_nodeCreation);
            
            NodeCreation duplicateResourceCreation = new() { LinkedSkill = SkillID.FARMING, ResourceIDs = [ResourceID.IRON] };
            Assert.DoesNotThrow(() => DispatchNodeCreation(duplicateResourceCreation));
            
            Assert.Multiple(() =>
            {
                Assert.That(_nodeCreationResponseListener.WasCalled, Is.True);
                Assert.That(_nodeCreationErrorListener.WasCalled, Is.True);
            });
            AssertResponseListener(_nodeCreation);
            AssertErrorListener<DuplicateEntityException>(duplicateResourceCreation);
        }
    }
}
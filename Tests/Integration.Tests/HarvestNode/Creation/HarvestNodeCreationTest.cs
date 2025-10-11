using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Progression;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.HarvestNode.Contracts;
using IdelPog.HarvestNode.Contracts.Command;
using IdelPog.HarvestNode.Contracts.Error;
using IdelPog.HarvestNode.Contracts.Response;

namespace IdelPog.Integration.Tests.HarvestNode
{
    [TestFixture]
    public sealed class HarvestNodeCreationTest : ManagedTestBuffer
    {
        private HarvestNodeCreation _miningCreation;
        
        private NodeCreationResponseListener _nodeCreationResponseListener;
        private NodeCreationErrorListener _nodeCreationErrorListener;

        [SetUp]
        public void Setup()
        {
            _miningCreation = new HarvestNodeCreation
            {
                ReadOnlyHarvestNodes =
                [
                    new ReadOnlyHarvestNode { ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }, ResourceID = ResourceID.STONE, LocationID = LocationID.CAVE},
                    new ReadOnlyHarvestNode { ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }, ResourceID = ResourceID.COPPER_CLUSTER, LocationID = LocationID.CAVE},
                    new ReadOnlyHarvestNode { ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }, ResourceID = ResourceID.GOLD_CLUSTER, LocationID = LocationID.CAVE},
                    new ReadOnlyHarvestNode { ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }, ResourceID = ResourceID.IRON_CLUSTER, LocationID = LocationID.CAVE}
                ],
                LinkedSkill = SkillID.MINING
            };

            _nodeCreationResponseListener = new NodeCreationResponseListener();
            _nodeCreationErrorListener = new NodeCreationErrorListener();
            ManagedSubscribe(_nodeCreationResponseListener);
            ManagedSubscribe(_nodeCreationErrorListener);
        }
        
        private void DispatchNodeCreation(params HarvestNodeCreation[] nodeCreations)
        {
            IBuffer<HarvestNodeCreation> buffer = BufferManager.RequestBuffer<HarvestNodeCreation>(new BufferRequest(nodeCreations.Length));
            buffer.Assign(nodeCreations);
            buffer.MarkReady();
        }

        private void AssertResponseListenerCalled(bool wasCalled)
        {
            Assert.That(_nodeCreationResponseListener.WasCalled, Is.EqualTo(wasCalled));
        }
        
        private void AssertResponseLength(int length)
        {
            Assert.That(_nodeCreationResponseListener.HarvestNodeCreationResponses, Has.Length.EqualTo(length));
        }

        private static void AssertResponseListener(HarvestNodeCreationResponse response, HarvestNodeCreation nodeCreation)
        {
            Assert.Multiple(() =>
            {
                Assert.That(response.LinkedSkill, Is.EqualTo(nodeCreation.LinkedSkill));
                Assert.That(response.ReadOnlyHarvestNodes, Is.EqualTo(nodeCreation.ReadOnlyHarvestNodes));
            });
        }
        
        private void AssertErrorListenerCalled(bool wasCalled)
        {
            Assert.That(_nodeCreationErrorListener.WasCalled, Is.EqualTo(wasCalled));
        }
        
        private void AssertErrorLength(int length)
        {
            Assert.That(_nodeCreationErrorListener.HarvestNodeCreationError.NodeCreations, Has.Length.EqualTo(length));
        }

        private void AssertErrorListener<TException>(params HarvestNodeCreation[] harvestNodeCreations)
        {
            
            Assert.Multiple(() =>
            {
                HarvestNodeCreationError error = _nodeCreationErrorListener.HarvestNodeCreationError;
                Assert.That(error.BaseError.Exception.InnerException, Is.Not.Null);
                Assert.That(error.BaseError.Exception.InnerException!.GetType(), Is.EqualTo(typeof(TException)));
                Assert.That(error.NodeCreations, Is.EqualTo(harvestNodeCreations));
            });
        }

        [Test]
        public void Positive_SendCommand_CreatesEachNode_DispatchesResponse()
        {
            Assert.DoesNotThrow(() => DispatchNodeCreation(_miningCreation));

            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponseListener(_nodeCreationResponseListener.HarvestNodeCreationResponses[0], _miningCreation);
        }
        
        [Test]
        public void Positive_SendMultipleCommands_CreatesEachNode_DispatchesResponse()
        {
            HarvestNodeCreation foragingCreation = new()
            {
                ReadOnlyHarvestNodes =
                [
                    new ReadOnlyHarvestNode { ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }, ResourceID = ResourceID.SMALL_PLANTS, LocationID = LocationID.PLAINS}
                ],
                LinkedSkill = SkillID.FORAGING
            };
            
            Assert.DoesNotThrow(() => DispatchNodeCreation(_miningCreation, foragingCreation));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(2);
            AssertResponseListener(_nodeCreationResponseListener.HarvestNodeCreationResponses[0], _miningCreation);
            AssertResponseListener(_nodeCreationResponseListener.HarvestNodeCreationResponses[1], foragingCreation);
        }
        
        [Test]
        public void Negative_SendCommand_DuplicateSkillID_DispatchesError()
        {
            HarvestNodeCreation miningCreation = new()
            {
                ReadOnlyHarvestNodes =
                [
                    new ReadOnlyHarvestNode { ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }, ResourceID = ResourceID.SMALL_PLANTS, LocationID = LocationID.PLAINS}
                ],
                LinkedSkill = SkillID.MINING
            };
            
            Assert.DoesNotThrow(() => DispatchNodeCreation(_miningCreation, miningCreation));

            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(2);
            AssertErrorListener<DuplicateEntityException>(_miningCreation, miningCreation);
        }

        [Test]
        public void Negative_SendCommand_EmptyResourceIDs_NoUpdate_DispatchesError()
        {
            HarvestNodeCreation emptyArrayCreation = _miningCreation with { ReadOnlyHarvestNodes = [] };
            Assert.DoesNotThrow(() => DispatchNodeCreation(emptyArrayCreation));
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorListener<EmptyCollectionException>(emptyArrayCreation);
        }

        [Test]
        public void Negative_SendCommand_DuplicateResource_OnlyOneUpdate_SecondCallDispatchesError()
        {
            HarvestNodeCreation duplicateResourceCreation = new()
            {
                ReadOnlyHarvestNodes =
                [
                    new ReadOnlyHarvestNode { ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }, ResourceID = ResourceID.IRON_CLUSTER, LocationID = LocationID.CAVE}
                ],
                LinkedSkill = SkillID.FORAGING
            };
            
            Assert.DoesNotThrow(() => DispatchNodeCreation(_miningCreation, duplicateResourceCreation));

            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(2);
            AssertErrorListener<DuplicateEntityException>(_miningCreation, duplicateResourceCreation);
        }
        
        [Test]
        public void Negative_SendCommand_DuplicateResourceInCommand_NoUpdate_DispatcherError()
        {
            HarvestNodeCreation duplicateCreation = new()
            {
                ReadOnlyHarvestNodes =
                [
                    new ReadOnlyHarvestNode { ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }, ResourceID = ResourceID.STONE, LocationID = LocationID.CAVE},
                    new ReadOnlyHarvestNode { ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }, ResourceID = ResourceID.STONE, LocationID = LocationID.CAVE}
                ],
                LinkedSkill = SkillID.MINING
            };
            
            Assert.DoesNotThrow(() => DispatchNodeCreation(duplicateCreation));
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertErrorListener<DuplicateEntityException>(duplicateCreation);
        }
    }
}
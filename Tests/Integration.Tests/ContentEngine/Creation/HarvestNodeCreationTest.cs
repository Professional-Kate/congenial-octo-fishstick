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
    public class HarvestNodeCreationTest : ManagedTestBuffer
    {
        private HarvestNodeCreation _harvestNodeCreation;
        private NodeCreationResponseListener _nodeCreationResponseListener;
        private NodeCreationErrorListener _nodeCreationErrorListener;

        [SetUp]
        public void Setup()
        {
            _harvestNodeCreation = new HarvestNodeCreation
            {
                ReadOnlyHarvestNodes =
                [
                    new ReadOnlyHarvestNode { ItemID =  ItemID.STONE, ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }},
                    new ReadOnlyHarvestNode { ItemID =  ItemID.COPPER, ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }},
                    new ReadOnlyHarvestNode { ItemID =  ItemID.GOLD, ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }},
                    new ReadOnlyHarvestNode { ItemID =  ItemID.IRON, ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }}
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

        private void AssertResponseListener(params HarvestNodeCreation[] nodeCreations)
        {
            Assert.Multiple(() =>
            {
                HarvestNodeCreationResponse response = _nodeCreationResponseListener.HarvestNodeCreationResponse;
                Assert.That(response.NodeCreations.Count, Is.EqualTo(nodeCreations.Length));
                Assert.That(response.NodeCreations, Is.EqualTo(nodeCreations));
            });
        }

        private void AssertErrorListener<TException>(HarvestNodeCreation harvestNodeCreation)
        {
            
            Assert.Multiple(() =>
            {
                HarvestNodeCreationError error = _nodeCreationErrorListener.HarvestNodeCreationError;
                Assert.That(error.BaseError.Exception.InnerException, Is.Not.Null);
            
                Assert.That(error.BaseError.Exception.InnerException!.GetType(), Is.EqualTo(typeof(TException)));
                Assert.That(error.NodeCreations, Has.Length.EqualTo(1));
                Assert.That(error.NodeCreations[0], Is.EqualTo(harvestNodeCreation));
            });
        }

        [Test]
        public void Positive_SendCommand_CreatesEachNode_DispatchesResponse()
        {
            Assert.DoesNotThrow(() => DispatchNodeCreation(_harvestNodeCreation));
            
            Assert.Multiple(() =>
            {
                Assert.That(_nodeCreationResponseListener.WasCalled, Is.True);
                Assert.That(_nodeCreationErrorListener.WasCalled, Is.False);
            });
            AssertResponseListener(_harvestNodeCreation);
        }
        
        [Test]
        public void Positive_SendMultipleCommands_CreatesEachNode_DispatchesResponse()
        {
            HarvestNodeCreation stoneCreation = new()
            {
                ReadOnlyHarvestNodes =
                [
                    new ReadOnlyHarvestNode { ItemID =  ItemID.STONE, ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }}
                ],
                LinkedSkill = SkillID.MINING
            };
            HarvestNodeCreation copperCreation = new()
            {
                ReadOnlyHarvestNodes =
                [
                    new ReadOnlyHarvestNode { ItemID =  ItemID.COPPER, ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }}
                ],
                LinkedSkill = SkillID.FORAGING
            };
            
            Assert.DoesNotThrow(() => DispatchNodeCreation(stoneCreation, copperCreation));
            
            Assert.Multiple(() =>
            {
                Assert.That(_nodeCreationResponseListener.WasCalled, Is.True);
                Assert.That(_nodeCreationErrorListener.WasCalled, Is.False);
            });
            AssertResponseListener(stoneCreation, copperCreation);
        }
        
        [Test]
        public void Negative_SendCommand_DuplicateSkillID_OnlyOneUpdate_SecondCallDispatchesError()
        {
            DispatchNodeCreation(_harvestNodeCreation);
            HarvestNodeCreation duplicateHarvestNodeCreation = new()
            {
                ReadOnlyHarvestNodes =
                [
                    new ReadOnlyHarvestNode { ItemID =  ItemID.STONE, ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }}
                ],
                LinkedSkill = SkillID.MINING
            };
            
            Assert.DoesNotThrow(() => DispatchNodeCreation(duplicateHarvestNodeCreation));
            
            Assert.Multiple(() =>
            {
                Assert.That(_nodeCreationResponseListener.WasCalled, Is.True);
                Assert.That(_nodeCreationErrorListener.WasCalled, Is.True);
            });
            
            AssertResponseListener(_harvestNodeCreation);
            AssertErrorListener<DuplicateEntityException>(duplicateHarvestNodeCreation);
        }

        [Test]
        public void Negative_SendCommand_EmptyResourceIDs_NoUpdate_DispatchesError()
        {
            HarvestNodeCreation emptyArrayCreation = _harvestNodeCreation with { ReadOnlyHarvestNodes = [] };
            Assert.DoesNotThrow(() => DispatchNodeCreation(emptyArrayCreation));
            
            Assert.Multiple(() =>
            {
                Assert.That(_nodeCreationResponseListener.WasCalled, Is.False);
                Assert.That(_nodeCreationErrorListener.WasCalled, Is.True);
            });
            AssertErrorListener<EmptyCollectionException>(emptyArrayCreation);
        }

        [Test]
        public void Negative_SendCommand_DuplicateResource_OnlyOneUpdate_SecondCallDispatchesError()
        {
            DispatchNodeCreation(_harvestNodeCreation);
            
            HarvestNodeCreation duplicateResourceCreation = new()
            {
                ReadOnlyHarvestNodes =
                [
                    new ReadOnlyHarvestNode { ItemID =  ItemID.IRON, ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }}
                ],
                LinkedSkill = SkillID.FORAGING
            };
            
            Assert.DoesNotThrow(() => DispatchNodeCreation(duplicateResourceCreation));
            
            Assert.Multiple(() =>
            {
                Assert.That(_nodeCreationResponseListener.WasCalled, Is.True);
                Assert.That(_nodeCreationErrorListener.WasCalled, Is.True);
            });
            AssertResponseListener(_harvestNodeCreation);
            AssertErrorListener<DuplicateEntityException>(duplicateResourceCreation);
            
            Assert.DoesNotThrow(() => DispatchNodeCreation(duplicateResourceCreation));
            
            Assert.Multiple(() =>
            {
                Assert.That(_nodeCreationResponseListener.WasCalled, Is.True);
                Assert.That(_nodeCreationErrorListener.WasCalled, Is.True);
            });
            AssertResponseListener(_harvestNodeCreation);
            AssertErrorListener<DuplicateEntityException>(duplicateResourceCreation);
        }
        
        [Test]
        public void Negative_SendCommand_DuplicateResourceInCommand_NoUpdate_DispatcherError()
        {
            HarvestNodeCreation duplicateCreation = new()
            {
                ReadOnlyHarvestNodes =
                [
                    new ReadOnlyHarvestNode { ItemID =  ItemID.STONE, ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }},
                    new ReadOnlyHarvestNode { ItemID =  ItemID.STONE, ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }}
                ],
                LinkedSkill = SkillID.MINING
            };
            
            Assert.DoesNotThrow(() => DispatchNodeCreation(duplicateCreation));
            
            Assert.Multiple(() =>
            {
                Assert.That(_nodeCreationResponseListener.WasCalled, Is.False);
                Assert.That(_nodeCreationErrorListener.WasCalled, Is.True);
            });
            AssertErrorListener<DuplicateEntityException>(duplicateCreation);
        }
    }
}
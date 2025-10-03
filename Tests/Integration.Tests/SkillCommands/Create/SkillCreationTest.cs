using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Information.Contracts;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Progression;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Integration.Tests.SkillCommands.Create
{
    [TestFixture]
    public sealed class SkillCreationTest : ManagedTestBuffer
    {
        private SkillCreationErrorListener _skillCreationErrorListener;
        private SkillCreationResponseListener _skillCreationResponseListener;

        private SkillCreation _miningCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _miningCreation = new SkillCreation
            {
                Information = new Information { Name = "", Description = "" },
                ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, ExperiencePerAction = 0, Level = 0, NextLevelExperience = 0 },
                SkillID = SkillID.MINING
            };
        }

        [SetUp]
        public void Setup()
        {
            _skillCreationErrorListener = new SkillCreationErrorListener();
            _skillCreationResponseListener = new SkillCreationResponseListener();
            
            ManagedSubscribe(_skillCreationErrorListener);
            ManagedSubscribe(_skillCreationResponseListener);
        }

        private void SendSkillCreationBuffer(SkillCreation[] skillCreations)
        {
            IBuffer<SkillCreation> buffer = BufferManager.RequestBuffer<SkillCreation>(new BufferRequest(skillCreations.Length));
            buffer.Assign(skillCreations);
            buffer.MarkReady();
        }

        private void AssertResponseListenerCalled(bool called)
        {
            Assert.That(_skillCreationResponseListener.WasCalled, Is.EqualTo(called));
        }

        private void AssertResponseLength(int length)
        {
            Assert.That(_skillCreationResponseListener.SkillCreationResponses, Has.Count.EqualTo(length));
        }
        
        private void AssertErrorListenerCalled(bool called)
        {
            Assert.That(_skillCreationErrorListener.WasCalled, Is.EqualTo(called));
        }

        private static void AssertResponse(SkillCreation skillCreation, SkillCreationResponse response)
        {
            Assert.Multiple(() =>
            {
                Assert.That(response.SkillID, Is.EqualTo(skillCreation.SkillID));
                Assert.That(response.Information, Is.EqualTo(skillCreation.Information));
                Assert.That(response.ReadOnlyLevelable, Is.EqualTo(skillCreation.ReadOnlyLevelable));
            });
        }
        
        private void AssertErrorLength(int length)
        {
            Assert.That(_skillCreationErrorListener.SkillCreationError.SkillCreations, Has.Length.EqualTo(length));
        }

        private static void AssertError(Type exception, SkillCreation[] skillCreations, SkillCreationError error)
        {
            Assert.Multiple(() =>
            {
                Assert.That(skillCreations, Is.EqualTo(error.SkillCreations));
                Assert.That(error.BaseError.Exception, Is.TypeOf(typeof(ControllerThrownException)));
                Assert.That(error.BaseError.Exception.InnerException, Is.TypeOf(exception));
            });
        }
        
 
        [Test]
        public void Positive_SendSingleCommand_CreatesSkill_DispatchesResponse()
        {
            Assert.DoesNotThrow(() => SendSkillCreationBuffer([_miningCreation]));

            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_miningCreation, _skillCreationResponseListener.SkillCreationResponses[0]);
        }

        [Test]
        public void Positive_SendMultipleCommands_DispatchesMultiple()
        {
            Assert.DoesNotThrow(() => SendSkillCreationBuffer([_miningCreation, _miningCreation with { SkillID = SkillID.FORAGING }]));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(2);
            AssertResponse(_miningCreation, _skillCreationResponseListener.SkillCreationResponses[0]);
            AssertResponse(_miningCreation with { SkillID = SkillID.FORAGING }, _skillCreationResponseListener.SkillCreationResponses[1]);
        }

        [Test]
        public void Negative_SendMultipleCommands_SameSkillID_Throws()
        {
            Assert.DoesNotThrow(() => SendSkillCreationBuffer([_miningCreation, _miningCreation]));
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(2);
            AssertError(typeof(DuplicateEntityException),[_miningCreation, _miningCreation], _skillCreationErrorListener.SkillCreationError);
        }

        [Test]
        public void Negative_SendSingleCommand_OveMaxLevel_Throws()
        {
            SkillCreation creation = _miningCreation with
            {
                ReadOnlyLevelable = _miningCreation.ReadOnlyLevelable with { Level = LevelConstants.MAX_LEVEL + 1 }
            };
            
            Assert.DoesNotThrow(() => SendSkillCreationBuffer([creation]));
         
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError(typeof(MaxLevelException),[creation], _skillCreationErrorListener.SkillCreationError);
        }
    }
}
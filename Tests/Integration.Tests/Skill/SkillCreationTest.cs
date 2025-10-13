using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Progression;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Skill.Contracts.Command;
using IdelPog.Skill.Contracts.Error;
using IdelPog.Skill.Contracts.Response;

namespace IdelPog.Integration.Tests.Skill
{
    [TestFixture]
    public sealed class SkillCreationTest : ManagedTestBuffer
    {
        private ManagedResponseListener<SkillCreationResponse> _skillCreationResponseListener;
        private ManagedErrorListener<SkillCreationError> _skillCreationErrorListener;
        private SkillCreationDispatcher _dispatcher;

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
            _dispatcher = new SkillCreationDispatcher();
            _skillCreationResponseListener = new ManagedResponseListener<SkillCreationResponse>();
            _skillCreationErrorListener = new ManagedErrorListener<SkillCreationError>();

            _miningCreation = _dispatcher.MiningCreation;
            
            ManagedSubscribe(_skillCreationErrorListener);
            ManagedSubscribe(_skillCreationResponseListener);
        }

        private void DispatchSkillCreation(params SkillCreation[] skillCreations)
        {
            SkillCreationDispatcher.SendSkillCreationBuffer(skillCreations, BufferManager);
        }

        private void AssertResponseListenerCalled(bool called)
        {
            Assert.That(_skillCreationResponseListener.WasCalled, Is.EqualTo(called));
        }

        private void AssertResponseLength(int length)
        {
            Assert.That(_skillCreationResponseListener.Responses, Has.Length.EqualTo(length));
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
            Assert.That(_skillCreationErrorListener.Error.SkillCreations, Has.Length.EqualTo(length));
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
            Assert.DoesNotThrow(() => DispatchSkillCreation(_miningCreation));

            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_miningCreation, _skillCreationResponseListener.Responses[0]);
        }

        [Test]
        public void Positive_SendMultipleCommands_DispatchesMultiple()
        {
            Assert.DoesNotThrow(() => DispatchSkillCreation(_miningCreation, _miningCreation with { SkillID = SkillID.FORAGING }));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(2);
            AssertResponse(_miningCreation, _skillCreationResponseListener.Responses[0]);
            AssertResponse(_miningCreation with { SkillID = SkillID.FORAGING }, _skillCreationResponseListener.Responses[1]);
        }

        [Test]
        public void Positive_SendSingleCommand_AtMaxLevel_DispatchesResponse()
        {
            SkillCreation maxLevelSkill = _miningCreation with {  ReadOnlyLevelable = _miningCreation.ReadOnlyLevelable with { Level = 100 }};
            Assert.DoesNotThrow(() => DispatchSkillCreation(maxLevelSkill));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(maxLevelSkill, _skillCreationResponseListener.Responses[0]);
        }

        [Test]
        public void Negative_SendMultipleCommands_SameSkillID_Throws()
        {
            Assert.DoesNotThrow(() => DispatchSkillCreation(_miningCreation, _miningCreation));
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(2);
            AssertError(typeof(DuplicateEntityException),[_miningCreation, _miningCreation], _skillCreationErrorListener.Error);
        }

        [Test]
        public void Negative_SendSingleCommand_OverMaxLevel_Throws()
        {
            SkillCreation creation = _miningCreation with
            {
                ReadOnlyLevelable = _miningCreation.ReadOnlyLevelable with { Level = LevelConstants.MAX_LEVEL + 1 }
            };
            
            Assert.DoesNotThrow(() => DispatchSkillCreation(creation));
         
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError(typeof(MaxLevelException),[creation], _skillCreationErrorListener.Error);
        }
    }
}
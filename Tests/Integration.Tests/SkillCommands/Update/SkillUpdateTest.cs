using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Skill.Contracts.Command;
using IdelPog.Skill.Contracts.Error;
using IdelPog.Skill.Contracts.Response;

namespace IdelPog.Integration.Tests.SkillCommands.Update
{
    [TestFixture]
    public sealed class SkillUpdateTest : ManagedTestBuffer
    {
        private SkillUpdateResponseListener _responseListener;
        private SkillUpdateErrorListener _errorListener;
        private SkillCreationDispatcher _dispatcher;

        private SkillCreation _miningCreation; 
        private SkillUpdate _miningUpdate;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _dispatcher = new SkillCreationDispatcher();
            
            _miningUpdate = new SkillUpdate
            {
                SkillID = SkillID.MINING
            };
            _miningCreation = _dispatcher.MiningCreation;
        }
        
        [SetUp]
        public void Setup()
        {
            _responseListener = new SkillUpdateResponseListener();
            _errorListener = new SkillUpdateErrorListener();

            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
        }
        
        private void DispatchSkillCreation(params SkillCreation[] skillCreations)
        {
            SkillCreationDispatcher.SendSkillCreationBuffer(skillCreations, BufferManager);
        }

        private void DispatchSkillUpdate(params SkillUpdate[] skillUpdates)
        {
            IBuffer<SkillUpdate> buffer = BufferManager.RequestBuffer<SkillUpdate>(new BufferRequest(skillUpdates.Length));
            buffer.Assign(skillUpdates);
            buffer.MarkReady();
        }

        private void AssertResponseListenerCalled(bool wasCalled)
        {
            Assert.That(_responseListener.WasCalled, Is.EqualTo(wasCalled));
        }

        private void AssertResponseLength(int length)
        {
            Assert.That(_responseListener.SkillUpdateResponses, Has.Length.EqualTo(length));
        }

        private static void AssertResponse(SkillCreation skillCreation, SkillUpdateResponse skillUpdateResponse)
        {
            Assert.Multiple(() =>
            {
                Assert.That(skillCreation.SkillID, Is.EqualTo(skillUpdateResponse.SkillID));
                Assert.That(skillCreation.ReadOnlyLevelable, Is.Not.EqualTo(skillUpdateResponse.ReadOnlyLevelable));
            });
        }
         
        private void AssertErrorListenerCalled(bool wasCalled)
        {
            Assert.That(_errorListener.WasCalled, Is.EqualTo(wasCalled));
        }
        
        private void AssertErrorLength(int length)
        {
            Assert.That(_errorListener.SkillUpdateError.SkillUpdates, Has.Length.EqualTo(length));
        }

        private void AssertError(Type exception, SkillUpdateError error, params SkillUpdate[] updates)
        {
            Assert.Multiple(() =>
            {
                Assert.That(error.BaseError.Exception, Is.TypeOf<ControllerThrownException>());
                Assert.That(error.BaseError.Exception.InnerException, Is.TypeOf(exception));
                Assert.That(error.SkillUpdates, Is.EqualTo(updates));
            });
        }

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        public void Positive_DispatchSkillUpdates_UpdatesSkill_DispatchesExpectedResponses(int amount)
        {
            DispatchSkillCreation(_miningCreation);
            SkillUpdate[] skillUpdates = Enumerable.Range(0, amount).Select(_ => _miningUpdate).ToArray();
            
            Assert.DoesNotThrow(() => DispatchSkillUpdate(skillUpdates));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(amount);

            for (int i = 0; i < amount; i++)
            {
                AssertResponse(_miningCreation, _responseListener.SkillUpdateResponses[i]);
            }
        }

        [Test]
        public void Positive_DispatchSkillUpdate_MultipleSkills_UpdatesBoth_DispatchesResponse()
        {
            SkillCreation foragingCreation = _miningCreation with { SkillID = SkillID.FORAGING };
            SkillUpdate foragingUpdate = new() { SkillID = SkillID.FORAGING };
            DispatchSkillCreation(_miningCreation, foragingCreation);
            
            Assert.DoesNotThrow(() => DispatchSkillUpdate(_miningUpdate, foragingUpdate));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(2);
            AssertResponse(_miningCreation, _responseListener.SkillUpdateResponses[0]);
            AssertResponse(foragingCreation, _responseListener.SkillUpdateResponses[1]);
        }

        [Test]
        public void Negative_DispatchUpdate_SkillNotFound_DispatchesError()
        { 
            Assert.DoesNotThrow(() => DispatchSkillUpdate(_miningUpdate));
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError(typeof(NotFoundException<SkillID>), _errorListener.SkillUpdateError, _miningUpdate);
        }

        [Test]
        public void Negative_DispatchUpdate_MaxLevel_DispatchesError()
        {
            DispatchSkillCreation(_miningCreation with { ReadOnlyLevelable = _miningCreation.ReadOnlyLevelable with { Level = 100 }});
            
            Assert.DoesNotThrow(() => DispatchSkillUpdate(_miningUpdate));
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError(typeof(MaxLevelException), _errorListener.SkillUpdateError, _miningUpdate);
        }
    }
}
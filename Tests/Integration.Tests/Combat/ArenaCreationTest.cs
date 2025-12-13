using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Error;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Core.Contracts;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Progression;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Integration.Tests.Combat
{
    [TestFixture]
    public sealed class ArenaCreationTest : ManagedTestBuffer
    {
        private ManagedResponseListener<ArenaCreationResponse> _arenaCreationResponseListener;
        private ManagedErrorListener<ArenaCreationError> _arenaCreationErrorListener;

        private ArenaCreation _caveCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        { 
            _caveCreation = new ArenaCreation
            {
                ArenaType = ArenaType.CAVE, 
                Information = new Information { Name = "", Description = "" }, 
                ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, ExperiencePerAction = 3, Level = 1, NextLevelExperience = 300 }
            };
        }

        [SetUp]
        public void Setup()
        {
            _arenaCreationResponseListener = new ManagedResponseListener<ArenaCreationResponse>();
            _arenaCreationErrorListener = new ManagedErrorListener<ArenaCreationError>();
            
            ManagedSubscribe(_arenaCreationResponseListener);
            ManagedSubscribe(_arenaCreationErrorListener);
        }

        private void DispatchArenaCreations(params ArenaCreation[] arenaCreations)
        {
            IBuffer<ArenaCreation> buffer = BufferManager.RequestBuffer<ArenaCreation>(new BufferRequest(arenaCreations.Length));
            buffer.Assign(arenaCreations);
            buffer.MarkReady();
        }

        private void AssertResponseListenerCalled(bool called)
        {
            Assert.That(_arenaCreationResponseListener.WasCalled, Is.EqualTo(called));
        }

        private void AssertResponseLength(int length)
        {
            Assert.That(_arenaCreationResponseListener.Responses, Has.Length.EqualTo(length));
        }

        private static void AssertResponse(ArenaCreationResponse arenaCreationResponse, ArenaCreation arenaCreation)
        {
            Assert.Multiple(() =>
            {
                Assert.That(arenaCreationResponse.ArenaType, Is.EqualTo(arenaCreation.ArenaType));
                Assert.That(arenaCreationResponse.ReadOnlyLevelable, Is.EqualTo(arenaCreation.ReadOnlyLevelable));
                Assert.That(arenaCreationResponse.Information, Is.EqualTo(arenaCreation.Information));
            });
        }
        
        private void AssertErrorListenerCalled(bool called)
        {
            Assert.That(_arenaCreationErrorListener.WasCalled, Is.EqualTo(called));
        }

        private void AssertErrorLength(int length)
        {
            Assert.That(_arenaCreationErrorListener.Error.ArenaCreations, Has.Length.EqualTo(length));
        }

        private void AssertError<TException>(params ArenaCreation[] arenaCreations) where TException : Exception
        {
            BaseError baseError = _arenaCreationErrorListener.Error.BaseError;
            Assert.Multiple(() =>
            {
                Assert.That(_arenaCreationErrorListener.Error.ArenaCreations, Is.EqualTo(arenaCreations));
                Assert.That(baseError.Exception, Is.TypeOf<ControllerThrownException>());
                Assert.That(baseError.Exception.InnerException, Is.TypeOf<TException>());
            });
        }

        [Test]
        public void Positive_DispatchSingleCreation_CreatesArena_DispatchesResponse()
        { 
            DispatchArenaCreations(_caveCreation);
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_arenaCreationResponseListener.Responses[0], _caveCreation);
        }

        [Test]
        public void Positive_DispatchMultipleCreations_DispatchesMultipleResponses()
        {
            ArenaCreation fieldCreation = _caveCreation with { ArenaType = ArenaType.FIELD };
            
            DispatchArenaCreations(_caveCreation, fieldCreation);
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(2);
            AssertResponse(_arenaCreationResponseListener.Responses[0], _caveCreation);
            AssertResponse(_arenaCreationResponseListener.Responses[1], fieldCreation);
        }

        [Test]
        public void Negative_DispatchMultipleCreations_DuplicateArenaType_DispatchesError()
        {
            DispatchArenaCreations(_caveCreation, _caveCreation);
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(2);
            AssertError<DuplicateEntityException>(_caveCreation, _caveCreation);
        }

        [Test]
        public void Negative_DispatchSingleCreation_OverMaxLevel_DispatchesError()
        {
            ArenaCreation overMaxLevelCreation = _caveCreation with { ReadOnlyLevelable = _caveCreation.ReadOnlyLevelable with { Level = LevelConstants.MAX_LEVEL + 1 } };
            
            DispatchArenaCreations(overMaxLevelCreation);
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError<MaxLevelException>(overMaxLevelCreation);
        }
    }
}
using IdelPog.Combat.Stat.Contracts.Command;
using IdelPog.Combat.Stat.Contracts.Response;
using IdelPog.Core.Contracts;

namespace IdelPog.Integration.Tests.Combat.Stat
{
    [TestFixture]
    public sealed class StatCreationTest : ManagedTestBuffer
    {
        private ManagedResponseListener<StatCreationResponse> _responseListener;
        private ManagedErrorListener<BufferedError<StatCreation>> _errorListener;
        
        private readonly StatCreation _statCreation = new()
        {
            InitialValue = 10
        };
        
        [SetUp]
        public void Setup()
        {
            _responseListener = new ManagedResponseListener<StatCreationResponse>();
            _errorListener = new ManagedErrorListener<BufferedError<StatCreation>>();
            
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
        }
        
        private static void AssertResponse(StatCreationResponse response, StatCreation source, byte expectedID)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.StatCreation, Is.EqualTo(source));
                Assert.That(response.StatID, Is.EqualTo(expectedID));
            }
        }

        [Test]
        public void Positive_DispatchMessages_CreatesNewStats_DispatchesResponse()
        { 
            DispatchMessage(_statCreation);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], _statCreation, 0);
        }
        
        [Test]
        public void Positive_DispatchMessages_MultipleStats_DispatchesResponse()
        {
            StatCreation minValueCreation = new() { InitialValue = uint.MinValue };
            StatCreation maxValueCreation = new() { InitialValue = uint.MaxValue };
            
            DispatchMessage(_statCreation, minValueCreation, maxValueCreation);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(3);
            AssertResponse(_responseListener.Responses[0], _statCreation, 0);
            AssertResponse(_responseListener.Responses[1], minValueCreation, 1);
            AssertResponse(_responseListener.Responses[2], maxValueCreation, 2);
        }
    }
}
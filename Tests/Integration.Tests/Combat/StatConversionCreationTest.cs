using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Stat.Contracts.Command;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Contracts.Response;
using IdelPog.Core.Contracts;

namespace IdelPog.Integration.Tests.Combat
{
    public sealed class StatConversionCreationTest : ManagedTestBuffer
    {
        private ManagedResponseListener<StatConversionCreationResponse> _responseListener;
        private ManagedErrorListener<BufferedError<StatConversionCreation>> _errorListener;

        private readonly StatConversionCreation _goodStatConversion = new()
        {
            TargetStatType = StatType.HEALTH,
            SourceStatType = StatType.BASE_HEALTH,
            SourceStatInterval = 10,
            StatOperation = StatOperation.ADDITIVE,
            TargetStatModifier = 1
        };
        
        [SetUp]
        public void Setup()
        {
            _responseListener = new ManagedResponseListener<StatConversionCreationResponse>();
            _errorListener = new ManagedErrorListener<BufferedError<StatConversionCreation>>();
            
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
        }

        private static void AssertResponse(StatConversionCreationResponse response, StatConversionCreation source, byte expectedID)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.StatConversionCreation, Is.EqualTo(source));
                Assert.That(response.StatConversionID, Is.EqualTo(expectedID));
            }
        }

        private void AssertErrorLength(int length)
        { 
            Assert.That(_errorListener.Error.Commands, Has.Length.EqualTo(length));
        }

        private void AssertErrorCollection(params StatConversionCreation[] combatantCreations)
        {
            Assert.That(_errorListener.Error.Commands, Is.EqualTo(combatantCreations));
        }

        private static void AssertSource(Exception exception, string expectedSource)
        {
            Assert.That(exception.Source, Is.EqualTo(expectedSource));
        }

        [Test]
        public void Positive_DispatchMessage_CreatesNewConversion()
        { 
            DispatchMessage(_goodStatConversion);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], _goodStatConversion, expectedID: 0);
        }

        [Test]
        public void Positive_DispatchMessage_MultipleCommands_IncreasesID()
        {
            DispatchMessage(_goodStatConversion, _goodStatConversion);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(2);
            AssertResponse(_responseListener.Responses[0], _goodStatConversion, expectedID: 0);
            AssertResponse(_responseListener.Responses[1], _goodStatConversion, expectedID: 1);
        }

        [Test]
        public void Positive_DispatchMessage_NegativeTargetStat()
        {
            DispatchMessage(_goodStatConversion with { TargetStatModifier = -10 });
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], _goodStatConversion with { TargetStatModifier = -10 }, expectedID: 0);
        }

        [Test]
        public void Negative_DispatchMessage_ZeroTargetStatModifier_Throws()
        {
            DispatchMessage(_goodStatConversion with { TargetStatModifier = 0 });
            
            _responseListener.AssertWasCalled(false);
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertBaseError<NumberZeroException>(_errorListener.Error.BaseError);
            AssertErrorCollection(_goodStatConversion with { TargetStatModifier = 0 });
            AssertSource(_errorListener.Error.BaseError.Exception.InnerException!, nameof(_goodStatConversion.TargetStatModifier));
        }

        [Test]
        public void Negative_DispatchMessage_ZeroOrNegativeSourceStatInterval_Throws()
        {
            DispatchMessage(_goodStatConversion with { SourceStatInterval = 0 });
            
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertBaseError<NumberZeroException>(_errorListener.Error.BaseError);
            AssertSource(_errorListener.Error.BaseError.Exception.InnerException!, nameof(_goodStatConversion.SourceStatInterval));
            
            DispatchMessage(_goodStatConversion with { SourceStatInterval = -10 });
            
            _responseListener.AssertWasCalled(false);
            AssertErrorLength(1);
            AssertBaseError<NegativeNumberException>(_errorListener.Error.BaseError);
            AssertSource(_errorListener.Error.BaseError.Exception.InnerException!, nameof(_goodStatConversion.SourceStatInterval));
        }
    }
}
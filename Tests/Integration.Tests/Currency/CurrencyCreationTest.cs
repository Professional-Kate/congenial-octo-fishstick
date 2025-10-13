using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Currency.Contracts.Command;
using IdelPog.Currency.Contracts.Error;
using IdelPog.Currency.Contracts.Response;

namespace IdelPog.Integration.Tests.Currency
{
    [TestFixture]
    public sealed class CurrencyCreationTest : ManagedTestBuffer
    {
        private ManagedResponseListener<CurrencyCreationResponse> _currencyCreationResponseListener;
        private ManagedErrorListener<CurrencyCreationError> _currencyCreationErrorListener;

        private CurrencyCreation _createGold;
        private CurrencyCreation _createGems;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _createGold = new CurrencyCreation
            {
                CurrencyType = CurrencyType.GOLD,
                StartingAmount = 0
            };

            _createGems = new CurrencyCreation
            {
                CurrencyType = CurrencyType.GEMS,
                StartingAmount = 0
            };
        }

        private void SendCurrencyCreationBuffer(CurrencyCreation[] currencyCreations)
        {
            IBuffer<CurrencyCreation> buffer = BufferManager.RequestBuffer<CurrencyCreation>(new BufferRequest(currencyCreations.Length));
            buffer.Assign(currencyCreations);
            buffer.MarkReady();
        }

        private void AssertCurrencyCreationResponseListener(CurrencyCreation[] currencyCreations, bool wasCalled)
        {
            if (wasCalled == false)
            {
                Assert.That(_currencyCreationResponseListener.WasCalled, Is.False);
                return;
            }

            Assert.Multiple(() =>
            {
                Assert.That(_currencyCreationResponseListener.WasCalled, Is.True);
                Assert.That(_currencyCreationResponseListener.Responses, Is.Not.Null);
                Assert.That(_currencyCreationResponseListener.Responses, Has.Length.EqualTo(currencyCreations.Length));
            });
        }

        private void AssertCurrencyCreationErrorListener(bool wasCalled)
        {
            if (wasCalled == false)
            {
                Assert.That(_currencyCreationErrorListener.WasCalled, Is.False);
                return;
            }

            Assert.That(_currencyCreationErrorListener.WasCalled, Is.True);
        }

        private void AssertCreationError<TException>(CurrencyCreationError currencyCreationError, CurrencyCreation[] creations)
        {
            BaseError baseError = _currencyCreationErrorListener.Error.BaseErrorDetails;
            Assert.Multiple(() =>
            {
                Assert.That(currencyCreationError.CurrencyCreations, Is.EquivalentTo(creations));
                Assert.That(baseError.Exception.GetType(), Is.EqualTo(typeof(TException)));
            });
        }

        [SetUp]
        public void SetUp()
        {
            _currencyCreationResponseListener = new ManagedResponseListener<CurrencyCreationResponse>();
            _currencyCreationErrorListener = new ManagedErrorListener<CurrencyCreationError>();

            ManagedSubscribe(_currencyCreationResponseListener);
            ManagedSubscribe(_currencyCreationErrorListener);
        }

        [Test]
        public void Positive_SendSingleCommand_DispatchesSingle_CurrencyCreationDTO()
        {
            CurrencyCreation[] currencyCreations = [_createGold];
            Assert.DoesNotThrow(() => SendCurrencyCreationBuffer(currencyCreations));
            AssertCurrencyCreationResponseListener(currencyCreations, true);
            AssertCurrencyCreationErrorListener(false);

            CurrencyCreationResponse[] creationResponses = _currencyCreationResponseListener.Responses;
            Assert.Multiple(() =>
            {
                Assert.That(creationResponses, Has.Length.EqualTo(1));
                Assert.That(creationResponses[0].Amount, Is.EqualTo(_createGold.StartingAmount));
                Assert.That(creationResponses[0].CurrencyType, Is.EqualTo(_createGold.CurrencyType));
            });
        }

        [Test]
        public void Positive_SendMultipleCommands_DispatchesMultiple_CurrencyCreationDTO()
        {
            CurrencyCreation[] currencyCreations = [_createGold, _createGems];
            Assert.DoesNotThrow(() => SendCurrencyCreationBuffer(currencyCreations));
            AssertCurrencyCreationResponseListener(currencyCreations, true);
            AssertCurrencyCreationErrorListener(false);

            CurrencyCreationResponse[] creationResponses = _currencyCreationResponseListener.Responses;
            foreach (CurrencyCreationResponse response in creationResponses)
            {
                Assert.That(response.Amount, Is.EqualTo(_createGems.StartingAmount));

                switch (response.CurrencyType)
                {
                    case CurrencyType.GOLD:
                        Assert.That(response.CurrencyType, Is.EqualTo(_createGold.CurrencyType));
                        break;
                    case CurrencyType.GEMS:
                        Assert.That(response.CurrencyType, Is.EqualTo(_createGems.CurrencyType));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        [Test]
        public void Negative_SendDuplicateCommand_NoCreation_SendsErrorDTO()
        {
            CurrencyCreation[] currencyCreations = [_createGold, _createGems, _createGold];
            Assert.DoesNotThrow(() => SendCurrencyCreationBuffer(currencyCreations));
            AssertCurrencyCreationResponseListener(currencyCreations, false);
            AssertCurrencyCreationErrorListener(true);
            AssertCreationError<ControllerThrownException>(_currencyCreationErrorListener.Error, currencyCreations);
        }
    }
}
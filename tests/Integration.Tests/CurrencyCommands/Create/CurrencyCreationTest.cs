using IdelPog.Common.Enums;
using IdelPog.Common.Errors;
using IdelPog.Flows;
using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Exceptions;
using IdelPog.SimulationEngine.Currency;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.Responses;

namespace Integration.Tests.CurrencyCommands.Create
{
    [TestFixture]
    public class CurrencyCreationTest : ManagedBuffer
    {
        private CurrencyCreationResponseListener _currencyCreationResponseListener;
        private CurrencyCreationErrorListener _currencyCreationErrorListener;

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
                Assert.That(_currencyCreationResponseListener.Item.CurrencyCreations, Is.Not.Null);
                Assert.That(_currencyCreationResponseListener.Item.CurrencyCreations, Has.Length.EqualTo(currencyCreations.Length));
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
            BaseError baseError = _currencyCreationErrorListener.CurrencyUpdateError.BaseErrorDetails;
            Assert.Multiple(() =>
            {
                Assert.That(currencyCreationError.CurrencyCreations, Is.EquivalentTo(creations));
                Assert.That(baseError.Exception.GetType(), Is.EqualTo(typeof(TException)));
            });
        }

        [SetUp]
        public void SetUp()
        {
            _currencyCreationResponseListener = new CurrencyCreationResponseListener();
            _currencyCreationErrorListener = new CurrencyCreationErrorListener();

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

            CurrencyCreationResponse creationResponse = _currencyCreationResponseListener.Item;
            Assert.Multiple(() =>
            {
                Assert.That(creationResponse.CurrencyCreations, Is.EquivalentTo(currencyCreations));
            });
        }

        [Test]
        public void Positive_SendMultipleCommands_DispatchesMultiple_CurrencyCreationDTO()
        {
            CurrencyCreation[] currencyCreations = [_createGold, _createGems];
            Assert.DoesNotThrow(() => SendCurrencyCreationBuffer(currencyCreations));
            AssertCurrencyCreationResponseListener(currencyCreations, true);
            AssertCurrencyCreationErrorListener(false);

            CurrencyCreationResponse creationResponse = _currencyCreationResponseListener.Item;
            foreach (CurrencyCreation currencyCreation in creationResponse.CurrencyCreations)
            {
                Assert.That(currencyCreation.StartingAmount, Is.EqualTo(_createGems.StartingAmount));

                switch (currencyCreation.CurrencyType)
                {
                    case CurrencyType.GOLD:
                        Assert.That(currencyCreation.CurrencyType, Is.EqualTo(_createGold.CurrencyType));
                        break;
                    case CurrencyType.GEMS:
                        Assert.That(currencyCreation.CurrencyType, Is.EqualTo(_createGems.CurrencyType));
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
            AssertCreationError<ControllerThrownException>(_currencyCreationErrorListener.CurrencyUpdateError, currencyCreations);
        }
    }
}
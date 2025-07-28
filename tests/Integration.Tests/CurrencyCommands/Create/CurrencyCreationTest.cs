using IdelPog.Common.DTO.Error;
using IdelPog.Common.Enums;
using IdelPog.Messaging.Buffer;
using IdelPog.SimulationEngine.Currency;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;
using IdelPog.Validation.Exceptions;

namespace Integration.Tests.CurrencyCommands.Create
{
    [TestFixture]
    public class CurrencyCreationTest : ManagedBuffer
    {
        private CurrencyCreationDTOListener _currencyCreationDTOListener;
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

        private void AssertCurrencyCreationDTOListener(CurrencyCreation[] currencyCreations, bool wasCalled)
        {
            if (wasCalled == false)
            {
                Assert.That(_currencyCreationDTOListener.WasCalled, Is.False);
                return;
            }

            Assert.Multiple(() =>
            {
                Assert.That(_currencyCreationDTOListener.WasCalled, Is.True);
                Assert.That(_currencyCreationDTOListener.Buffer, Is.Not.Null);
                Assert.That(_currencyCreationDTOListener.Buffer!, Has.Count.EqualTo(currencyCreations.Length));
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

        private void AssertCreationErrorDTO<T>(CurrencyCreationDTO creationDTO, CurrencyCreation creation)
        {
            ErrorDTO errorDTO = _currencyCreationErrorListener.CurrencyUpdateErrorDTO.ErrorDetails;
            Assert.Multiple(() =>
            {
                Assert.That(creationDTO.CurrencyType, Is.EqualTo(creation.CurrencyType));
                Assert.That(creationDTO.Amount, Is.EqualTo(creation.StartingAmount));
                Assert.That(errorDTO.Exception, Is.TypeOf(typeof(T)));
            });
        }

        [SetUp]
        public void SetUp()
        {
            new CurrencyBootstrapper().Initialize(BufferMessenger, BufferManager);

            _currencyCreationDTOListener = new CurrencyCreationDTOListener();
            _currencyCreationErrorListener = new CurrencyCreationErrorListener();

            ManagedSubscribe(_currencyCreationDTOListener);
            ManagedSubscribe(_currencyCreationErrorListener);
        }

        [Test]
        public void Positive_SendSingleCommand_DispatchesSingle_CurrencyCreationDTO()
        {
            CurrencyCreation[] currencyCreations = [_createGold];
            Assert.DoesNotThrow(() => SendCurrencyCreationBuffer(currencyCreations));
            AssertCurrencyCreationDTOListener(currencyCreations, true);
            AssertCurrencyCreationErrorListener(false);

            CurrencyCreationDTO creationDTO = _currencyCreationDTOListener.Buffer![0];
            Assert.Multiple(() =>
            {
                Assert.That(creationDTO.Amount, Is.EqualTo(_createGold.StartingAmount));
                Assert.That(creationDTO.CurrencyType, Is.EqualTo(_createGold.CurrencyType));
            });
        }

        [Test]
        public void Positive_SendMultipleCommands_DispatchesMultiple_CurrencyCreationDTO()
        {
            CurrencyCreation[] currencyCreations = [_createGold, _createGems];
            Assert.DoesNotThrow(() => SendCurrencyCreationBuffer(currencyCreations));
            AssertCurrencyCreationDTOListener(currencyCreations, true);
            AssertCurrencyCreationErrorListener(false);

            IReadOnlyList<CurrencyCreationDTO> creationDTOs = _currencyCreationDTOListener.Buffer!;
            foreach (CurrencyCreationDTO currencyCreationDTO in creationDTOs)
            {
                Assert.That(currencyCreationDTO.Amount, Is.EqualTo(_createGems.StartingAmount));

                switch (currencyCreationDTO.CurrencyType)
                {
                    case CurrencyType.GOLD:
                        Assert.That(currencyCreationDTO.CurrencyType, Is.EqualTo(_createGold.CurrencyType));
                        break;
                    case CurrencyType.GEMS:
                        Assert.That(currencyCreationDTO.CurrencyType, Is.EqualTo(_createGems.CurrencyType));
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
            AssertCurrencyCreationDTOListener(currencyCreations, false);
            AssertCurrencyCreationErrorListener(true);

            CurrencyCreationDTO[] creationDTOs = _currencyCreationErrorListener.CurrencyUpdateErrorDTO.CurrencyCreations;
            // AssertCreationErrorDTO<DuplicateEntityException>(creationDTOs, _createGold);
        }
    }
}
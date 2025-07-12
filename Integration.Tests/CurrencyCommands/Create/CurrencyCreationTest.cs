using IdelPog.Messaging.Buffer;
using IdelPog.SimulationEngine.Currency;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;
using IdelPog.SimulationEngine.Currency.Exceptions;
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
                StartingAmount = 10
            };

            _createGems = new CurrencyCreation
            {
                CurrencyType = CurrencyType.GEMS,
                StartingAmount = 10
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

        private void AssertCurrencyCreationErrorListener(CurrencyCreation[] currencyCreations, bool wasCalled)
        {
            if (wasCalled == false)
            {
                Assert.That(_currencyCreationErrorListener.WasCalled, Is.False);
                return;
            }

            Assert.Multiple(() =>
            {
                Assert.That(_currencyCreationErrorListener.WasCalled, Is.True);
                Assert.That(_currencyCreationErrorListener.CurrencyUpdateErrorDTO.CurrencyCreations, Is.Not.Null);
                Assert.That(_currencyCreationErrorListener.CurrencyUpdateErrorDTO.CurrencyCreations, Has.Length.EqualTo(currencyCreations.Length));
            });
        }
        
        private void AssertCreationErrorDTO<T>(CurrencyCreationDTO creationDTO, CurrencyCreation creation)
        {
            ErrorDTO errorDTO = _currencyCreationErrorListener.CurrencyUpdateErrorDTO.ErrorDetails;
            Assert.Multiple(() =>
            {
                Assert.That(creationDTO.Currency, Is.EqualTo(creation.CurrencyType));
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
            AssertCurrencyCreationErrorListener(currencyCreations, false);

            CurrencyCreationDTO creationDTO = _currencyCreationDTOListener.Buffer![0];
            Assert.Multiple(() =>
            {
                Assert.That(creationDTO.Amount, Is.EqualTo(_createGold.StartingAmount));
                Assert.That(creationDTO.Currency, Is.EqualTo(_createGold.CurrencyType));
            });
        }

        [Test]
        public void Positive_SendMultipleCommands_DispatchesMultiple_CurrencyCreationDTO()
        {
            CurrencyCreation[] currencyCreations = [_createGold, _createGems];
            Assert.DoesNotThrow(() => SendCurrencyCreationBuffer(currencyCreations));
            AssertCurrencyCreationDTOListener(currencyCreations, true);
            AssertCurrencyCreationErrorListener(currencyCreations, false);
            
            IReadOnlyList<CurrencyCreationDTO> creationDTOs = _currencyCreationDTOListener.Buffer!;
            foreach (CurrencyCreationDTO currencyCreationDTO in creationDTOs)
            {
                Assert.That(currencyCreationDTO.Amount, Is.EqualTo(10));
                
                switch (currencyCreationDTO.Currency)
                {
                    case CurrencyType.GOLD: 
                        Assert.That(currencyCreationDTO.Currency, Is.EqualTo(_createGold.CurrencyType));
                        break;
                    case CurrencyType.GEMS: 
                        Assert.That(currencyCreationDTO.Currency, Is.EqualTo(_createGems.CurrencyType));
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
            AssertCurrencyCreationErrorListener(currencyCreations, true);

            CurrencyCreationDTO creationDTO = _currencyCreationErrorListener.CurrencyUpdateErrorDTO.CurrencyCreations[0];
            AssertCreationErrorDTO<DuplicateItemException>(creationDTO, _createGold);
            
        }

        [Test]
        public void Negative_SendCommand_WithNegativeStartingAmount_NoCreation_SendsErrorDTO()
        {
            CurrencyCreation negativeNumberCommand = new() { CurrencyType = CurrencyType.GOLD, StartingAmount = -10 };
            CurrencyCreation[] currencyCreations = [negativeNumberCommand];
            Assert.DoesNotThrow(() => SendCurrencyCreationBuffer(currencyCreations));
            AssertCurrencyCreationDTOListener(currencyCreations, false);
            AssertCurrencyCreationErrorListener(currencyCreations, true);
            
            CurrencyCreationDTO creationDTO = _currencyCreationErrorListener.CurrencyUpdateErrorDTO.CurrencyCreations[0];
            AssertCreationErrorDTO<NegativeNumberException>(creationDTO, negativeNumberCommand);
        }
    }
}
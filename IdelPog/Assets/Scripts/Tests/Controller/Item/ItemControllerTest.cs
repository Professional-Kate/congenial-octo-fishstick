using IdelPog.Orchestration;
using IdelPog.Structures;
using IdelPog.Structures.Enums;
using IdelPog.Structures.Models.Item;
using Moq;
using NUnit.Framework;

namespace Tests.Controller
{
    [TestFixture]
    public class ItemControllerTest
    {
        private TestableItemController _itemController { get; set; }
        private Mock<IInventoryMediator> _inventoryMediatorMock { get; set; }

        private const InventoryID ID = InventoryID.WILLOW_WOOD;
        private const int AMOUNT = 10;

        [SetUp]
        public void SetUp()
        {
            _inventoryMediatorMock = new Mock<IInventoryMediator>();
            _itemController = new TestableItemController(_inventoryMediatorMock.Object);
        }

        private void VerifyDependencyCalls(int addCalls, int removeCalls)
        {
            _inventoryMediatorMock.Verify(library => library.AddAmount(ID, AMOUNT), Times.Exactly(addCalls));
            _inventoryMediatorMock.Verify(library => library.RemoveAmount(ID, AMOUNT), Times.Exactly(removeCalls));
        }

        private void SetupMock(ServiceResponse response)
        {
            _inventoryMediatorMock.Setup(library => library.AddAmount(ID, AMOUNT))
                .Returns(response);
            
            _inventoryMediatorMock.Setup(library => library.RemoveAmount(ID, AMOUNT))
                .Returns(response);
        }

        [TestCase(ActionType.ADD)]
        [TestCase(ActionType.REMOVE)]
        public void Positive_ModifyItem_CallsCorrectMethod_ReturnsTrue(ActionType actionType)
        {
           SetupMock(ServiceResponse.Success());
            
            _itemController.ModifyItem(ID, AMOUNT, actionType);

            switch (actionType)
            {
                case ActionType.ADD:
                    VerifyDependencyCalls(1, 0);
                    break;
                case ActionType.REMOVE:
                    VerifyDependencyCalls(0, 1);
                    break;
            }
        }
        
        [TestCase(ActionType.ADD)]
        [TestCase(ActionType.REMOVE)]
        public void Positive_ModifyItem_Fails_ReturnsFalse(ActionType actionType)
        {
            SetupMock(ServiceResponse.Failure(""));
            
            _itemController.ModifyItem(ID, AMOUNT, actionType);

            switch (actionType)
            {
                case ActionType.ADD:
                    VerifyDependencyCalls(1, 0);
                    break;
                case ActionType.REMOVE:
                    VerifyDependencyCalls(0, 1);
                    break;
            }
        }
    }
}
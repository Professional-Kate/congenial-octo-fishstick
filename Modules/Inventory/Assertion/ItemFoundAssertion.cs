using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Inventory.Assertion.Interface;

namespace IdelPog.Inventory.Assertion
{
    public sealed class ItemFoundAssertion : IItemFoundAssertion
    {
        public void AssertItemFound(bool contains, ActionType actionType, ItemID itemID)
        {
            if (actionType != ActionType.REMOVE)
            {
                return;
            }

            if (contains == false)
            {
                throw new NotFoundException<ItemID>(itemID);
            }
        }
    }
}
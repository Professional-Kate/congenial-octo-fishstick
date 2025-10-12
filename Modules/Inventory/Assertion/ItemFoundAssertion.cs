using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.Inventory.Assertion.Interface;

namespace IdelPog.Inventory.Assertion
{
    public sealed class ItemFoundAssertion : BaseAssertion, IItemFoundAssertion
    {
        public ItemFoundAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertItemFound(bool contains, ActionType actionType, ItemID itemID)
        {
            Assert<NotFoundException<ItemID>>(() =>
            {
                if (actionType != ActionType.REMOVE)
                {
                    return;
                }

                if (contains == false)
                {
                    throw new NotFoundException<ItemID>(itemID);
                }
            });
        }
    }
}
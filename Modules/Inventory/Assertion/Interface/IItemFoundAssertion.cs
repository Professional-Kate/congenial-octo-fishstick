using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Inventory.Assertion.Interface
{
    public interface IItemFoundAssertion
    {
        public void AssertItemFound(bool contains, ActionType actionType, ItemID itemID);
    }
}
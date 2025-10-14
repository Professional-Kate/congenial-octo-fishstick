using IdelPog.Core.Contracts.Enum;
using IdelPog.Inventory.Assertion.Interface;
using IdelPog.Inventory.Exceptions;

namespace IdelPog.Inventory.Assertion
{
    public sealed class AmountAssertion : IAmountAssertion
    {
        public void AssertAmountNotZero(uint amount)
        {
            if (amount == 0)
            {
                throw new AmountZeroException();
            }
        }

        public void AssertEnoughAmount(uint requestedAmount, uint actualAmount, ItemID itemID)
        {
            if (requestedAmount > actualAmount)
            {
                throw new InsufficientAmountException(requestedAmount, actualAmount, itemID);
            }
        }
    }
}
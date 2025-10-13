using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.Inventory.Assertion.Interface;
using IdelPog.Inventory.Exceptions;

namespace IdelPog.Inventory.Assertion
{
    public class AmountAssertion : BaseAssertion, IAmountAssertion
    {
        public AmountAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertAmountNotZero(uint amount)
        {
            Assert<AmountZeroException>(() =>
            {
                if (amount == 0)
                {
                    throw new AmountZeroException();
                }
            });
        }

        public void AssertEnoughAmount(uint requestedAmount, uint actualAmount, ItemID itemID)
        {
            Assert<InsufficientAmountException>(() =>
            {
                if (requestedAmount > actualAmount)
                {
                    throw new InsufficientAmountException(requestedAmount, actualAmount, itemID);
                }
            });
        }
    }
}
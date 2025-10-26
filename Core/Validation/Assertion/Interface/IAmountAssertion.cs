using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Validation.Assertion.Interface
{
    public interface IAmountAssertion
    {
        public void AssertAmountNotZero(uint amount);
        
        public void AssertEnoughAmount(uint requestedAmount, uint actualAmount, ItemID itemID);
    }
}
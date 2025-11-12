using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Validation.Exceptions
{
    public class InsufficientAmountException : Exception
    {
        private const string MESSAGE = "Item ID : {0} does not have emough amount to remove!!!!! Requested amount : {1}. Actual Amount : {2})";

        public readonly ItemID ItemID;
        public readonly uint RequestedAmount;
        public readonly uint ActualAmount;

        public InsufficientAmountException(uint requestedAmount, uint actualAmount, ItemID itemID) : base(string.Format(MESSAGE, itemID, requestedAmount, actualAmount))
        {
            RequestedAmount = requestedAmount;
            ActualAmount = actualAmount;
            ItemID = itemID;
        }
    }
}
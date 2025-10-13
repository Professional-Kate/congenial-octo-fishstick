namespace IdelPog.Inventory.Exceptions
{
    public sealed class AmountZeroException : Exception
    {
        private const string MESSAGE = "The passed amount was zero!!! Don't be cringe!";
        
        public AmountZeroException() : base(MESSAGE) { }
    }
}
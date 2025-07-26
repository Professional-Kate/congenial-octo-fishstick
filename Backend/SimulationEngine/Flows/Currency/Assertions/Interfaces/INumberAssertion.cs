namespace IdelPog.SimulationEngine.Currency.Assertions
{
    public interface INumberAssertion
    {
        public void AssertNonNegative(int number);
        
        public void AssertAllNonNegative(int[] numbers);
    }
}
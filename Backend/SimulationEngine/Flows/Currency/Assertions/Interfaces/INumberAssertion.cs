namespace IdelPog.SimulationEngine.Currency.Assertions
{
    public interface INumberAssertion
    {
        public void AssertNonNegative(uint number);

        public void AssertAllNonNegative(uint[] numbers);
    }
}
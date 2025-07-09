namespace IdelPog.SimulationEngine.Currency.Assertions
{
    public interface IAssertCollectionNotEmpty
    {
        public void Handle<T>(IReadOnlyList<T> collection);
    }
}
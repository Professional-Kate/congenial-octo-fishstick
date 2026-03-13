namespace IdelPog.Progression.Assertion.Interface
{
    public interface IQueueAssertion
    {
        public void AssertSuccessfulDequeue(bool successfulDequeue);

        public void AssertSuccessfulPeek(bool successfulPeek);
    }
}
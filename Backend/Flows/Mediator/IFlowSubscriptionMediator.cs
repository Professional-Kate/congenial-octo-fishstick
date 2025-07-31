namespace IdelPog.Flows
{
    public interface IFlowSubscriptionMediator
    {
        public void ConstructAndSubscribe<TCommand, TError>();
    }
}
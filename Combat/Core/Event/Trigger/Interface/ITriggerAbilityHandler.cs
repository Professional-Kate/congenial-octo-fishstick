namespace IdelPog.Combat.Core.Event.Trigger.Interface
{
    public interface ITriggerAbilityHandler<in T> where T : struct
    {
        public void Handle(double tick, T triggerData);
    }
}
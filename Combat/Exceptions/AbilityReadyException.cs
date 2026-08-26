namespace IdelPog.Combat.Exceptions
{
    public sealed class AbilityReadyException : Exception
    {
        private const string MESSAGE = "AbilityReady is not configured properly!!\nRules:\n -> TargetingType has to be SELF\n -> MinTriggerValue has to be zero\n -> MaxTriggerValue has to be zero";
        
        public AbilityReadyException() : base(MESSAGE) { }
    }
}
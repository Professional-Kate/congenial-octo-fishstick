namespace IdelPog.Combat.Exceptions
{
    public sealed class PriorityMismatchException : Exception
    {
        private const string MESSAGE = "AbilityStage {0} : StrategyCard {1}\n Priorities do not match!!! Please provide correct Priority!!";

        public readonly byte AbilityStagePriority;
        public readonly byte StrategyCardPriority;

        public PriorityMismatchException(byte abilityStagePriority, byte strategyCardPriority) : base(string.Format(MESSAGE, abilityStagePriority, strategyCardPriority))
        {
            AbilityStagePriority = abilityStagePriority;
            StrategyCardPriority = strategyCardPriority;
        }
    }
}
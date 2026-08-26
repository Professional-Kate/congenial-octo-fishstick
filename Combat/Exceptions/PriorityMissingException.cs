namespace IdelPog.Combat.Exceptions
{
    public sealed class PriorityMissingException : Exception
    {
        private const string MESSAGE = "AbilityStages length {0} : StrategyCards length {1}\nNot enough Priority have been provided!!!";

        public PriorityMissingException(int abilityStagesLength, int strategyCardLength) : base(string.Format(MESSAGE, abilityStagesLength, strategyCardLength)) { }
    }
}
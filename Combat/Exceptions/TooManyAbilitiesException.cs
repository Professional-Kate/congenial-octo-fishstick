namespace IdelPog.Combat.Exceptions
{
    public sealed class TooManyAbilitiesException : Exception
    {
        private const string MESSAGE = "Oops! Too many abliities! You tried to equip {0} abilities when you set the MaxAbilities to {1}!!";
        
        public TooManyAbilitiesException(byte abilitySlots, byte maxAbilities) : base(string.Format(MESSAGE, abilitySlots, maxAbilities))
        {
        }
    }
}
namespace IdelPog.Combat.Exceptions
{
    public sealed class CombatantDeadException : Exception
    {
        private const string MESSAGE = "Oi!! Entity: {0} you are not alive!!! You are not allowed to do this!!";

        public readonly byte CombatantID;
        
        public CombatantDeadException(byte combatantID) : base(string.Format(MESSAGE, combatantID))
        {
            CombatantID = combatantID;
        }
    }
}
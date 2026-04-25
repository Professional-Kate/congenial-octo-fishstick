namespace IdelPog.Combat.Runtime.System.Interface
{
    public interface IFriendlyStatusAssigner
    { 
        public void AssignFriendlyStatus(byte[] friendlyCombatantIDs, byte[] enemyCombatantIDs);
    }
}
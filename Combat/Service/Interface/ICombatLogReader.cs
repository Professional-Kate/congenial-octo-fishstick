using IdelPog.Combat.Contracts;

namespace IdelPog.Combat.Service.Interface
{
    public interface ICombatLogReader
    {
        public CombatEventLog CurrentCombatState { get; }
        
        public bool NextCombatState();

        public void Dispose();
    }
}
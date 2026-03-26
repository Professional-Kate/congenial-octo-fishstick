using IdelPog.Combat.Contracts;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Service
{
    public sealed class CombatLogReader : ICombatLogReader
    {
        private readonly IEnumerator<CombatEventLog> _enumerator;

        public CombatLogReader(IEnumerator<CombatEventLog> enumerator)
        {
            _enumerator = enumerator;
        }

        public CombatEventLog CurrentCombatState { get; private set; }
        
        public bool NextCombatState()
        {
            bool successful = _enumerator.MoveNext();
            if (successful)
            {
                CurrentCombatState = _enumerator.Current;
            }
            
            return successful;
        }

        public void Dispose()
        {
            _enumerator.Dispose();
        }
    }
}
using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Card;

namespace IdelPog.Integration.Tests.Combat.Tools
{
    internal sealed class CombatTools
    {
        private readonly Dictionary<byte, CombatantTracker> _combatantCards = new();
        internal CombatantStateChange FirstDeadCombatant { get; private set; }

        /// <summary>
        /// Should be called from a setup or teardown method
        /// </summary>
        internal void Reset()
        {
            _combatantCards.Clear();
            FirstDeadCombatant = default;
        }

        /// <summary>
        /// Will update each property of this class
        /// </summary>
        /// <param name="combatantStateChanges">The full array of state changes</param>
        internal void RegisterChanges(CombatantStateChange[] combatantStateChanges)
        {
            foreach (CombatantStateChange combatantStateChange in combatantStateChanges)
            {
                if (_combatantCards.ContainsKey(combatantStateChange.CombatantID) == false)
                {
                    AddCombatantCard(combatantStateChange);
                    continue;
                }
            
                UpdateCombatantCard(combatantStateChange);
            }
        }
        
        internal CombatantTracker GetCombatantTracker(CombatantCard combatantCard)
        {
            foreach (CombatantTracker combatantCardsValue in _combatantCards.Values)
            {
                if (combatantCardsValue.CombatantCard.Information == combatantCard.Information)
                {
                    return combatantCardsValue;
                }
            }

            throw new ArgumentException("No tracker found!!!!!!!");
        }
        
        internal static void PrintStateChanges(CombatantStateChange[] combatantStateChanges)
        {
            System.Console.WriteLine("\n--0-- Combatant State Changes -----\n");
            
            foreach (CombatantStateChange combatantStateChange in combatantStateChanges)
            {
                System.Console.WriteLine(combatantStateChange.ToString());
            }
            
            System.Console.WriteLine("\n--0--\n");
        }

        private void AddCombatantCard(CombatantStateChange combatantStateChange)
        { 
            CombatantTracker combatantTracker = new(combatantStateChange.CombatantCard);
            _combatantCards.Add(combatantStateChange.CombatantID, combatantTracker);
            
            UpdateCombatantCard(combatantStateChange);
        }

        private void UpdateCombatantCard(CombatantStateChange combatantStateChange)
        {
            _combatantCards[combatantStateChange.CombatantID].CombatantCard = combatantStateChange.CombatantCard;

            if (_combatantCards.ContainsKey(combatantStateChange.AttackerID) == false)
            {
                _combatantCards.Add(combatantStateChange.AttackerID, new CombatantTracker(1));
            }
            else
            {
                _combatantCards[combatantStateChange.AttackerID].TotalAttacks++;
            }
            
            if (combatantStateChange.IsAlive)
            {
                return;
            }

            if (FirstDeadCombatant == default)
            {
                FirstDeadCombatant = combatantStateChange;
            }
        }
    }
}
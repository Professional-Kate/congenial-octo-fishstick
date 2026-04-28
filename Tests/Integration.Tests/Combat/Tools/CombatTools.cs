using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Response;

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
        
        internal CombatantTracker GetCombatantTracker(CombatantCreation combatantCreation)
        {
            // TODO: if a Combatant is not attacked they will not be added to the _combatantCards. 
            foreach (CombatantTracker combatantTracker in _combatantCards.Values)
            {
                if (combatantTracker.CombatantCreation.Information == combatantCreation.Information)
                {
                    return combatantTracker;
                }
            }
            
            return new CombatantTracker(combatantCreation);
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
        
        internal static void AssertFirstDeadCombatant(CombatantCreation firstDeadCombatant, CombatantCreation expectedFirstDead)
        { 
            Assert.That(firstDeadCombatant.Information, Is.EqualTo(expectedFirstDead.Information));
        }
        
        internal void AssertZeroAttacks(params CombatantCreation[] combatantCards)
        {
            foreach (CombatantCreation combatantCard in combatantCards)
            {
                CombatantTracker tracker = GetCombatantTracker(combatantCard);
                Assert.That(tracker.TotalAttacks, Is.EqualTo(0));
            }
        }

        internal void AssertOneOrMoreAttacks(params CombatantCreation[] combatantCards)
        {
            foreach (CombatantCreation combatantCard in combatantCards)
            {
                CombatantTracker tracker = GetCombatantTracker(combatantCard);
                Assert.That(tracker.TotalAttacks, Is.GreaterThanOrEqualTo(1));
            }
        }

        internal static void AssertVictory(BasicEncounterDeckResponse basicEncounterDeckResponse, bool friendlyVictory)
        { 
            Assert.That(basicEncounterDeckResponse.FriendlyVictory, Is.EqualTo(friendlyVictory));
        }

        private void AddCombatantCard(CombatantStateChange combatantStateChange)
        { 
            CombatantTracker combatantTracker = new(combatantStateChange.CombatantCreation);
            _combatantCards.Add(combatantStateChange.CombatantID, combatantTracker);
            
            UpdateCombatantCard(combatantStateChange);
        }

        private void UpdateCombatantCard(CombatantStateChange combatantStateChange)
        {
            _combatantCards[combatantStateChange.CombatantID].CombatantCreation = combatantStateChange.CombatantCreation;

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
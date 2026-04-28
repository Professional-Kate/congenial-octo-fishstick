using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Response;

namespace IdelPog.Integration.Tests.Combat.Tools
{
    internal sealed class CombatTools
    {
        private readonly Dictionary<byte, CombatantTracker> _combatantTrackers = new();
        internal CombatantStateChange FirstDeadCombatant { get; private set; }

        /// <summary>
        /// Should be called from a setup or teardown method
        /// </summary>
        internal void Reset()
        {
            _combatantTrackers.Clear();
            FirstDeadCombatant = default;
        }

        /// <summary>
        /// Will update each property of this class
        /// </summary>
        /// <param name="combatantStateChanges">The full array of state changes</param>
        /// <param name="combatantCreationResponses">Used to seed the <see cref="CombatantTracker"/> collection</param>
        internal void RegisterChanges(CombatantStateChange[] combatantStateChanges, CombatantCreationResponse[] combatantCreationResponses)
        {
            foreach (CombatantCreationResponse response in combatantCreationResponses)
            {
                CombatantCreation creation = new() { CombatantType = response.CombatantType, Information = response.Information, StatCard = response.StatCard };
                _combatantTrackers.Add(response.CombatantID, new CombatantTracker(creation));
            }

            foreach (CombatantStateChange combatantStateChange in combatantStateChanges)
            {
                if (_combatantTrackers.ContainsKey(combatantStateChange.CombatantID) == false)
                {
                    AddCombatantCard(combatantStateChange);
                    continue;
                }
            
                UpdateCombatantCard(combatantStateChange);
            }
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
        
        private CombatantTracker GetCombatantTracker(CombatantCreation combatantCreation)
        {
            foreach (CombatantTracker combatantTracker in _combatantTrackers.Values)
            {
                if (combatantTracker.CombatantCreation.Information == combatantCreation.Information)
                {
                    return combatantTracker;
                }
            }
            
            return new CombatantTracker(combatantCreation);
        }

        private void AddCombatantCard(CombatantStateChange combatantStateChange)
        { 
            CombatantTracker combatantTracker = new(combatantStateChange.CombatantCreation);
            _combatantTrackers.Add(combatantStateChange.CombatantID, combatantTracker);
            
            UpdateCombatantCard(combatantStateChange);
        }

        private void UpdateCombatantCard(CombatantStateChange combatantStateChange)
        {
            _combatantTrackers[combatantStateChange.CombatantID].CombatantCreation = combatantStateChange.CombatantCreation;

            if (_combatantTrackers.TryGetValue(combatantStateChange.AttackerID, out CombatantTracker? card) == false)
            {
                _combatantTrackers.Add(combatantStateChange.AttackerID, new CombatantTracker(1));
            }
            else
            {
                card.TotalAttacks++;
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
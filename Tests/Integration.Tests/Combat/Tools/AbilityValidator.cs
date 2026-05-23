using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Ability;

namespace IdelPog.Integration.Tests.Combat.Tools
{
    internal static class AbilityValidator
    {
        private static List<CombatantStateChange> _stateChanges = [];
        private static List<CombatantStateChange>.Enumerator _enumerator;
        
        internal static void Reset()
        { 
            _stateChanges.Clear();
            _enumerator = default;
        }

        internal static void RegisterChanges(CombatantStateChange[] stateChanges)
        { 
            _stateChanges = [.. stateChanges];
            
            _enumerator = _stateChanges.GetEnumerator();
        }

        internal static void AssertAttacker(byte attackingCombatantID)
        { 
            MoveNext();
            CombatantStateChange stateChange = _enumerator.Current;

            Assert.That(stateChange.AttackingCombatant.CombatantID, Is.EqualTo(attackingCombatantID));
        }
        
        internal static void AssertAttackers(params byte[] attackingCombatantIDs)
        { 
            foreach (byte attackingCombatantID in attackingCombatantIDs)
            {
                AssertAttacker(attackingCombatantID);
            }
        }

        internal static void AssertAttackerAbility(byte attackingCombatantID, AbilityType abilityType)
        {
            MoveNext();
            CombatantStateChange stateChange = _enumerator.Current;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(stateChange.AttackingCombatant.CombatantID, Is.EqualTo(attackingCombatantID));
                Assert.That(stateChange.AttackingCombatant.AbilityType, Is.EqualTo(abilityType));
            }
        }

        private static void MoveNext()
        {
            Assert.That(_enumerator, Is.Not.Default, "Someone hasn't called AbilityValidator.RegisterChanges............. It's you. Do that.");
            Assert.That(_enumerator.MoveNext(), Is.True, "Expected another CombatantStateChange, you fool.");
        }
    }
}
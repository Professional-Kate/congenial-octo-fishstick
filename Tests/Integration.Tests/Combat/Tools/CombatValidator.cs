using System.Collections.Immutable;
using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Response;

namespace IdelPog.Integration.Tests.Combat.Tools
{
    internal static class CombatValidator
    {
        private static ReadOnlyCombatant _firstDeadCombatant; 
        private static List<CombatStage> _stateChanges = [];
        private static readonly Dictionary<byte, int> _attacksByCombatantID = [];
        private static List<CombatStage>.Enumerator _enumerator;
            
        /// <summary>
        /// should be called in the TearDown method!!!
        /// </summary>
        internal static void Reset()
        { 
            _stateChanges.Clear();
            _attacksByCombatantID.Clear();
            _enumerator = default;
            _firstDeadCombatant = default;
        }
        
        /// <summary>
        /// Must be called before any AssertNext method is called!!
        /// </summary>
        /// <param name="combatStages"></param>
        internal static void RegisterCombatStages(CombatStage[] combatStages)
        {
            RegisterChanges(combatStages);
            foreach (CombatStage combatStage in combatStages)
            {
                byte initiatingCombatantID = combatStage.InitiatingCombatant.CombatantID;
                if (_attacksByCombatantID.TryAdd(initiatingCombatantID, 1) == false)
                {
                    _attacksByCombatantID[initiatingCombatantID]++;
                }
                
                if (_firstDeadCombatant != default)
                {
                    return;
                }
                
                SetFirstDeadCombatant(combatStage.CombatantStateChanges);
            }
        }
        
        internal static void PrintCombatStages(CombatStage[] combatStages)
        {
            System.Console.WriteLine("\n--0-- Combatant State Changes -----\n");
            
            foreach (CombatStage combatStage in combatStages)
            {
                System.Console.WriteLine($"\nInitiatingCombatant:\n -> {combatStage.InitiatingCombatant}");
                
                System.Console.WriteLine($"AbilityID: {combatStage.AbilityID}");
                foreach (CombatantStateChange stateChange in combatStage.CombatantStateChanges)
                {
                    System.Console.WriteLine($"Tick: {stateChange.Tick}");
                    System.Console.WriteLine($"AbilityStage:\n -> {stateChange.ReadOnlyAbilityStage}");
                    
                    System.Console.WriteLine($"Target Combatants:");
                    foreach (ReadOnlyCombatant targetCombatant in stateChange.TargetCombatants)
                    {
                        System.Console.WriteLine($" -> {targetCombatant}");
                    }
                }
            }
            
            System.Console.WriteLine("\n--0--\n");
        }
        
        internal static void AssertVictory(BasicEncounterDeckResponse basicEncounterDeckResponse, bool friendlyVictory)
        { 
            Assert.That(basicEncounterDeckResponse.FriendlyVictory, Is.EqualTo(friendlyVictory));
        }
        
        internal static void AssertFirstDeadCombatant(byte expectedCombatantID)
        { 
            Assert.That(_firstDeadCombatant.CombatantID, Is.EqualTo(expectedCombatantID));
        }

        internal static void AssertCombatantDidNotAttack(params byte[] combatantIDs)
        {
            foreach (byte combatantID in combatantIDs)
            {
                Assert.That(_attacksByCombatantID.ContainsKey(combatantID), Is.False);
            }
        }

        internal static void AssertCombatantDidAttack(params byte[] combatantIDs)
        {
            foreach (byte combatantID in combatantIDs)
            {
                Assert.That(_attacksByCombatantID.ContainsKey(combatantID), Is.True);
            }
        }

        internal static CombatStage GetCombatStage()
        {
            MoveNext();
            return _enumerator.Current;
        }
        
        /// <summary>
        /// Flattens each <see cref="CombatantStateChange"/>, in the next <see cref="CombatStage"/>, into a byte[] to compare with <paramref name="expectedTargetCombatants"/>
        /// </summary>
        /// <param name="expectedTargetCombatants"></param>
        internal static void AssertNextTargets(params byte[] expectedTargetCombatants)
        {
            MoveNext();
            CombatStage combatStage = _enumerator.Current;

            List<byte> targetCombatantIDs = [];
            foreach (CombatantStateChange combatantStateChange in combatStage.CombatantStateChanges)
            {
                foreach (ReadOnlyCombatant readOnlyCombatant in combatantStateChange.TargetCombatants)
                {
                    targetCombatantIDs.Add(readOnlyCombatant.CombatantID);
                }
            }
            
            Assert.That(targetCombatantIDs, Is.EqualTo(expectedTargetCombatants));
        }

        internal static void AssertNextInitiatingCombatant(params byte[] initiatingCombatantIDs)
        {
            foreach (byte initiatingCombatantID in initiatingCombatantIDs)
            {
                MoveNext();
                CombatStage combatStage = _enumerator.Current;
            
                Assert.That(combatStage.InitiatingCombatant.CombatantID, Is.EqualTo(initiatingCombatantID));
            }
        }

        internal static void AssertNextInitiatingInstanceID(params byte[] initiatingCombatantIDs)
        {
            foreach (byte initiatingCombatantID in initiatingCombatantIDs)
            {
                MoveNext();
                CombatStage combatStage = _enumerator.Current;
            
                Assert.That(combatStage.InitiatingCombatant.InstanceID, Is.EqualTo(initiatingCombatantID));
            }
        }

        internal static void AssertNextAbilityID(params byte[] expectedAbilityIDs)
        {
            foreach (byte expectedAbilityID in expectedAbilityIDs)
            {
                MoveNext();
                CombatStage combatStage = _enumerator.Current;
                
                Assert.That(combatStage.AbilityID, Is.EqualTo(expectedAbilityID));
            }
        }

        internal static void AssertAbilityNeverUsed(byte abilityID)
        {
            while (_enumerator.MoveNext())
            {
                CombatStage combatStage = _enumerator.Current;
                Assert.That(combatStage.AbilityID, Is.Not.EqualTo(abilityID));
            }
        }

        private static void RegisterChanges(CombatStage[] stateChanges)
        { 
            _stateChanges = [..stateChanges];
            
            _enumerator = _stateChanges.GetEnumerator();
        }

        private static void SetFirstDeadCombatant(ImmutableArray<CombatantStateChange> combatantStateChanges)
        {
            foreach (CombatantStateChange combatantStateChange in combatantStateChanges)
            {
                foreach (ReadOnlyCombatant readOnlyCombatant in combatantStateChange.TargetCombatants)
                {
                    // is not alive
                    if (readOnlyCombatant.IsAlive)
                    {
                        continue;
                    }

                    _firstDeadCombatant = readOnlyCombatant;
                    return;
                }
            }
        }
        
        private static void MoveNext()
        {
            Assert.That(_enumerator, Is.Not.Default, "Someone hasn't called CombatValidator.RegisterCombatStages............. It's you. Do that.");
            Assert.That(_enumerator.MoveNext(), Is.True, "Expected another CombatStage, you fool.");
        }
    }
}
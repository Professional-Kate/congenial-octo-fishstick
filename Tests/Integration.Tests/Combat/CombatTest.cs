using IdelPog.Combat;
using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Deck;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Integration.Tests.Combat
{
    [TestFixture]
    public sealed class CombatTest
    {
        private ICombatService _combatService;
        private BasicEncounterDeck _basicEncounterDeck;

        private CombatantCard _humanCard;
        private CombatantCard _goblinCard;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _humanCard = new CombatantCard
            {
                CombatantType = CombatantType.HUMAN, 
                IsFriendly = true, 
                StatCard = new StatCard { Health = 90, Attack = 5, Speed = 5 },
                TargetingType = TargetingType.LOW_HEALTH
            };
            
            _goblinCard = new CombatantCard
            {
                CombatantType = CombatantType.GOBLIN, 
                IsFriendly = false, 
                StatCard = new StatCard { Health = 9, Attack = 2, Speed = 10 },
                TargetingType = TargetingType.LOW_HEALTH
            };
            
            _basicEncounterDeck = new BasicEncounterDeck
            {
                FriendlyCombatantCards = [_humanCard],
                EnemyCombatantCards = [_goblinCard]
            };
        }
        
        [SetUp]
        public void Setup()
        { 
            _combatService = CombatBootstrapper.SetupCombat();
        }

        private static void TranslateEventLog(CombatEventLog combatEventLog)
        {
            string attacker = combatEventLog.AttackerID == 0 ? "Human" : "Goblin";
            string defender = combatEventLog.DefenderID == 0 ? "Human" : "Goblin";
            
            CombatantStatsComponent attackerStats = combatEventLog.AttackerStats;
            CombatantStatsComponent defenderStats = combatEventLog.DefenderStats;
            
            System.Console.WriteLine($"-> The {attacker} ({combatEventLog.AttackerID}) attacks the {defender} ({combatEventLog.DefenderID}) for {attackerStats.Attack} damage!");

            if (defenderStats.Health == 0)
            {
                System.Console.WriteLine($"--> The {defender} ({combatEventLog.DefenderID}) has died! Killed by the {attacker} ({combatEventLog.AttackerID})!");
                return;
            }
            System.Console.WriteLine($"--> The {defender} ({combatEventLog.DefenderID}) has {defenderStats.Health} health remaining...");
        }

        [Test]
        public void Positive_SimulateCombat_FriendlyVictory()
        { 
            EncounterResponse encounterResponse = _combatService.RunEncounter(_basicEncounterDeck);
            
            ICombatLogReader logReader = encounterResponse.CombatLogReader;
            while (logReader.NextCombatState())
            {
                CombatEventLog combatEventLog = logReader.CurrentCombatState;
                TranslateEventLog(combatEventLog);
            }
            
            logReader.Dispose();
        }
    }
}
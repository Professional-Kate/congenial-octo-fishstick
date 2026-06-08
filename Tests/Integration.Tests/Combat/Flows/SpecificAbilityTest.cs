using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Event;
using IdelPog.Core.Contracts;
using IdelPog.Integration.Tests.Combat.Tools;

namespace IdelPog.Integration.Tests.Combat.Flows
{
    [TestFixture]
    public sealed class SpecificAbilityTest : ManagedTestBuffer
    {
        private ManagedResponseListener<BasicEncounterDeckResponse> _responseListener;
        
        [SetUp]
        public void Setup()
        {
            _responseListener = new ManagedResponseListener<BasicEncounterDeckResponse>();
            
            ManagedSubscribe(_responseListener);
            AbilityValidator.Reset();
        }
        
        private void RunCombat(byte[] friendlyCombatantIDs, byte[] enemyCombatantIDs)
        {
            BasicEncounterDeck basicEncounterDeck = new()
            {
                FriendlyCombatantIDs = friendlyCombatantIDs,
                EnemyCombatantIDs = enemyCombatantIDs
            };
            
            DispatchMessage(basicEncounterDeck);
            
            _responseListener.AssertWasCalled(true);
            _responseListener.AssertResponseLength(1);
            AbilityValidator.RegisterChanges(_responseListener.Responses[0].CombatantStateChanges);
        }

        private static uint GetExpectedDamage(AbilityCreation abilityCreation)
        {
            ElementalDamageCard elementalDamageCard = abilityCreation.ElementalDamageCard;
            PhysicalDamageCard physicalDamageCard = abilityCreation.PhysicalDamageCard;
            
            uint elementalDamage = elementalDamageCard.ColdDamage + elementalDamageCard.FireDamage + elementalDamageCard.LightningDamage; 
            uint physicalDamage = physicalDamageCard.SlashDamage + physicalDamageCard.StrikeDamage + physicalDamageCard.ThrustDamage;

            return physicalDamage + elementalDamage;
        }

        private static void AssertDamageDealt(AbilityCreation abilityCreation, byte targetID = 1)
        {
            CombatantStateChange combatantStateChange = AbilityValidator.GetStateChange();
            AttackingCombatant attackingCombatant = combatantStateChange.AttackingCombatant;
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(attackingCombatant.AbilityType, Is.EqualTo(abilityCreation.AbilityCard.AbilityType));
                Assert.That(attackingCombatant.DamageDealt, Is.EqualTo(GetExpectedDamage(abilityCreation)));
                Assert.That(combatantStateChange.CombatantID, Is.EqualTo(targetID));
            }
        }
        
        [Test]
        public void SlashAttack_DamagesEnemy()
        {
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.WolfCreation);
            DispatchMessage(StaticCombatCommands.SlashAttackCreation);
            DispatchMessage(StaticCombatCommands.EquipSlashAttack(0));
            
            RunCombat([0], [1]);

            AssertDamageDealt(StaticCombatCommands.SlashAttackCreation);
        }

        [Test]
        public void StabAttack_DamagesEnemy()
        {
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.WolfCreation);
            DispatchMessage(StaticCombatCommands.StabAttackCreation);
            DispatchMessage(StaticCombatCommands.EquipStabAttack(1));
            
            RunCombat([0], [1]);

            AssertDamageDealt(StaticCombatCommands.StabAttackCreation, 0);
        }

        [Test]
        public void StrikeAttack_DamagesEnemy()
        {
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.WolfCreation);
            DispatchMessage(StaticCombatCommands.StrikeAttackCreation);
            DispatchMessage(StaticCombatCommands.EquipStrikeAttack(0));
            
            RunCombat([0], [1]);

            AssertDamageDealt(StaticCombatCommands.StrikeAttackCreation);
        }

        [Test]
        public void FireLance_DamagesEnemy()
        {
            AbilityCreation fireLanceCreation = new()
            {
                AbilityCard = new AbilityCard {  AbilityType = AbilityType.FIRE_LANCE, EventType = EventType.DIRECT_DAMAGE, Cooldown = 15, AbilitySlots = 2, CastTime = 5 },
                ElementalDamageCard = new ElementalDamageCard { ColdDamage = 0, LightningDamage = 0, FireDamage = 10 },
                PhysicalDamageCard = new PhysicalDamageCard { SlashDamage = 0, StrikeDamage = 0, ThrustDamage = 3 },
                Information = new Information { Name = "Fire Lance!", Description = "BURNS AA" }
            };
            
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.WolfCreation);
            DispatchMessage(fireLanceCreation);
            DispatchMessage(StaticCombatCommands.EquipAbility(0, AbilityType.FIRE_LANCE));
            
            RunCombat([0], [1]);

            AssertDamageDealt(fireLanceCreation);
        }
        
        [Test]
        public void ColdLance_DamagesEnemy()
        {
            AbilityCreation coldLanceCreation = new()
            {
                AbilityCard = new AbilityCard {  AbilityType = AbilityType.COLD_LANCE, EventType = EventType.DIRECT_DAMAGE, Cooldown = 15, AbilitySlots = 2, CastTime = 5 },
                ElementalDamageCard = new ElementalDamageCard { ColdDamage = 10, LightningDamage = 0, FireDamage = 0 },
                PhysicalDamageCard = new PhysicalDamageCard { SlashDamage = 0, StrikeDamage = 0, ThrustDamage = 3 },
                Information = new Information { Name = "Cold Lance!", Description = "It is FREEZING cold" }
            };
            
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.WolfCreation);
            DispatchMessage(coldLanceCreation);
            DispatchMessage(StaticCombatCommands.EquipAbility(0, AbilityType.COLD_LANCE));
            
            RunCombat([0], [1]);

            AssertDamageDealt(coldLanceCreation);
        }
        
        [Test]
        public void LightningLance_DamagesEnemy()
        {
            AbilityCreation lightningLanceCreation = new()
            {
                AbilityCard = new AbilityCard {  AbilityType = AbilityType.LIGHTNING_LANCE, EventType = EventType.DIRECT_DAMAGE, Cooldown = 15, AbilitySlots = 2, CastTime = 5 },
                ElementalDamageCard = new ElementalDamageCard { ColdDamage = 0, LightningDamage = 10, FireDamage = 0 },
                PhysicalDamageCard = new PhysicalDamageCard { SlashDamage = 0, StrikeDamage = 0, ThrustDamage = 3 },
                Information = new Information { Name = "Lightning Lance!", Description = "Why is it not a bolt?" }
            };
            
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.WolfCreation);
            DispatchMessage(lightningLanceCreation);
            DispatchMessage(StaticCombatCommands.EquipAbility(0, AbilityType.LIGHTNING_LANCE));
            
            RunCombat([0], [1]);

            AssertDamageDealt(lightningLanceCreation);
        }

        [Test]
        public void MinorHeal_HealsFriendlyTarget()
        {
            AbilityCreation minorHealCreation = new()
            {
                AbilityCard = new AbilityCard {  AbilityType = AbilityType.MINOR_HEAL, EventType = EventType.HEALING, Cooldown = 30, AbilitySlots = 1, CastTime = 10 },
                ElementalDamageCard = new ElementalDamageCard { ColdDamage = 0, LightningDamage = 0, FireDamage = 0 },
                PhysicalDamageCard = new PhysicalDamageCard { SlashDamage = 0, StrikeDamage = 0, ThrustDamage = 0 },
                Information = new Information { Name = "Minor Heal!", Description = "Heals only minor wounds!" }
            };
            
            DispatchMessage(StaticCombatCommands.HumanCreation, StaticCombatCommands.WolfCreation);
            DispatchMessage(minorHealCreation);
            DispatchMessage(StaticCombatCommands.EquipAbility(0, AbilityType.MINOR_HEAL));
            
            RunCombat([0], [1]);

            AssertDamageDealt(minorHealCreation);
        }
    }
}
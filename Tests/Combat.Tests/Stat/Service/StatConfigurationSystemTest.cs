using System.Collections.Immutable;
using IdelPog.Combat.Ability.Contracts;
using IdelPog.Combat.Combatant.Contracts;
using IdelPog.Combat.Stat.Contracts;
using IdelPog.Combat.Stat.Contracts.Command;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Service;

namespace IdelPog.Combat.Tests.Stat.Service
{
    public sealed class StatConfigurationSystemTest
    {
        private StatConfigurationSystem _statConfigurationSystem;

        private readonly StatConfiguration _statConfiguration = new()
        {
            CombatantStatLinks = new CombatantStatLinks
            {
                BaseHealthID = 13,
                HealthID = 22,
                SpeedID = 4,
                InitiativeID = 0
            },
            AbilityStatLinks = new AbilityStatLinks
            {
                AbilityDamageID = 4,
                AbilityHealingID = 5,
                RetaliationDamageID = 98,
                CastTimeID = 1,
                CooldownID = 23,
                AbilitySlotsID = 12
            }
        };
        
        [SetUp]
        public void Setup()
        { 
            _statConfigurationSystem = new StatConfigurationSystem();
        }

        private static void VerifyGetStatBindings(ImmutableArray<StatBinding> statBindings, StatConfiguration statConfiguration)
        {
            foreach (StatBinding statBinding in statBindings)
            {
                byte expectedID = statBinding.StatType switch
                {
                    StatType.BASE_HEALTH => statConfiguration.CombatantStatLinks.BaseHealthID,
                    StatType.HEALTH => statConfiguration.CombatantStatLinks.HealthID,
                    StatType.SPEED => statConfiguration.CombatantStatLinks.SpeedID,
                    StatType.INITIATIVE => statConfiguration.CombatantStatLinks.InitiativeID,
                    StatType.ABILITY_DAMAGE => statConfiguration.AbilityStatLinks.AbilityDamageID,
                    StatType.ABILITY_HEALING => statConfiguration.AbilityStatLinks.AbilityHealingID,
                    StatType.RETALIATION_DAMAGE => statConfiguration.AbilityStatLinks.RetaliationDamageID,
                    StatType.CAST_TIME => statConfiguration.AbilityStatLinks.CastTimeID,
                    StatType.COOLDOWN => statConfiguration.AbilityStatLinks.CooldownID,
                    StatType.ABILITY_SLOTS => statConfiguration.AbilityStatLinks.AbilitySlotsID,
                    _ => throw new ArgumentOutOfRangeException()
                };

                Assert.That(statBinding.LinkedID, Is.EqualTo(expectedID));
            }
        }

        [Test]
        public void Positive_SetStatConfiguration_UpdatesConfiguration()
        { 
            _statConfigurationSystem.SetStatConfiguration(_statConfiguration);
            
            VerifyGetStatBindings(_statConfigurationSystem.GetStatBindings(), _statConfiguration);
        }

        [Test]
        public void Positive_SetStatConfiguration_MultipleCalls_UpdatesConfiguration()
        {
            _statConfigurationSystem.SetStatConfiguration(_statConfiguration);
            VerifyGetStatBindings(_statConfigurationSystem.GetStatBindings(), _statConfiguration);

            CombatantStatLinks combatantStatLinks = new()
            {
                BaseHealthID = 194,
                HealthID = 130,
                SpeedID = 209,
                InitiativeID = 234
            };
            
            _statConfigurationSystem.SetStatConfiguration(_statConfiguration with { CombatantStatLinks = combatantStatLinks });
            VerifyGetStatBindings(_statConfigurationSystem.GetStatBindings(), _statConfiguration with { CombatantStatLinks = combatantStatLinks });
        }

        [Test]
        public void Positive_GetStatBindings_ReturnsImmutableCollection()
        {
            _statConfigurationSystem.SetStatConfiguration(_statConfiguration);
            
            ImmutableArray<StatBinding> statBindings = _statConfigurationSystem.GetStatBindings();
            
            CombatantStatLinks combatantStatLinks = new()
            {
                BaseHealthID = 194,
                HealthID = 130,
                SpeedID = 209,
                InitiativeID = 234
            };
            _statConfigurationSystem.SetStatConfiguration(_statConfiguration with { CombatantStatLinks = combatantStatLinks });
            Assert.That(statBindings, Is.Not.EqualTo(_statConfigurationSystem.GetStatBindings()));
        }

        [Test]
        public void Positive_GetStatID_ReturnsExpectedID()
        {
            _statConfigurationSystem.SetStatConfiguration(_statConfiguration);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(_statConfigurationSystem.GetStatID(StatType.COOLDOWN), Is.EqualTo(_statConfiguration.AbilityStatLinks.CooldownID));
                Assert.That(_statConfigurationSystem.GetStatID(StatType.HEALTH), Is.EqualTo(_statConfiguration.CombatantStatLinks.HealthID));
            }
        }

        [Test]
        public void Positive_GetStatID_ThenSetStatConfiguration_ShouldReturnDifferentID()
        {
            _statConfigurationSystem.SetStatConfiguration(_statConfiguration);
            
            Assert.That(_statConfigurationSystem.GetStatID(StatType.HEALTH), Is.EqualTo(_statConfiguration.CombatantStatLinks.HealthID));
            
            CombatantStatLinks combatantStatLinks = new()
            {
                BaseHealthID = 194,
                HealthID = 130,
                SpeedID = 209,
                InitiativeID = 234
            };
            _statConfigurationSystem.SetStatConfiguration(_statConfiguration with { CombatantStatLinks = combatantStatLinks });
            
            Assert.That(_statConfigurationSystem.GetStatID(StatType.HEALTH), Is.EqualTo(combatantStatLinks.HealthID));
        }
    }
}
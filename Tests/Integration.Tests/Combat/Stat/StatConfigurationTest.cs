using IdelPog.Combat.Ability.Contracts;
using IdelPog.Combat.Combatant.Contracts;
using IdelPog.Combat.Stat.Contracts;
using IdelPog.Combat.Stat.Contracts.Command;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Contracts.Response;
using IdelPog.Core.Contracts;

namespace IdelPog.Integration.Tests.Combat.Stat
{
    [TestFixture]
    public class StatConfigurationTest : ManagedTestBuffer
    {
        private ManagedResponseListener<StatConfigurationResponse> _responseListener;
        private ManagedErrorListener<BufferedError<StatConfiguration>> _errorListener;
        
        private readonly StatConfiguration _statConfiguration = new()
        {
            CombatantStatLinks = new CombatantStatLinks
            {
                BaseHealthID = 0,
                HealthID = 1,
                SpeedID = 2,
                InitiativeID = 3
            },
            AbilityStatLinks = new AbilityStatLinks
            {
                AbilityDamageID = 4,
                AbilityHealingID = 5,
                RetaliationDamageID = 6,
                CastTimeID = 7,
                CooldownID = 8,
                AbilitySlotsID = 9
            }
        };
        
        [SetUp]
        public void Setup()
        {
            _responseListener = new ManagedResponseListener<StatConfigurationResponse>();
            _errorListener = new ManagedErrorListener<BufferedError<StatConfiguration>>();
            
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
        }
        
        private static void AssertResponse(StatConfigurationResponse response, StatConfiguration source)
        {
            Assert.That(response.StatConfiguration, Is.EqualTo(source));
            foreach (StatBinding statBinding in response.StatBindings)
            {
                bool matched = statBinding.StatType switch
                {
                    StatType.HEALTH => statBinding.LinkedID == source.CombatantStatLinks.HealthID,
                    StatType.BASE_HEALTH => statBinding.LinkedID == source.CombatantStatLinks.BaseHealthID,
                    StatType.SPEED => statBinding.LinkedID == source.CombatantStatLinks.SpeedID,
                    StatType.INITIATIVE => statBinding.LinkedID == source.CombatantStatLinks.InitiativeID,
                    StatType.ABILITY_DAMAGE => statBinding.LinkedID == source.AbilityStatLinks.AbilityDamageID,
                    StatType.ABILITY_HEALING => statBinding.LinkedID == source.AbilityStatLinks.AbilityHealingID,
                    StatType.RETALIATION_DAMAGE => statBinding.LinkedID == source.AbilityStatLinks.RetaliationDamageID,
                    StatType.CAST_TIME => statBinding.LinkedID == source.AbilityStatLinks.CastTimeID,
                    StatType.COOLDOWN => statBinding.LinkedID == source.AbilityStatLinks.CooldownID,
                    StatType.ABILITY_SLOTS => statBinding.LinkedID == source.AbilityStatLinks.AbilitySlotsID,
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                Assert.That(matched, Is.True);
            }
        }
        
        [Test]
        public void Positive_DispatchMessage_ChangesStatBindings()
        {
            DispatchMessage(_statConfiguration);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], _statConfiguration);
        }

        [Test]
        public void Positive_DispatchMessage_MultipleMessages()
        {
            StatConfiguration differentStatConfig = new()
            {
                CombatantStatLinks = new CombatantStatLinks
                {
                    BaseHealthID = 4,
                    HealthID = 15,
                    SpeedID = 5,
                    InitiativeID = 7
                },
                AbilityStatLinks = new AbilityStatLinks
                {
                    AbilityDamageID = 90,
                    AbilityHealingID = 54,
                    RetaliationDamageID = 12,
                    CastTimeID = 9,
                    CooldownID = 10,
                    AbilitySlotsID = 25
                }
            };
            
            DispatchMessage(_statConfiguration, differentStatConfig);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(2);
            AssertResponse(_responseListener.Responses[0], _statConfiguration);
            AssertResponse(_responseListener.Responses[1], differentStatConfig);
        }
    }
}
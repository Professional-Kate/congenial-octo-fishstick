using System.Collections.Immutable;
using IdelPog.Combat.Stat.Contracts;
using IdelPog.Combat.Stat.Contracts.Command;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Service.Interface;

namespace IdelPog.Combat.Stat.Service
{
    public sealed class StatConfigurationSystem : IStatConfigurationSetter, IStatConfigurationGetter
    {
        private readonly Dictionary<StatType, byte> _statTypeLookup = [];
        
        public void SetStatConfiguration(StatConfiguration statConfiguration)
        {
            _statTypeLookup[StatType.BASE_HEALTH] = statConfiguration.CombatantStatLinks.BaseHealthID;
            _statTypeLookup[StatType.HEALTH] = statConfiguration.CombatantStatLinks.HealthID;
            _statTypeLookup[StatType.SPEED] = statConfiguration.CombatantStatLinks.SpeedID;
            _statTypeLookup[StatType.INITIATIVE] = statConfiguration.CombatantStatLinks.InitiativeID;
            
            _statTypeLookup[StatType.ABILITY_DAMAGE] = statConfiguration.AbilityStatLinks.AbilityDamageID;
            _statTypeLookup[StatType.ABILITY_HEALING] = statConfiguration.AbilityStatLinks.AbilityHealingID;
            _statTypeLookup[StatType.RETALIATION_DAMAGE] = statConfiguration.AbilityStatLinks.RetaliationDamageID;
            _statTypeLookup[StatType.CAST_TIME] = statConfiguration.AbilityStatLinks.CastTimeID;
            _statTypeLookup[StatType.COOLDOWN] = statConfiguration.AbilityStatLinks.CooldownID;
            _statTypeLookup[StatType.ABILITY_SLOTS] = statConfiguration.AbilityStatLinks.AbilitySlotsID;
        }

        public ImmutableArray<StatBinding> GetStatBindings()
        {
            List<StatBinding> statBindings = new(_statTypeLookup.Count);
            foreach (KeyValuePair<StatType, byte> keyValuePair in _statTypeLookup)
            {
                statBindings.Add(new StatBinding { StatType = keyValuePair.Key, LinkedID = keyValuePair.Value });
            }

            return [..statBindings];
        }

        public byte GetStatID(StatType statType)
        {
            return _statTypeLookup[statType];
        }
    }
}
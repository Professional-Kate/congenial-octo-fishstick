using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Stat.Runtime.Component;

namespace IdelPog.Combat.Ability.Runtime.System.Interface
{
    public interface  IAbilityStatsFactory
    {
        public StatComponent[] CreateStats(AbilityStagesComponent abilityStagesComponent, AbilityCard abilityCard);
    }
}
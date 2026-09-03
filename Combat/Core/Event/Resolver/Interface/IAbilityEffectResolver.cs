using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Entities;

namespace IdelPog.Combat.Core.Event.Resolver.Interface
{
    public interface IAbilityEffectResolver
    {
        public void ResolveEffect(double tick, AbilityEntity abilityEntity, AbilityStage abilityStage);
    }
}
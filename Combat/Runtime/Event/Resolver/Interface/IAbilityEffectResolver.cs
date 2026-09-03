using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Entity;

namespace IdelPog.Combat.Runtime.Event.Resolver.Interface
{
    public interface IAbilityEffectResolver
    {
        public void ResolveEffect(double tick, AbilityEntity abilityEntity, AbilityStage abilityStage);
    }
}
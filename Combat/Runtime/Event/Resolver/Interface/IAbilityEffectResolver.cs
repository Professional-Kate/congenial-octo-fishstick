using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.Event.Resolver.Interface
{
    public interface IAbilityEffectResolver
    {
        public void ResolveEffect(double tick, AbilityEntity abilityEntity, AbilityStage abilityStage);
    }
}
using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Event.Resolver.Interface;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Event.Resolver
{
    public sealed class CastingEventResolver : IEventResolver
    {
        private readonly IAbilityEventScheduler _abilityEventScheduler;

        public CastingEventResolver(IAbilityEventScheduler abilityEventScheduler)
        {
            _abilityEventScheduler = abilityEventScheduler;
        }

        public void ResolveEvent(double tick, byte combatantID, AbilityType abilityType)
        {
            _abilityEventScheduler.EnqueueAbilityEvent(tick, combatantID, abilityType);
        }
    }
}
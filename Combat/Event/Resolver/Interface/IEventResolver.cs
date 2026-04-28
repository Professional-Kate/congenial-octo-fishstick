using IdelPog.Combat.Contracts.Ability;

namespace IdelPog.Combat.Event.Resolver.Interface
{
    public interface IEventResolver
    {
        public void ResolveEvent(double tick, byte combatantID, AbilityType abilityType);
    }
}
using IdelPog.Combat.Contracts.Enum;

namespace IdelPog.Combat.Event.Resolver.Interface
{
    public interface IEventResolver
    {
        public void ResolveEvent(double tick, byte combatantID, AbilityType abilityType);
    }
}
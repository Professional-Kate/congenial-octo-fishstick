using IdelPog.Combat.Contracts.Ability;

namespace IdelPog.Combat.Runtime.System.Mediator.Interface
{
    public interface IEntityDamageMediator
    {
        public void ApplyDamage(byte attackingCombatantID, AbilityType abilityType);
    }
}
using IdelPog.Combat.Combatant.Contracts.Enum;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.ECS.Entity;

namespace IdelPog.Combat.Combatant.Runtime.Entities
{
    public sealed record CombatantEntity : Entity
    {
        public required byte CombatantID { get; init; }
        public required byte InstanceID { get; init; }
        public required CombatantType CombatantType { get; init; }
        public required TargetingType TargetingType { get; init; }

        public CombatantEntity(HealthCard healthCard, AgilityCard agilityCard)
            : base(requiredComponents:
            [
                new BaseHealthComponent { Health = healthCard.BaseHealth }, new HealthComponent { Health = healthCard.Health},
                new AgilityComponent { Speed = agilityCard.Speed, Initiative = agilityCard.Initiative }, new LifeStatusComponent { IsAlive = true }
            ])
        { }
    }
}

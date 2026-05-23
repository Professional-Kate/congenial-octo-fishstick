using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.System.Factory.Interface;

namespace IdelPog.Combat.Runtime.System.Factory
{
    public sealed class AbilityEntityFactory : IAbilityEntityFactory
    {
        public AbilityEntity CreateAbilityEntity(AbilityCreation abilityCreation)
        {
            CooldownComponent cooldownComponent = new() { Cooldown = abilityCreation.Cooldown };
            DamageComponent damageComponent = CreateDamageComponent(abilityCreation.DamageCard);

            AbilityEntity abilityEntity = new(cooldownComponent, damageComponent)
            {
                AbilityType = abilityCreation.AbilityType,
                AbilitySlots = abilityCreation.AbilitySlots,
                Information = abilityCreation.Information
            };

            if (abilityCreation.CastTime != 0)
            { 
                abilityEntity.AddComponent(new CastTimeComponent { CastTime = abilityCreation.CastTime });
            }
            
            return abilityEntity;
        }

        private static DamageComponent CreateDamageComponent(DamageCard damageCard)
        {
            return new DamageComponent
            {
                PhysicalDamage = damageCard.PhysicalDamage,
                LightningDamage = damageCard.LightningDamage,
                ColdDamage = damageCard.ColdDamage,
                FireDamage = damageCard.FireDamage
            };
        }
    }
}
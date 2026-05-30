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
            ElementalDamageComponent elementalDamageComponent = CreateElementalDamageComponent(abilityCreation.ElementalDamageCard);
            PhysicalDamageComponent physicalDamageComponent = CreatePhysicalDamageComponent(abilityCreation.PhysicalDamageCard); 
            
            AbilityEntity abilityEntity = new(cooldownComponent, elementalDamageComponent, physicalDamageComponent)
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

        private static ElementalDamageComponent CreateElementalDamageComponent(ElementalDamageCard elementalDamageCard)
        {
            return new ElementalDamageComponent
            {
                LightningDamage = elementalDamageCard.LightningDamage,
                ColdDamage = elementalDamageCard.ColdDamage,
                FireDamage = elementalDamageCard.FireDamage
            };
        }
        
        private static PhysicalDamageComponent CreatePhysicalDamageComponent(PhysicalDamageCard elementalDamageCard)
        {
            return new PhysicalDamageComponent
            {
                StrikeDamage = elementalDamageCard.StrikeDamage,
                SlashDamage = elementalDamageCard.SlashDamage,
                ThrustDamage = elementalDamageCard.ThrustDamage
            };
        }
    }
}
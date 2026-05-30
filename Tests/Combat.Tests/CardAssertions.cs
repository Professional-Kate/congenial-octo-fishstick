using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Tests
{
    internal static class CardAssertions
    {
        internal static void AssertElementalDamageCard(CombatantAbilityEntity abilityEntity, ElementalDamageCard elementalDamageCard)
        {
            ElementalDamageComponent elementalDamageComponent = abilityEntity.GetComponent<ElementalDamageComponent>();
            AssertElementalDamage(elementalDamageComponent, elementalDamageCard);
        }
        
        internal static void AssertElementalDamageCard(AbilityEntity abilityEntity, ElementalDamageCard elementalDamageCard)
        {
            ElementalDamageComponent elementalDamageComponent = abilityEntity.GetComponent<ElementalDamageComponent>();
            AssertElementalDamage(elementalDamageComponent, elementalDamageCard);
        }
        
        internal static void AssertPhysicalDamageCard(CombatantAbilityEntity abilityEntity, PhysicalDamageCard physicalDamageCard)
        {
            PhysicalDamageComponent physicalDamageComponent = abilityEntity.GetComponent<PhysicalDamageComponent>();
            AssertPhysicalDamage(physicalDamageComponent, physicalDamageCard);
        }
        
        internal static void AssertPhysicalDamageCard(AbilityEntity abilityEntity, PhysicalDamageCard physicalDamageCard)
        {
            PhysicalDamageComponent physicalDamageComponent = abilityEntity.GetComponent<PhysicalDamageComponent>();
            AssertPhysicalDamage(physicalDamageComponent, physicalDamageCard);
        }

        private static void AssertPhysicalDamage(PhysicalDamageComponent physicalDamageComponent, PhysicalDamageCard physicalDamageCard)
        { 
            using (Assert.EnterMultipleScope())
            {
                Assert.That(physicalDamageComponent.SlashDamage, Is.EqualTo(physicalDamageCard.SlashDamage));
                Assert.That(physicalDamageComponent.StrikeDamage, Is.EqualTo(physicalDamageCard.StrikeDamage));
                Assert.That(physicalDamageComponent.ThrustDamage, Is.EqualTo(physicalDamageCard.ThrustDamage));
            }
        }
        
        private static void AssertElementalDamage(ElementalDamageComponent elementalDamageComponent, ElementalDamageCard elementalDamageCard)
        { 
            using (Assert.EnterMultipleScope())
            {
                Assert.That(elementalDamageComponent.FireDamage, Is.EqualTo(elementalDamageCard.FireDamage));
                Assert.That(elementalDamageComponent.ColdDamage, Is.EqualTo(elementalDamageCard.ColdDamage));
                Assert.That(elementalDamageComponent.LightningDamage, Is.EqualTo(elementalDamageCard.LightningDamage));
            }
        }
    }
}
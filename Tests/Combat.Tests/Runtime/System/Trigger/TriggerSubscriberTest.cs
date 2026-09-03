using System.Collections.Immutable;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entity;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Event.Trigger;
using IdelPog.Combat.Tests.TestFactory;

namespace IdelPog.Combat.Tests.Runtime.System.Trigger
{
    [TestFixture]
    public sealed class TriggerSubscriberTest
    {
        private TriggerSubscriber _triggerSubscriber;
        
        private AbilityEntity _abilityEntity;
        
        [SetUp]
        public void Setup()
        {
            _triggerSubscriber = new TriggerSubscriber();
            
            _abilityEntity = TestAbilityEntityFactory.Create(1, 1);
        }

        private static TriggerEventType GetTriggerEventType(AbilityEntity abilityEntity) => abilityEntity.GetComponent<TriggerComponent>().TriggerEventType;

        private void VerifyAbilityAdded(TriggerEventType triggerEventType, params byte[] abilityIDs)
        { 
            ImmutableArray<AbilityEntity> abilities = _triggerSubscriber.GetAbilities(triggerEventType);
            byte[] returnedIDs = new byte[abilities.Length];
            for (int i = 0; i < abilities.Length; i++)
            {
                AbilityEntity abilityEntity = abilities[i];
                returnedIDs[i] = abilityEntity.AbilityID;
            }
            
            Assert.That(returnedIDs, Is.EqualTo(abilityIDs));
        }

        [Test]
        public void Positive_SubscribeAbility_FirstTriggerEventType_AddsListToDictionary()
        {
            _triggerSubscriber.SubscribeAbility(_abilityEntity);
            
            VerifyAbilityAdded(GetTriggerEventType(_abilityEntity), 1);
        }

        [Test]
        public void Positive_SubscribeAbility_AlreadyContainsTriggerEventType_AddsEntityToList()
        {
            _triggerSubscriber.SubscribeAbility(_abilityEntity);
            _triggerSubscriber.SubscribeAbility(_abilityEntity);
            
            VerifyAbilityAdded(GetTriggerEventType(_abilityEntity), 1, 1);
        }

        [Test]
        public void Positive_SubscribeAbility_MultipleTriggerEventTypes()
        {
            AbilityEntity anotherAbility = TestAbilityEntityFactory.Create(2, 2);
            anotherAbility.ReplaceComponent(new TriggerComponent { TriggerEventType = TriggerEventType.COMBATANT_DEATH, MaxTriggerValue = 1, MinTriggerValue = 2, TargetingType = TargetingType.FRIENDLY });
                        
            _triggerSubscriber.SubscribeAbility(_abilityEntity);
            _triggerSubscriber.SubscribeAbility(anotherAbility);
            
            VerifyAbilityAdded(GetTriggerEventType(_abilityEntity), 1);
            VerifyAbilityAdded(GetTriggerEventType(anotherAbility), 2);
        }

        [Test]
        public void Positive_GetAbilities_ReturnsAllAbilities()
        {
            _triggerSubscriber.SubscribeAbility(_abilityEntity);
            _triggerSubscriber.SubscribeAbility(_abilityEntity with { AbilityID = 2 });
            _triggerSubscriber.SubscribeAbility(_abilityEntity with { AbilityID = 3 });
            _triggerSubscriber.SubscribeAbility(_abilityEntity with { AbilityID = 4 });

            VerifyAbilityAdded(GetTriggerEventType(_abilityEntity), 1, 2, 3, 4);
        }

        [Test]
        public void Positive_GetAbilities_NoEventTypeSubscribed_ReturnsEmptyArray()
        {
            ImmutableArray<AbilityEntity> combatantAbilityEntities = _triggerSubscriber.GetAbilities(GetTriggerEventType(_abilityEntity));
            
            Assert.That(combatantAbilityEntities, Has.Length.EqualTo(0));
        }

        [Test]
        public void Positive_GetAbilities_EventTypeSubscribed_ButNoAbilities_ReturnsEmptyArray()
        {
            ImmutableArray<AbilityEntity> combatantAbilityEntities = _triggerSubscriber.GetAbilities(GetTriggerEventType(_abilityEntity));
            
            Assert.That(combatantAbilityEntities, Has.Length.EqualTo(0));
        }
    }
}
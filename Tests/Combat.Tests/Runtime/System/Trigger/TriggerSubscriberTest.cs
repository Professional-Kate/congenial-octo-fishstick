using System.Collections.Immutable;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event.Trigger;
using IdelPog.Combat.Tests.TestFactory;

namespace IdelPog.Combat.Tests.Runtime.System.Trigger
{
    [TestFixture]
    public sealed class TriggerSubscriberTest
    {
        private CombatantAbilityEntity _combatantAbilityEntity;
        
        [SetUp]
        public void Setup()
        {
            _combatantAbilityEntity = TestCombatantAbilityEntityFactory.Create(1, 1);
        }

        private static TriggerComponent GetTriggerComponent(CombatantAbilityEntity combatantAbilityEntity) => combatantAbilityEntity.GetComponent<TriggerComponent>();

        [Test]
        public void Positive_SubscribeAbility_FirstTriggerEventType_AddsListToDictionary()
        {
            Dictionary<TriggerEventType, IList<CombatantAbilityEntity>> dictionary = [];

            TriggerSubscriber triggerSubscriber = new(dictionary);

            Assert.DoesNotThrow(() => triggerSubscriber.SubscribeAbility(_combatantAbilityEntity));

            TriggerEventType triggerEventType = GetTriggerComponent(_combatantAbilityEntity).TriggerEventType;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(dictionary, Does.ContainKey(triggerEventType));
                Assert.That(dictionary[triggerEventType], Does.Contain(_combatantAbilityEntity));
                Assert.That(dictionary[triggerEventType], Has.Count.EqualTo(1));
            }
        }

        [Test]
        public void Positive_SubscribeAbility_AlreadyContainsTriggerEventType_AddsEntityToList()
        {
            Dictionary<TriggerEventType, IList<CombatantAbilityEntity>> dictionary = [];
            
            TriggerEventType triggerEventType = GetTriggerComponent(_combatantAbilityEntity).TriggerEventType;
            dictionary.Add(triggerEventType, [_combatantAbilityEntity]);
            
            TriggerSubscriber triggerSubscriber = new(dictionary);

            Assert.DoesNotThrow(() => triggerSubscriber.SubscribeAbility(_combatantAbilityEntity));

            Assert.That(dictionary[triggerEventType], Has.Count.EqualTo(2));
        }

        [Test]
        public void Positive_SubscribeAbility_MultipleTriggerEventTypes()
        {
            Dictionary<TriggerEventType, IList<CombatantAbilityEntity>> dictionary = [];

            TriggerSubscriber triggerSubscriber = new(dictionary);

            Assert.DoesNotThrow(() => triggerSubscriber.SubscribeAbility(_combatantAbilityEntity));
            TriggerEventType triggerEventType = GetTriggerComponent(_combatantAbilityEntity).TriggerEventType;
            
            _combatantAbilityEntity.ReplaceComponent(new TriggerComponent { TriggerEventType = TriggerEventType.COMBATANT_DAMAGED, MaxTriggerValue = 1, MinTriggerValue = 2, TargetingType = TargetingType.FRIENDLY });
            Assert.DoesNotThrow(() => triggerSubscriber.SubscribeAbility(_combatantAbilityEntity));
            TriggerEventType newTriggerEventType = GetTriggerComponent(_combatantAbilityEntity).TriggerEventType;
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(dictionary, Does.ContainKey(triggerEventType));
                Assert.That(dictionary[triggerEventType], Does.Contain(_combatantAbilityEntity));
                Assert.That(dictionary[triggerEventType], Has.Count.EqualTo(1));
                Assert.That(dictionary, Does.ContainKey(newTriggerEventType));
                Assert.That(dictionary[newTriggerEventType], Does.Contain(_combatantAbilityEntity));
                Assert.That(dictionary[newTriggerEventType], Has.Count.EqualTo(1));
            }
        }

        [Test]
        public void Positive_GetAbilities_ReturnsAllAbilities()
        {
            Dictionary<TriggerEventType, IList<CombatantAbilityEntity>> dictionary = [];
            
            TriggerEventType triggerEventType = GetTriggerComponent(_combatantAbilityEntity).TriggerEventType;
            dictionary.Add(triggerEventType, [_combatantAbilityEntity, TestCombatantAbilityEntityFactory.Create(2, 2)]);
            
            TriggerSubscriber triggerSubscriber = new(dictionary);

            ImmutableArray<CombatantAbilityEntity> combatantAbilityEntities = triggerSubscriber.GetAbilities(triggerEventType);
            
            Assert.That(combatantAbilityEntities, Has.Length.EqualTo(2));
        }

        [Test]
        public void Positive_GetAbilities_NoEventTypeSubscribed_ReturnsEmptyArray()
        {
            Dictionary<TriggerEventType, IList<CombatantAbilityEntity>> dictionary = [];
            dictionary.Add(TriggerEventType.COMBATANT_CASTING_COMPLETE, [_combatantAbilityEntity]);
            
            TriggerSubscriber triggerSubscriber = new(dictionary);

            TriggerEventType triggerEventType = GetTriggerComponent(_combatantAbilityEntity).TriggerEventType;
            ImmutableArray<CombatantAbilityEntity> combatantAbilityEntities = triggerSubscriber.GetAbilities(triggerEventType);
            
            Assert.That(combatantAbilityEntities, Has.Length.EqualTo(0));
        }

        [Test]
        public void Positive_GetAbilities_EventTypeSubscribed_ButNoAbilities_ReturnsEmptyArray()
        {
            Dictionary<TriggerEventType, IList<CombatantAbilityEntity>> dictionary = [];
            
            TriggerEventType triggerEventType = GetTriggerComponent(_combatantAbilityEntity).TriggerEventType;
            dictionary.Add(triggerEventType, []);
            
            TriggerSubscriber triggerSubscriber = new(dictionary);

            ImmutableArray<CombatantAbilityEntity> combatantAbilityEntities = triggerSubscriber.GetAbilities(triggerEventType);
            
            Assert.That(combatantAbilityEntities, Has.Length.EqualTo(0));
        }
    }
}
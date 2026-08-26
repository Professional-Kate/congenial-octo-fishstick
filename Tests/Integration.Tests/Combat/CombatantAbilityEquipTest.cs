using IdelPog.Combat;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Error;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Runtime.Event;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Integration.Tests.Combat
{
    [TestFixture]
    public sealed class CombatantAbilityEquipTest : ManagedTestBuffer
    {
        private ManagedResponseListener<CombatantAbilityEquipResponse> _responseListener;
        private ManagedErrorListener<CombatantAbilityEquipError> _errorListener;

        private CombatantAbilityCard _combatantAbilityCard;
        private AbilityCreation _basicAttackCreation; 
        private CombatantAbilityEquip _combatantAbilityEquip;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantAbilityCard = new CombatantAbilityCard
                { AbilityID = 0, StrategyCards = [ new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY, Priority = 0 }]};
            
            _basicAttackCreation = new AbilityCreation
            {
                AbilityCard = new AbilityCard { AbilitySlots = 1, Cooldown = 9 },
                TriggerCard = new TriggerCard { TriggerEventType = TriggerEventType.ABILITY_READY, TargetingType = TargetingType.SELF, MinTriggerValue = 0, MaxTriggerValue = 0 },
                AbilityStageCards = [new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.SLASH, CastTime = 0,  MaxTargets = 1, Priority = 0, Value = 37 }]
            };
            
            _combatantAbilityEquip = new CombatantAbilityEquip
            {
                CombatantID = 0, 
                AbilityCards = [_combatantAbilityCard]
            };
        }

        [SetUp]
        public void Setup()
        {
            _responseListener = new ManagedResponseListener<CombatantAbilityEquipResponse>();
            _errorListener = new ManagedErrorListener<CombatantAbilityEquipError>();
            
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
        }
        
        private static void AssertResponse(CombatantAbilityEquipResponse response, CombatantAbilityEquip source)
        {
            Assert.That(response.CombatantID, Is.EqualTo(source.CombatantID));

            for (int i = 0; i < source.AbilityCards.Length; i++)
            {
                CombatantAbilityCard sourceCombatantAbilityCard = source.AbilityCards[i];
                Assert.That(response.CombatantAbilityIDs[i], Is.EqualTo(sourceCombatantAbilityCard.AbilityID));
            }
        }

        private void AssertErrorLength(int length)
        { 
            Assert.That(_errorListener.Error.CombatantAbilityEquips, Has.Length.EqualTo(length));
        }

        private void AssertErrorCollection(params CombatantAbilityEquip[] combatantAbilityEquips)
        {
            Assert.That(_errorListener.Error.CombatantAbilityEquips, Is.EqualTo(combatantAbilityEquips));
        }

        [Test]
        public void Positive_DispatchMessage_EquipsAbility()
        {
            DispatchMessage(_basicAttackCreation);
            
            DispatchMessage(_combatantAbilityEquip);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], _combatantAbilityEquip);
        }
        
        [Test]
        public void Positive_DispatchMessage_EquipsDuplicateAbility()
        {
            CombatantAbilityEquip duplicateEquip = new()
            {
                CombatantID = 0, 
                AbilityCards = [_combatantAbilityCard, _combatantAbilityCard]
            };
            
            DispatchMessage(_basicAttackCreation);
            
            DispatchMessage(duplicateEquip);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], duplicateEquip);
        }

        [Test]
        public void Positive_DispatchMessage_MultipleCombatants_DispatchesMultipleResponses()
        {
            CombatantAbilityEquip secondEquip = _combatantAbilityEquip with { CombatantID = 1 };
            
            DispatchMessage(_basicAttackCreation);
            
            DispatchMessage(_combatantAbilityEquip, secondEquip);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(2);
            AssertResponse(_responseListener.Responses[0], _combatantAbilityEquip);
            AssertResponse(_responseListener.Responses[1], secondEquip);
        }

        [Test]
        public void Positive_DispatchMessage_CombatantNotCreated_DispatchesResponse()
        {
            DispatchMessage(_basicAttackCreation);
            
            DispatchMessage(_combatantAbilityEquip);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], _combatantAbilityEquip);
        }

        [Test]
        public void Positive_DispatchMessage_MultipleEquipsForSameCombatant_DispatchesResponses()
        {
            DispatchMessage(_basicAttackCreation);
            
            // Multiple equips at once
            DispatchMessage(_combatantAbilityEquip, _combatantAbilityEquip);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(2);
            
            AssertResponse(_responseListener.Responses[0], _combatantAbilityEquip);
            AssertResponse(_responseListener.Responses[1], _combatantAbilityEquip);
            
            // Equipping another ability later 
            DispatchMessage(_combatantAbilityEquip);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], _combatantAbilityEquip);
            
        }

        [Test]
        public void Positive_DispatchMessage_PriorityIsSorted_DispatchesResponse()
        {
            AbilityStageCard[] abilityStageCards =
            [
                new() { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.SLASH, CastTime = 0,  MaxTargets = 1, Value = 5, Priority = 3 },
                new() { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.SLASH, CastTime = 0,  MaxTargets = 1, Value = 5, Priority = 4 },
                new() { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.SLASH, CastTime = 0,  MaxTargets = 1, Value = 5, Priority = 1 },
                new() { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.SLASH, CastTime = 0,  MaxTargets = 1, Value = 5, Priority = 5 },
                new() { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.SLASH, CastTime = 0,  MaxTargets = 1, Value = 5, Priority = 2 }
            ];

            AbilityCreation abilityCreation = _basicAttackCreation with { AbilityStageCards = abilityStageCards };
            DispatchMessage(abilityCreation);
            
            StrategyCard[] strategyCards = 
            [
                new() { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY, Priority = 2 },
                new() { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY, Priority = 4 },
                new() { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY, Priority = 3 },
                new() { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY, Priority = 1 },
                new() { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY, Priority = 5 }
            ];
            
            CombatantAbilityEquip combatantAbilityEquip = new() { CombatantID = 0, AbilityCards = [_combatantAbilityCard with { StrategyCards = strategyCards}] };
            
            DispatchMessage(combatantAbilityEquip);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], combatantAbilityEquip);
        }

        [Test]
        public void Negative_DispatchMessage_EmptyAbilities_DispatchesError()
        {
            DispatchMessage(_basicAttackCreation);
            
            DispatchMessage(_combatantAbilityEquip with { AbilityCards = [] });
            
            _responseListener.AssertWasCalled(false);
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertErrorCollection(_combatantAbilityEquip with { AbilityCards = [] });
            AssertBaseError<EmptyCollectionException>(_errorListener.Error.BaseError);
        }

        [Test]
        public void Negative_DispatchMessage_AbilityNotCreated_DispatchesError()
        {
            DispatchMessage(_combatantAbilityEquip);
            
            _responseListener.AssertWasCalled(false);
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertErrorCollection(_combatantAbilityEquip);
            AssertBaseError<NotFoundException<byte>>(_errorListener.Error.BaseError);
        }

        [Test]
        public void Negative_DispatchMessage_MoreAbilitiesThanMaximum_SingleMessage_DispatchesError()
        {
            RegisterWithOptions(new CombatOptions { MaxCombatantAbilitySlots = 1, MaxIterations = 100 });
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
            
            CombatantAbilityEquip tooManyAbilities = new()
            {
                CombatantID = 0, 
                AbilityCards = [_combatantAbilityCard, _combatantAbilityCard with { AbilityID = 0 }]
            };

            DispatchMessage(_basicAttackCreation, _basicAttackCreation);
            
            DispatchMessage(tooManyAbilities);
            
            _responseListener.AssertWasCalled(false);
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertErrorCollection(tooManyAbilities);
            AssertBaseError<TooManyAbilitiesException>(_errorListener.Error.BaseError);
        }

        [Test]
        public void Negative_DispatchMessage_MoreAbilitiesThanMinimum_MultipleCommands_DispatchesError()
        {
            RegisterWithOptions(new CombatOptions { MaxCombatantAbilitySlots = 1, MaxIterations = 100 });
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
            
            CombatantAbilityEquip tooManyAbilities = new()
            {
                CombatantID = 0, 
                AbilityCards = [_combatantAbilityCard]
            };

            DispatchMessage(_basicAttackCreation);
            
            // allowed
            DispatchMessage(_combatantAbilityEquip);
            _responseListener.AssertWasCalled(true);
            
            // One ability is already added (1 AbilitySlot) adding another should throw
            DispatchMessage(tooManyAbilities);
            
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertErrorCollection(tooManyAbilities);
            AssertBaseError<TooManyAbilitiesException>(_errorListener.Error.BaseError);
        }

        [Test]
        public void Negative_DispatchMessage_TooManyStrategyCards_DispatchesError()
        {
            DispatchMessage(_basicAttackCreation);

            StrategyCard strategyCard = new()
            {
                TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY, Priority = 0
            };

            CombatantAbilityEquip tooManyCards = new()
            {
                CombatantID = 0,
                AbilityCards = [_combatantAbilityCard with { StrategyCards = [strategyCard, strategyCard] }]
            };
            
            DispatchMessage(tooManyCards);
            
            _responseListener.AssertWasCalled(false);
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertErrorCollection(tooManyCards);
            AssertBaseError<PriorityMissingException>(_errorListener.Error.BaseError);
        }

        [Test]
        public void Negative_DispatchMessage_NotEnoughStrategyCards_DispatchesError()
        {
            AbilityStageCard abilityStageCard = new()
            {
                AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.SLASH, CastTime = 0,  MaxTargets = 1, Priority = 0, Value = 37
            };
                
            DispatchMessage(_basicAttackCreation with { AbilityStageCards = [abilityStageCard,  abilityStageCard]});
            
            DispatchMessage(_combatantAbilityEquip);
            
            _responseListener.AssertWasCalled(false);
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertErrorCollection(_combatantAbilityEquip);
            AssertBaseError<PriorityMissingException>(_errorListener.Error.BaseError);
        }

        [Test]
        public void Negative_DispatchMessage_MismatchedPriority_DispatchesError()
        {
            AbilityStageCard abilityStageCard = new()
            {
                AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.SLASH, CastTime = 0,  MaxTargets = 1, Priority = 5, Value = 37
            };

            // This Priority should be 23 to pass
            AbilityCreation abilityCreation = _basicAttackCreation with
            {
                AbilityStageCards = [abilityStageCard, abilityStageCard with { Priority = 1 }, abilityStageCard with { Priority = 22 }]
            };
            
            StrategyCard strategyCard = new()
            {
                TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY, Priority = 5
            };
            
            CombatantAbilityEquip combatantAbilityEquip = new()
            {
                CombatantID = 0,
                AbilityCards = [_combatantAbilityCard with { StrategyCards = [strategyCard, strategyCard with { Priority = 23 }, strategyCard with { Priority = 1 }]}]
            };

            DispatchMessage(abilityCreation);
            
            DispatchMessage(combatantAbilityEquip);
            
            _responseListener.AssertWasCalled(false);
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertErrorCollection(combatantAbilityEquip);
            AssertBaseError<PriorityMismatchException>(_errorListener.Error.BaseError);
        }
    }
}
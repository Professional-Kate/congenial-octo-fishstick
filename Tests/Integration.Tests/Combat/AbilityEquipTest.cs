using IdelPog.Combat;
using IdelPog.Combat.Ability.Contracts.Command;
using IdelPog.Combat.Combatant.Contracts;
using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Combatant.Contracts.Error;
using IdelPog.Combat.Combatant.Contracts.Response;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Exceptions;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Integration.Tests.Combat
{
    [TestFixture]
    public sealed class AbilityEquipTest : ManagedTestBuffer
    {
        private ManagedResponseListener<AbilityEquipResponse> _responseListener;
        private ManagedErrorListener<AbilityEquipError> _errorListener;

        private EquippedAbility _equippedAbility;
        private AbilityCreation _basicAttackCreation; 
        private AbilityEquip _abilityEquip;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _equippedAbility = new EquippedAbility
                { AbilityID = 0, StrategyCards = [ new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY, Priority = 0 }]};
            
            _basicAttackCreation = new AbilityCreation
            {
                AbilityCard = new AbilityCard { AbilitySlots = 1, Cooldown = 9 },
                TriggerCard = new TriggerCard { TriggerEventType = TriggerEventType.ABILITY_READY, TargetingType = TargetingType.SELF, MinTriggerValue = 0, MaxTriggerValue = 0 },
                AbilityStageCards = [new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.SLASH, CastTime = 0,  MaxTargets = 1, Priority = 0, Value = 37 }]
            };
            
            _abilityEquip = new AbilityEquip
            {
                CombatantID = 0, 
                EquippedAbilities = [_equippedAbility]
            };
        }

        [SetUp]
        public void Setup()
        {
            _responseListener = new ManagedResponseListener<AbilityEquipResponse>();
            _errorListener = new ManagedErrorListener<AbilityEquipError>();
            
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
        }
        
        private static void AssertResponse(AbilityEquipResponse response, AbilityEquip source)
        {
            Assert.That(response.CombatantID, Is.EqualTo(source.CombatantID));

            for (int i = 0; i < source.EquippedAbilities.Length; i++)
            { 
                Assert.That(response.CombatantID, Is.EqualTo(source.CombatantID));
            }
        }

        private void AssertErrorLength(int length)
        { 
            Assert.That(_errorListener.Error.AbilityEquips, Has.Length.EqualTo(length));
        }

        private void AssertErrorCollection(params AbilityEquip[] combatantAbilityEquips)
        {
            Assert.That(_errorListener.Error.AbilityEquips, Is.EqualTo(combatantAbilityEquips));
        }

        [Test]
        public void Positive_DispatchMessage_EquipsAbility()
        {
            DispatchMessage(_basicAttackCreation);
            
            DispatchMessage(_abilityEquip);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], _abilityEquip);
        }
        
        [Test]
        public void Positive_DispatchMessage_EquipsDuplicateAbility()
        {
            AbilityEquip duplicateEquip = new()
            {
                CombatantID = 0, 
                EquippedAbilities = [_equippedAbility, _equippedAbility]
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
            AbilityEquip secondEquip = _abilityEquip with { CombatantID = 1 };
            
            DispatchMessage(_basicAttackCreation);
            
            DispatchMessage(_abilityEquip, secondEquip);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(2);
            AssertResponse(_responseListener.Responses[0], _abilityEquip);
            AssertResponse(_responseListener.Responses[1], secondEquip);
        }

        [Test]
        public void Positive_DispatchMessage_CombatantNotCreated_DispatchesResponse()
        {
            DispatchMessage(_basicAttackCreation);
            
            DispatchMessage(_abilityEquip);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], _abilityEquip);
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
            
            AbilityEquip abilityEquip = new() { CombatantID = 0, EquippedAbilities = [_equippedAbility with { StrategyCards = strategyCards}] };
            
            DispatchMessage(abilityEquip);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], abilityEquip);
        }
        
        [Test]
        public void Negative_DispatchMessage_MultipleEquipsForSameCombatant_DispatchesError()
        {
            DispatchMessage(_basicAttackCreation);
            
            DispatchMessage(_abilityEquip);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], _abilityEquip);
            
            // Equipping another ability later should throw
            DispatchMessage(_abilityEquip);
            _errorListener.AssertWasCalled(true);
            
        }

        [Test]
        public void Negative_DispatchMessage_EmptyAbilities_DispatchesError()
        {
            DispatchMessage(_basicAttackCreation);
            
            DispatchMessage(_abilityEquip with { EquippedAbilities = [] });
            
            _responseListener.AssertWasCalled(false);
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertErrorCollection(_abilityEquip with { EquippedAbilities = [] });
            AssertBaseError<EmptyCollectionException>(_errorListener.Error.BaseError);
        }

        [Test]
        public void Negative_DispatchMessage_AbilityNotCreated_DispatchesError()
        {
            DispatchMessage(_abilityEquip);
            
            _responseListener.AssertWasCalled(false);
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertErrorCollection(_abilityEquip);
            AssertBaseError<NotFoundException<byte>>(_errorListener.Error.BaseError);
        }

        [Test]
        public void Negative_DispatchMessage_MoreAbilitiesThanMaximum_SingleAbility_DispatchesError()
        {
            RegisterWithOptions(new CombatOptions { MaxCombatantAbilitySlots = 0, MaxIterations = 100 });
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
            
            AbilityEquip tooManyAbilities = new()
            {
                CombatantID = 0, 
                EquippedAbilities = [_equippedAbility]
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
        public void Negative_DispatchMessage_MoreAbilitiesThanMaximum_MultipleAbilities_DispatchesError()
        {
            RegisterWithOptions(new CombatOptions { MaxCombatantAbilitySlots = 1, MaxIterations = 100 });
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
            DispatchMessage(_basicAttackCreation);
            
            AbilityEquip tooManyAbilities = new()
            {
                CombatantID = 0, 
                EquippedAbilities = [_equippedAbility, _equippedAbility]
            };

            DispatchMessage(tooManyAbilities);
            
            _responseListener.AssertWasCalled(false);
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

            AbilityEquip tooManyCards = new()
            {
                CombatantID = 0,
                EquippedAbilities = [_equippedAbility with { StrategyCards = [strategyCard, strategyCard] }]
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
            
            DispatchMessage(_abilityEquip);
            
            _responseListener.AssertWasCalled(false);
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertErrorCollection(_abilityEquip);
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
            
            AbilityEquip abilityEquip = new()
            {
                CombatantID = 0,
                EquippedAbilities = [_equippedAbility with { StrategyCards = [strategyCard, strategyCard with { Priority = 23 }, strategyCard with { Priority = 1 }]}]
            };

            DispatchMessage(abilityCreation);
            
            DispatchMessage(abilityEquip);
            
            _responseListener.AssertWasCalled(false);
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertErrorCollection(abilityEquip);
            AssertBaseError<PriorityMismatchException>(_errorListener.Error.BaseError);
        }
    }
}
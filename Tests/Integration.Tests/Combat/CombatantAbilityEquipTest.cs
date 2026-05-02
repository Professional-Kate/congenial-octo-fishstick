using IdelPog.Combat;
using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Error;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Exceptions;
using IdelPog.Core.Contracts;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Integration.Tests.Combat
{
    [TestFixture]
    public sealed class CombatantAbilityEquipTest : ManagedTestBuffer
    {
        private ManagedResponseListener<CombatantAbilityEquipResponse> _responseListener;
        private ManagedErrorListener<CombatantAbilityEquipError> _errorListener;

        private AbilityCard _abilityCard;
        private CombatantCreation _combatantCreation;
        private AbilityCreation _basicAttackCreation; 
        private CombatantAbilityEquip _combatantAbilityEquip;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _abilityCard = new AbilityCard
                { AbilityType = AbilityType.BASIC_ATTACK, StrategyCard = new StrategyCard { TargetingType = TargetingType.HIGH_ATTACK } };
            
            _basicAttackCreation = new AbilityCreation
            {
                Information = new Information { Name = "Basic attack", Description = "Attack an enemy but kinda basically" },
                AbilityType = AbilityType.BASIC_ATTACK,
                Cooldown = 9,
                Damage = 3,
                AbilitySlots = 1
            };
            
            _combatantCreation = new CombatantCreation
            {
                CombatantType = CombatantType.HUMAN,
                Information = new Information { Name = "Human", Description = "Man" },
                StatCard = new StatCard { Attack = 10, Speed = 5, Health = 20 }
            };
            
            _combatantAbilityEquip = new CombatantAbilityEquip
            {
                CombatantID = 0, 
                AbilityCards = [_abilityCard]
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
        
        private static void AssertResponse(CombatantAbilityEquipResponse response, CombatantAbilityEquip source, AbilityCreation abilityCreation)
        {
            Assert.That(response.CombatantID, Is.EqualTo(source.CombatantID));

            for (int i = 0; i < source.AbilityCards.Length; i++)
            {
                AbilityCard sourceAbilityCard = source.AbilityCards[i];
                Assert.Multiple(() =>
                {
                    Assert.That(response.CombatantAbilities[i].AbilityType, Is.EqualTo(sourceAbilityCard.AbilityType));
                    Assert.That(response.CombatantAbilities[i].Damage, Is.EqualTo(abilityCreation.Damage));
                    Assert.That(response.CombatantAbilities[i].Cooldown, Is.EqualTo(abilityCreation.Cooldown));
                });
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
            DispatchMessage(_combatantCreation);
            DispatchMessage(_combatantAbilityEquip);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], _combatantAbilityEquip, _basicAttackCreation);
        }
        
        [Test]
        public void Positive_DispatchMessage_EquipsDuplicateAbility()
        {
            CombatantAbilityEquip duplicateEquip = new()
            {
                CombatantID = 0, 
                AbilityCards = [_abilityCard, _abilityCard]
            };
            
            DispatchMessage(_basicAttackCreation);
            DispatchMessage(_combatantCreation);
            DispatchMessage(duplicateEquip);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], duplicateEquip, _basicAttackCreation);
        }

        [Test]
        public void Positive_DispatchMessage_MultipleCombatants_DispatchesMultipleResponses()
        {
            CombatantAbilityEquip secondEquip = _combatantAbilityEquip with { CombatantID = 1 };
            
            DispatchMessage(_basicAttackCreation);
            DispatchMessage(_combatantCreation, _combatantCreation);
            DispatchMessage(_combatantAbilityEquip, secondEquip);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(2);
            AssertResponse(_responseListener.Responses[0], _combatantAbilityEquip, _basicAttackCreation);
            AssertResponse(_responseListener.Responses[1], secondEquip, _basicAttackCreation);
        }

        [Test]
        public void Positive_DispatchMessage_CombatantNotCreated_DispatchesResponse()
        {
            DispatchMessage(_basicAttackCreation);
            DispatchMessage(_combatantAbilityEquip);
            
            _responseListener.AssertWasCalled(true);
            _errorListener.AssertWasCalled(false);
            _responseListener.AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], _combatantAbilityEquip, _basicAttackCreation);
        }

        [Test]
        public void Negative_DispatchMessage_EmptyAbilities_DispatchesError()
        {
            DispatchMessage(_basicAttackCreation);
            DispatchMessage(_combatantCreation);
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
            DispatchMessage(_combatantCreation);
            DispatchMessage(_combatantAbilityEquip);
            
            _responseListener.AssertWasCalled(false);
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertErrorCollection(_combatantAbilityEquip);
            AssertBaseError<NotFoundException<AbilityType>>(_errorListener.Error.BaseError);
        }

        [Test]
        public void Negative_DispatchMessage_MoreAbilitiesThanMaximum_DispatchesError()
        {
            RegisterWithOptions(new CombatOptions { MaxCombatantAbilitySlots = 1, MaxIterations = 100 });
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
            
            CombatantAbilityEquip tooManyAbilities = new()
            {
                CombatantID = 0, 
                AbilityCards = [_abilityCard, _abilityCard with { AbilityType = AbilityType.STRONG_ATTACK }]
            };
            
            DispatchMessage(_basicAttackCreation, _basicAttackCreation with { AbilityType = AbilityType.STRONG_ATTACK });
            DispatchMessage(_combatantCreation);
            DispatchMessage(tooManyAbilities);
            
            _responseListener.AssertWasCalled(false);
            _errorListener.AssertWasCalled(true);
            AssertErrorLength(1);
            AssertErrorCollection(tooManyAbilities);
            AssertBaseError<TooManyAbilitiesException>(_errorListener.Error.BaseError);
        }
    }
}
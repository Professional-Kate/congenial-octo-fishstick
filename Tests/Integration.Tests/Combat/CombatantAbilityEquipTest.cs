using IdelPog.Combat;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Error;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Event;
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

        private CombatantAbilityCard _combatantAbilityCard;
        private CombatantCreation _combatantCreation;
        private AbilityCreation _basicAttackCreation; 
        private CombatantAbilityEquip _combatantAbilityEquip;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantAbilityCard = new CombatantAbilityCard
                { AbilityType = AbilityType.SLASH, StrategyCard = new StrategyCard { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH }};
            
            _basicAttackCreation = new AbilityCreation
            {
                Information = new Information { Name = "Basic attack", Description = "Attack an enemy but kinda basically" },
                AbilityCard = new AbilityCard {  AbilityType = AbilityType.SLASH, EventType = EventType.DIRECT_DAMAGE, Cooldown = 9, AbilitySlots = 1, CastTime = 0},
                ElementalDamageCard = new ElementalDamageCard { ColdDamage = 2, LightningDamage = 5, FireDamage = 125 },
                PhysicalDamageCard = new PhysicalDamageCard { SlashDamage = 3, StrikeDamage = 123, ThrustDamage = 1 }
            };
            
            _combatantCreation = new CombatantCreation
            {
                CombatantType = CombatantType.HUMAN,
                Information = new Information { Name = "Human", Description = "Man" },
                StatCard = new StatCard { Health = 20 },
                AgilityCard = new AgilityCard { Speed = 5, Initiative = 1 }
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
        
        private static void AssertResponse(CombatantAbilityEquipResponse response, CombatantAbilityEquip source, AbilityCreation abilityCreation)
        {
            Assert.That(response.CombatantID, Is.EqualTo(source.CombatantID));

            for (int i = 0; i < source.AbilityCards.Length; i++)
            {
                CombatantAbilityCard sourceCombatantAbilityCard = source.AbilityCards[i];
                Assert.Multiple(() =>
                {
                    Assert.That(response.CombatantAbilities[i].AbilityType, Is.EqualTo(sourceCombatantAbilityCard.AbilityType));
                    Assert.That(response.CombatantAbilities[i].ElementalDamageCard, Is.EqualTo(abilityCreation.ElementalDamageCard));
                    Assert.That(response.CombatantAbilities[i].Cooldown, Is.EqualTo(abilityCreation.AbilityCard.Cooldown));
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
                AbilityCards = [_combatantAbilityCard, _combatantAbilityCard]
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
                AbilityCards = [_combatantAbilityCard, _combatantAbilityCard with { AbilityType = AbilityType.STAB }]
            };

            AbilityCard abilityCard = _basicAttackCreation.AbilityCard with { AbilityType = AbilityType.STAB };
            DispatchMessage(_basicAttackCreation, _basicAttackCreation with { AbilityCard = abilityCard });
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
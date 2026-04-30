using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Factory.Interface;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Mediator
{
    public sealed class CombatantAbilityEquipMediator : IBatchMediator<CombatantAbilityEquip>
    {
        private readonly ICombatantAbilityEntityRepository _combatantAbilityEntityRepository;
        private readonly ICombatantAbilityEntityFactory _combatantAbilityEntityFactory;
        private readonly ICombatantAbilityFactory _combatantAbilityFactory;
        private readonly IDispatchMany<CombatantAbilityEquipResponse> _responseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly ICombatantAbilityAssertion _combatantAbilityAssertion;

        public CombatantAbilityEquipMediator(ICombatantAbilityEntityRepository combatantAbilityEntityRepository, ICombatantAbilityEntityFactory combatantAbilityEntityFactory, ICombatantAbilityFactory combatantAbilityFactory, IDispatchMany<CombatantAbilityEquipResponse> responseDispatcher, ICollectionAssertion collectionAssertion, ICombatantAbilityAssertion combatantAbilityAssertion)
        {
            _combatantAbilityEntityRepository = combatantAbilityEntityRepository;
            _combatantAbilityEntityFactory = combatantAbilityEntityFactory;
            _combatantAbilityFactory = combatantAbilityFactory;
            _responseDispatcher = responseDispatcher;
            _collectionAssertion = collectionAssertion;
            _combatantAbilityAssertion = combatantAbilityAssertion;
        }

        public void HandleMessages(IReadOnlyList<CombatantAbilityEquip> messages)
        {
            _collectionAssertion.AssertHasElements(messages);
            
            CombatantAbilityEquipResponse[] responses = new CombatantAbilityEquipResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                CombatantAbilityEquip combatantAbilityEquip = messages[i];
                _collectionAssertion.AssertHasElements(combatantAbilityEquip.AbilityCards);
                _combatantAbilityAssertion.AssertAbilityCount(combatantAbilityEquip);

                _combatantAbilityEntityRepository.Add(combatantAbilityEquip.CombatantID, _combatantAbilityEntityFactory.Create(combatantAbilityEquip));
                
                responses[i] = CreateResponse(combatantAbilityEquip, _combatantAbilityFactory.CreateCombatantAbilities(_combatantAbilityEntityRepository.GetAll(combatantAbilityEquip.CombatantID)));
            }
            
            _responseDispatcher.Dispatch(responses);
        }
        
        private static CombatantAbilityEquipResponse CreateResponse(CombatantAbilityEquip combatantAbilityEquip, CombatantAbility[] combatantAbilities) => 
            new() { CombatantAbilities = combatantAbilities, CombatantID = combatantAbilityEquip.CombatantID };
    }
}
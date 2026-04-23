using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Contracts.Skill;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Mediator
{
    public sealed class CombatantSkillCreationMediator : IBatchMediator<CombatantSkillCreation>
    {
        private readonly IAssetRepository<SkillType, SkillEntity> _skillEntityRepository;
        private readonly ISkillEntityFactory _skillEntityFactory;
        private readonly IDispatchMany<CombatantSkillCreationResponse> _responseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IUniqueAssertion _uniqueAssertion;

        public CombatantSkillCreationMediator(IAssetRepository<SkillType, SkillEntity> skillEntityRepository, ISkillEntityFactory skillEntityFactory, IDispatchMany<CombatantSkillCreationResponse> responseDispatcher, ICollectionAssertion collectionAssertion, IUniqueAssertion uniqueAssertion)
        {
            _skillEntityRepository = skillEntityRepository;
            _skillEntityFactory = skillEntityFactory;
            _responseDispatcher = responseDispatcher;
            _collectionAssertion = collectionAssertion;
            _uniqueAssertion = uniqueAssertion;
        }

        public void HandleMessages(IReadOnlyList<CombatantSkillCreation> messages)
        {
            _collectionAssertion.AssertHasElements(messages);
            
            CombatantSkillCreationResponse[] responses = new CombatantSkillCreationResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                CombatantSkillCreation combatantSkillCreation = messages[i];
                _uniqueAssertion.AssertUnique(combatantSkillCreation.SkillType, _skillEntityRepository.Contains(combatantSkillCreation.SkillType));
                
                _skillEntityRepository.Add(combatantSkillCreation.SkillType, _skillEntityFactory.CreateSkillEntity(combatantSkillCreation));
                responses[i] = CreateResponse(combatantSkillCreation);
            }
            
            _responseDispatcher.Dispatch(responses);
        }

        private static CombatantSkillCreationResponse CreateResponse(CombatantSkillCreation combatantSkillCreation)
        {
            return new CombatantSkillCreationResponse
            {
                Information = combatantSkillCreation.Information, 
                SkillType =  combatantSkillCreation.SkillType, 
                Speed =  combatantSkillCreation.Speed,
                Damage = combatantSkillCreation.Damage
            };
        }
    }
}
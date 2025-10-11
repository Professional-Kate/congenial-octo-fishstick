using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.HarvestNode.Contracts;
using IdelPog.HarvestNode.Contracts.Command;
using IdelPog.HarvestNode.Contracts.Response;
using IdelPog.HarvestNode.Runtime.Factory.Interface;
using IdelPog.Progression.Runtime;

namespace IdelPog.HarvestNode.Runtime.Mediator
{
    public sealed class NodeRequirementsCreationMediator : IBatchMediator<HarvestNodeRequirementsCreation>
    {
        private readonly IAssetRepository<SkillID, UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse>> _entityRepository;
        private readonly IUnlockRequirementsEntityFactory _entityFactory;
        private readonly IDispatchMany<HarvestNodeRequirementsCreationResponse> _responseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IUniqueAssertion _uniqueAssertion;

        public NodeRequirementsCreationMediator(IAssetRepository<SkillID, UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse>> entityRepository, IUnlockRequirementsEntityFactory entityFactory, IDispatchMany<HarvestNodeRequirementsCreationResponse> responseDispatcher, ICollectionAssertion collectionAssertion, IUniqueAssertion uniqueAssertion)
        {
            _entityRepository = entityRepository;
            _entityFactory = entityFactory;
            _responseDispatcher = responseDispatcher;
            _collectionAssertion = collectionAssertion;
            _uniqueAssertion = uniqueAssertion;
        }

        public void HandleMessages(IReadOnlyList<HarvestNodeRequirementsCreation> messages)
        {
            _collectionAssertion.AssertHasElements(messages);
            HarvestNodeRequirementsCreationResponse[] responses = new HarvestNodeRequirementsCreationResponse[messages.Count];

            for (int i = 0; i < messages.Count; i++)
            {
                HarvestNodeRequirementsCreation creation = messages[i];
                _collectionAssertion.AssertHasElements(creation.HarvestNodeRequirements);
                
                SkillID skillID = creation.SkillID;
                _uniqueAssertion.AssertUnique(skillID, _entityRepository.Contains(skillID));

                UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse> entity = _entityFactory.Create(skillID, creation.HarvestNodeRequirements);
                _entityRepository.Add(skillID, entity);

                responses[i] = CreateResponse(skillID, creation.HarvestNodeRequirements);
            }

            _responseDispatcher.Dispatch(responses);
        }

        private static HarvestNodeRequirementsCreationResponse CreateResponse(SkillID skillID, HarvestNodeRequirement[] harvestNodeRequirements)
        {
            return new HarvestNodeRequirementsCreationResponse
            {
                SkillID = skillID,
                HarvestNodeRequirements = harvestNodeRequirements
            };
        }
    }
}
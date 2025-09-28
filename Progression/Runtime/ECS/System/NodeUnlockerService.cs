using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Progression.Assertion.Interface;
using IdelPog.Progression.Contracts;
using IdelPog.Progression.Runtime.ECS.Component;
using IdelPog.Progression.Runtime.ECS.System.Interface;

namespace IdelPog.Progression.Runtime.ECS.System
{
    public sealed class NodeUnlockerService : INodeUnlockerService
    {
        private readonly IAssetRepository<SkillID, UnlockRequirementsEntity<HarvestNodeUnlockResponse>> _entityRepository;
        private readonly IFoundAssertion _foundAssertion;
        private readonly ISkillMatchesAssertion _skillMatchesAssertion;
        private readonly ICanUnlockAssertion<HarvestNodeUnlockResponse> _canUnlockAssertion;
        private readonly IQueueAssertion<HarvestNodeUnlockResponse> _queueAssertion;

        public NodeUnlockerService(IAssetRepository<SkillID, UnlockRequirementsEntity<HarvestNodeUnlockResponse>> entityRepository, IFoundAssertion foundAssertion, ICanUnlockAssertion<HarvestNodeUnlockResponse> canUnlockAssertion, ISkillMatchesAssertion skillMatchesAssertion, IQueueAssertion<HarvestNodeUnlockResponse> queueAssertion)
        {
            _entityRepository = entityRepository;
            _foundAssertion = foundAssertion;
            _canUnlockAssertion = canUnlockAssertion;
            _skillMatchesAssertion = skillMatchesAssertion;
            _queueAssertion = queueAssertion;
        }

        public bool CanUnlock(HarvestNodeUnlock harvestNodeUnlock)
        {
            AssertSkillFound(harvestNodeUnlock.SkillID);
            
            UnlockRequirementsEntity<HarvestNodeUnlockResponse> entity = _entityRepository.Get(harvestNodeUnlock.SkillID);
            NodeLevelRequirement<HarvestNodeUnlockResponse> firstComponent = GetFirstComponent(entity, harvestNodeUnlock);
            
            bool canUnlock = harvestNodeUnlock.SkillLevel >= firstComponent.Level;
            return canUnlock;
        }

        public HarvestNodeUnlockResponse Unlock(HarvestNodeUnlock harvestNodeUnlock)
        {
            AssertSkillFound(harvestNodeUnlock.SkillID);
            
            UnlockRequirementsEntity<HarvestNodeUnlockResponse> entity = _entityRepository.Get(harvestNodeUnlock.SkillID);
            NodeLevelRequirement<HarvestNodeUnlockResponse> firstComponent = GetFirstComponent(entity, harvestNodeUnlock);
            
            _canUnlockAssertion.AssertCanUnlock(harvestNodeUnlock.SkillLevel, firstComponent.Level, firstComponent);
            
            bool successful = entity.TryDequeue(out NodeLevelRequirement<HarvestNodeUnlockResponse> dequeuedComponent);
            _queueAssertion.AssertSuccessfulDequeue(successful, firstComponent);

            return dequeuedComponent.OnUnlockCommand;
        }

        private void AssertSkillFound(SkillID skillID)
        {
            _foundAssertion.AssertFound(skillID, _entityRepository.Contains(skillID));
        }

        private NodeLevelRequirement<HarvestNodeUnlockResponse> GetFirstComponent(UnlockRequirementsEntity<HarvestNodeUnlockResponse> entity, HarvestNodeUnlock harvestNodeUnlock)
        {
            NodeLevelRequirement<HarvestNodeUnlockResponse> firstComponent = entity.Peek();
            _skillMatchesAssertion.AssertSkillMatches(harvestNodeUnlock.SkillID, firstComponent.SkillID);

            return firstComponent;
        }
    }
}
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Progression.Assertion.Interface;
using IdelPog.Progression.Runtime.Component;
using IdelPog.Progression.Runtime.System.Interface;

namespace IdelPog.Progression.Runtime.System
{
    public sealed class EntityUnlockerService<TID, TCommand> : IEntityUnlockerService<TID, TCommand> where TCommand : struct
    {
        private readonly IAssetRepository<TID, UnlockRequirementsEntity<TID, TCommand>> _entityRepository;
        private readonly IFoundAssertion _foundAssertion;
        private readonly ISkillMatchesAssertion<TID> _skillMatchesAssertion;
        private readonly ICanUnlockAssertion<TID, TCommand> _canUnlockAssertion;
        private readonly IQueueAssertion<TID, TCommand> _queueAssertion;

        public EntityUnlockerService(IAssetRepository<TID, UnlockRequirementsEntity<TID, TCommand>> entityRepository, IFoundAssertion foundAssertion, ICanUnlockAssertion<TID, TCommand> canUnlockAssertion, ISkillMatchesAssertion<TID> skillMatchesAssertion, IQueueAssertion<TID, TCommand> queueAssertion)
        {
            _entityRepository = entityRepository;
            _foundAssertion = foundAssertion;
            _canUnlockAssertion = canUnlockAssertion;
            _skillMatchesAssertion = skillMatchesAssertion;
            _queueAssertion = queueAssertion;
        }

        public bool CanUnlock(TID id, byte skillLevel)
        {
            AssertSkillFound(id);
            
            UnlockRequirementsEntity<TID, TCommand> entity = _entityRepository.Get(id);
            LevelRequirementComponent<TID, TCommand> firstComponent = GetFirstComponent(entity, id);
            
            bool canUnlock = skillLevel >= firstComponent.Level;
            return canUnlock;
        }

        public TCommand Unlock(TID id, byte skillLevel)
        {
            AssertSkillFound(id);
            
            UnlockRequirementsEntity<TID, TCommand> entity = _entityRepository.Get(id);
            LevelRequirementComponent<TID, TCommand> firstComponent = GetFirstComponent(entity, id);
            
            _canUnlockAssertion.AssertCanUnlock(skillLevel, firstComponent.Level, firstComponent);
            
            bool successful = entity.TryDequeue(out LevelRequirementComponent<TID, TCommand> dequeuedComponent);
            _queueAssertion.AssertSuccessfulDequeue(successful, firstComponent);

            return dequeuedComponent.OnUnlockCommand;
        }

        private void AssertSkillFound(TID id)
        {
            _foundAssertion.AssertFound(id, _entityRepository.Contains(id));
        }

        private LevelRequirementComponent<TID, TCommand> GetFirstComponent(UnlockRequirementsEntity<TID, TCommand> entity, TID id)
        {
            LevelRequirementComponent<TID, TCommand> firstComponent = entity.Peek();
            _skillMatchesAssertion.AssertSkillMatches(id, firstComponent.ID);

            return firstComponent;
        }
    }
}
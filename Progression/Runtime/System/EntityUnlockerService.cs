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
        private readonly IIDMatchesAssertion<TID> _idMatchesAssertion;
        private readonly ICanUnlockAssertion<TID, TCommand> _canUnlockAssertion;
        private readonly IQueueAssertion<TID, TCommand> _queueAssertion;

        public EntityUnlockerService(IAssetRepository<TID, UnlockRequirementsEntity<TID, TCommand>> entityRepository, IFoundAssertion foundAssertion, ICanUnlockAssertion<TID, TCommand> canUnlockAssertion, IIDMatchesAssertion<TID> idMatchesAssertion, IQueueAssertion<TID, TCommand> queueAssertion)
        {
            _entityRepository = entityRepository;
            _foundAssertion = foundAssertion;
            _canUnlockAssertion = canUnlockAssertion;
            _idMatchesAssertion = idMatchesAssertion;
            _queueAssertion = queueAssertion;
        }

        public bool CanUnlock(TID id, byte skillLevel)
        {
            AssertSkillFound(id);

            return CanUnlockComponent(id, skillLevel, GetEntity(id));
        }

        public TCommand Unlock(TID id, byte skillLevel)
        {
            AssertSkillFound(id);

            return DequeueComponent(id, skillLevel, GetEntity(id));
        }

        public IEnumerable<TCommand> UnlockAllAvailable(TID id, byte skillLevel)
        {
            AssertSkillFound(id);
            UnlockRequirementsEntity<TID, TCommand> entity = GetEntity(id);
            
            while (CanUnlockComponent(id, skillLevel, entity))
            {
                yield return DequeueComponent(id, skillLevel, entity);
            }
        }

        private UnlockRequirementsEntity<TID, TCommand> GetEntity(TID id)
        {
            UnlockRequirementsEntity<TID, TCommand> entity = _entityRepository.Get(id);
            return entity;
        }

        private bool CanUnlockComponent(TID id, byte skillLevel, UnlockRequirementsEntity<TID, TCommand> entity)
        {
            if (GetFirstComponent(entity, id, out LevelRequirementComponent<TID, TCommand> firstComponent) == false)
            {
                return false;
            }
            
            bool canUnlock = skillLevel >= firstComponent.Level;
            return canUnlock;
        }

        private TCommand DequeueComponent(TID id, byte skillLevel, UnlockRequirementsEntity<TID, TCommand> entity)
        {
            bool hasFirst = GetFirstComponent(entity, id, out LevelRequirementComponent<TID, TCommand> firstComponent);
            if (hasFirst == false)
            {
                throw new InvalidOperationException();
            }
            
            _canUnlockAssertion.AssertCanUnlock(skillLevel, firstComponent.Level, firstComponent);
            
            bool successful = entity.TryDequeue(out LevelRequirementComponent<TID, TCommand> dequeuedComponent);
            _queueAssertion.AssertSuccessfulDequeue(successful, firstComponent);

            return dequeuedComponent.OnUnlockCommand;
        }

        private void AssertSkillFound(TID id)
        { 
            _foundAssertion.AssertFound(id, _entityRepository.Contains(id));
            
        }

        private bool GetFirstComponent(UnlockRequirementsEntity<TID, TCommand> entity, TID id, out LevelRequirementComponent<TID, TCommand> component)
        {
            if (entity.TryPeek(out component) == false)
            {
                return false;
            }
            
            _idMatchesAssertion.AssertIDMatches(id, component.ID);

            return true;
        }
    }
}
using IdelPog.Core.Repository.Asset;
using IdelPog.Progression.Runtime.Component;
using IdelPog.Progression.Runtime.System.Interface;

namespace IdelPog.Progression.Runtime.System
{
    public sealed class EntityUnlockChecker<TID, TCommand> : IEntityUnlockChecker<TID, TCommand> where TCommand : struct
    {
        private readonly IAssetRepository<TID, UnlockRequirementsEntity<TID, TCommand>> _entityRepository;

        public EntityUnlockChecker(IAssetRepository<TID, UnlockRequirementsEntity<TID, TCommand>> entityRepository)
        {
            _entityRepository = entityRepository;
        }

        public bool IsUnlocked(TID id, Predicate<LevelRequirementComponent<TID, TCommand>> predicate)
        {
            bool contains = _entityRepository.Contains(id);
            if (contains == false)
            {
                return true;
            }
            
            UnlockRequirementsEntity<TID, TCommand> entity = _entityRepository.Get(id);
            bool containsComponent = entity.ContainsComponent(predicate);
            return containsComponent == false;
        }
    }
}
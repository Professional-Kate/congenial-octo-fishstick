using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class FriendlyStatusAssigner : IFriendlyStatusAssigner
    { 
        private readonly ICombatantRepository _combatantRepository;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IFoundAssertion _foundAssertion;

        public FriendlyStatusAssigner(ICombatantRepository combatantRepository, ICollectionAssertion collectionAssertion, IFoundAssertion foundAssertion)
        {
            _combatantRepository = combatantRepository;
            _collectionAssertion = collectionAssertion;
            _foundAssertion = foundAssertion;
        }

        public void AssignFriendlyStatus(byte[] friendlyCombatantIDs, byte[] enemyCombatantIDs)
        {
            AssignComponent(friendlyCombatantIDs, TargetingType.FRIENDLY);
            AssignComponent(enemyCombatantIDs, TargetingType.ENEMY);
        }

        private void AssignComponent(byte[] combatantIDs, TargetingType targetingType)
        {
            _collectionAssertion.AssertHasElements(combatantIDs);
            foreach (byte combatantID in combatantIDs)
            { 
                _foundAssertion.AssertFound(combatantID, _combatantRepository.Contains(combatantID));
                
                CombatantEntity combatantEntity = _combatantRepository.Get(combatantID);
                
                combatantEntity.AddComponent(CreateComponent(targetingType));
                combatantEntity.AddComponent(new CombatParticipantComponent());
            }
        }
        
        private static TargetingTypeComponent CreateComponent(TargetingType targetingType) => new() { TargetingType = targetingType };
    } 
}
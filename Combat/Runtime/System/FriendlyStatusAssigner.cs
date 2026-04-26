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
            AssignComponent(friendlyCombatantIDs, true);
            AssignComponent(enemyCombatantIDs, false);
        }

        private void AssignComponent(byte[] combatantIDs, bool isFriendly)
        {
            _collectionAssertion.AssertHasElements(combatantIDs);
            foreach (byte combatantID in combatantIDs)
            { 
                _foundAssertion.AssertFound(combatantID, _combatantRepository.Contains(combatantID));
                
                CombatantEntity combatantEntity = _combatantRepository.Get(combatantID);
                
                combatantEntity.AddComponent(CreateComponent(isFriendly));
            }
        }
        
        private static FriendlyStatusComponent CreateComponent(bool isFriendly) => new() { IsFriendly = isFriendly };
    } 
}
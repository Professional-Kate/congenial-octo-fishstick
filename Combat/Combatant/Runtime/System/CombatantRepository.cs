using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Core.Contracts.Enum;

namespace IdelPog.Combat.Combatant.Runtime.System
{
    public sealed class CombatantRepository : ICombatantRepository, ICombatantFilters
    {
        private CombatantEntity[] _friendlyEntities = [];
        private CombatantEntity[] _enemyEntities = [];

        public void SeedFriendlyCombatants(CombatantEntity[] friendlyCombatants) => _friendlyEntities = friendlyCombatants;

        public void SeedEnemyCombatants(CombatantEntity[] enemyCombatants) => _enemyEntities = enemyCombatants;
        
        public CombatantEntity Get(byte id)
        { 
            foreach (CombatantEntity combatantEntity in Enumerate())
            {
                if (combatantEntity.InstanceID == id)
                {
                    return combatantEntity;
                }
            }
            
            throw new KeyNotFoundException();
        }

        public IEnumerable<CombatantEntity> Enumerate()
        { 
            foreach (CombatantEntity combatantEntity in _friendlyEntities)
            {
                yield return combatantEntity;
            }

            foreach (CombatantEntity combatantEntity in _enemyEntities)
            {
                yield return combatantEntity;
            }
        }

        public void Clear()
        {
            _friendlyEntities = [];
            _enemyEntities = [];
        }

        public bool HasValidCombatants(TargetingType targetingType)
        {
            foreach (CombatantEntity combatantEntity in EnumerateCombatants(targetingType))
            {
                if (IsCombatantAlive(combatantEntity) == false)
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        public IReadOnlyList<CombatantEntity> GetCombatants(TargetingType targetingType, TargetingType casterTargetingType)
        {

            TargetingType wantedTargetingType;
            if (casterTargetingType == TargetingType.FRIENDLY)
            {
                wantedTargetingType = targetingType == TargetingType.FRIENDLY ? TargetingType.FRIENDLY : TargetingType.ENEMY;
            }
            else
            {
                wantedTargetingType = targetingType == TargetingType.ENEMY ? TargetingType.FRIENDLY : TargetingType.ENEMY;
            }
            
            
            List<CombatantEntity> combatantEntities = [];
            foreach (CombatantEntity combatantEntity in EnumerateCombatants(wantedTargetingType))
            {
                if (targetingType == TargetingType.SELF)
                {
                    continue;
                }
                
                if (IsCombatantAlive(combatantEntity) == false)
                {
                    continue;
                }
                
                combatantEntities.Add(combatantEntity);
            }
            
            return combatantEntities.ToArray();
        }
        
        private IEnumerable<CombatantEntity> EnumerateCombatants(TargetingType targetingType)
        {
            switch (targetingType)
            {
                case TargetingType.FRIENDLY:
                {
                    foreach (CombatantEntity combatantEntity in _friendlyEntities)
                    {
                        yield return combatantEntity;
                    }

                    break;
                }
                case TargetingType.ENEMY:
                {
                    foreach (CombatantEntity combatantEntity in _enemyEntities)
                    {
                        yield return combatantEntity;
                    }

                    break;
                }
            }
        }
        
        private static bool IsCombatantAlive(CombatantEntity combatantEntity) => combatantEntity.GetComponent<LifeStatusComponent>().IsAlive;
    }
}
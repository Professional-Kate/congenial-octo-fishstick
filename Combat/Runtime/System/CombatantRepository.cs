using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Core.Repository.Asset;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class CombatantRepository : ICombatantRepository
    {
        private readonly IAssetRepository<byte, CombatantEntity> _combatantRepository;

        public CombatantRepository(IAssetRepository<byte, CombatantEntity> combatantRepository)
        {
            _combatantRepository = combatantRepository;
        }

        private byte _nextID { get; set; }

        public void Add(CombatantEntity combatantEntity)
        {
            _combatantRepository.Add(_nextID, combatantEntity);
            _nextID++;
        }

        public bool Contains(byte id)
        { 
            return _combatantRepository.Contains(id);
        }

        public void Clear()
        {
            for (byte i = 0; i < _nextID; i++)
            { 
                _combatantRepository.Remove(i);
            }
            
            _nextID = 0;
        }
    }
}
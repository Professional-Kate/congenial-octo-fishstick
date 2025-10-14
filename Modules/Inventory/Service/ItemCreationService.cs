using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Service.Interface;

namespace IdelPog.Inventory.Service
{
    public sealed class ItemCreationService : IItemCreationService
    {
        private readonly IAssetRepository<ItemID, ItemDefinition> _definitionRepository;
        private readonly IFoundAssertion _foundAssertion;

        public ItemCreationService(IAssetRepository<ItemID, ItemDefinition> definitionRepository, IFoundAssertion foundAssertion)
        {
            _definitionRepository = definitionRepository;
            _foundAssertion = foundAssertion;
        }

        public Item Create(ItemID id, uint amount)
        {
            _foundAssertion.AssertFound(id, _definitionRepository.Contains(id));
            
            ItemDefinition definition = _definitionRepository.Get(id);
            return new Item(id, definition.BaseSellPrice, definition.Information, amount);
        }
    }
}
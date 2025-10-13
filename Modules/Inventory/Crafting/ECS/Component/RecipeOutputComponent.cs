using IdelPog.Core.Contracts.Enum;
using IdelPog.ECS.Component;

namespace IdelPog.Inventory.Crafting.ECS.Component
{
    public readonly record struct RecipeOutputComponent : IComponent<RecipeOutputComponent>
    {
        public required ItemID ItemID { get; init; }
        public required uint OutputAmount { get; init; }
        
        public RecipeOutputComponent DeepClone()
        {
            return this;
        }
    }
}
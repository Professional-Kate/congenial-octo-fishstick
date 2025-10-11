using IdelPog.Core.Contracts.Enum;
using IdelPog.ECS.Component;

namespace IdelPog.Inventory.Crafting.Runtime.ECS.Component
{
    public readonly record struct RecipeOutputComponent : IComponent<RecipeOutputComponent>
    {
        public required ItemID ItemID { get; init; }
        public byte OutputAmount { get; init; }
        
        public RecipeOutputComponent DeepClone()
        {
            return this;
        }
    }
}
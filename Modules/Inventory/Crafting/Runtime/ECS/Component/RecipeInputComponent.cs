using IdelPog.Core.Contracts.Enum;
using IdelPog.ECS.Component;

namespace IdelPog.Inventory.Crafting.Runtime.ECS.Component
{
    public readonly record struct RecipeInputComponent : IComponent<RecipeInputComponent>
    {
        public required ItemID ItemID { get; init; }
        public byte RequiredAmount { get; init; }

        public RecipeInputComponent DeepClone()
        {
            return this;
        }
    }
}
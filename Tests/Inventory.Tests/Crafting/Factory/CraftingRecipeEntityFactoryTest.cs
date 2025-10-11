using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.Inventory.Crafting.Contracts;
using IdelPog.Inventory.Crafting.Runtime.ECS;
using IdelPog.Inventory.Crafting.Runtime.Factory;

namespace IdelPog.Inventory.Tests.Crafting.Factory
{
    [TestFixture]
    public sealed class CraftingRecipeEntityFactoryTest
    {
        private CraftingRecipeEntityFactory _entityFactory;
        private RecipeInput _recipeInput;
        private RecipeOutput _recipeOutput;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _recipeInput = new RecipeInput { ItemID = ItemID.SAND, Amount = 4 };
            _recipeOutput = new RecipeOutput { ItemID = ItemID.STONE, Amount = 1 };

            ThrowHandler throwHandler = new();
            _entityFactory = new CraftingRecipeEntityFactory(throwHandler, new CollectionAssertion(throwHandler));
        }

        private static void AssertEntityContains(CraftingRecipeEntity entity, ItemID itemID, bool contains)
        {
            bool containsEntity = entity.ContainsRecipe(component => component.ItemID == itemID);
            Assert.That(containsEntity, Is.EqualTo(contains));
        }
        
        [Test]
        public void Positive_Create_CreatesEntity_WithRecipe()
        {
            CraftingRecipeEntity entity = _entityFactory.Create([_recipeInput], [_recipeOutput]);
            
            AssertEntityContains(entity, _recipeOutput.ItemID, true);
        }

        [Test]
        public void Negative_Create_EmptyInputs_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _entityFactory.Create([], [_recipeOutput]));
        }
        
        [Test]
        public void Negative_Create_NullInputs_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _entityFactory.Create(null!, [_recipeOutput]));
        }
        
        [Test]
        public void Negative_Create_EmptyOutputs_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _entityFactory.Create([_recipeInput], []));
        }
        
        [Test]
        public void Negative_Create_NullOutputs_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _entityFactory.Create([_recipeInput], null!));
        }
    }
}
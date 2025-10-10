using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.Loot.Assertion;
using IdelPog.Loot.Exceptions;
using IdelPog.Loot.Random;
using IdelPog.Loot.Table;
using Moq;

// ReSharper disable ObjectCreationAsStatement

namespace Loot.Tests
{
    [TestFixture]
    public class WeightedLootTableTest
    {
        private WeightedLootTable _weightedLootTable;
        private Mock<ILootRoll> _lootRollMock;

        private WeightedEntry[] _entries;
        private int _weight;

        private const int STONE_WEIGHT = 50;
        private const int COPPER_WEIGHT = 25;
        private const int IRON_WEIGHT = 10;
        private const int GOLD_WEIGHT = 5;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _lootRollMock = new Mock<ILootRoll>();
            
            _entries =
            [
                new WeightedEntry { ItemID = ItemID.STONE, Weight = STONE_WEIGHT },
                new WeightedEntry { ItemID = ItemID.COPPER, Weight = COPPER_WEIGHT },
                new WeightedEntry { ItemID = ItemID.IRON, Weight = IRON_WEIGHT },
                new WeightedEntry { ItemID = ItemID.GOLD, Weight = GOLD_WEIGHT }
            ];
            
            foreach (WeightedEntry weightedEntry in _entries)
            {
                _weight += weightedEntry.Weight;
            }
            
            _weightedLootTable = new WeightedLootTable(_entries, _lootRollMock.Object, new CollectionAssertion(new ThrowHandler()), new WeightAssertion(new ThrowHandler()));
        }

        [Test]
        public void Positive_Roll_AllPossibleValues_MapsToExpectedWeights()
        {
            Dictionary<ItemID, int> counters = new()
            {
                { ItemID.STONE, 0 },
                { ItemID.COPPER, 0 },
                { ItemID.IRON, 0 },
                { ItemID.GOLD, 0 }
            };
            
            for (int i = 0; i < _weight; i++)
            {
                _lootRollMock.Setup(library => library.ExclusiveNextInt(0, _weight)).Returns(i);
                
                ItemID returnedItemID = _weightedLootTable.Roll();
                counters[returnedItemID]++;

                _lootRollMock.Reset();
            }
            
            Assert.Multiple(() =>
            {
                Assert.That(counters[ItemID.STONE], Is.EqualTo(STONE_WEIGHT));
                Assert.That(counters[ItemID.COPPER], Is.EqualTo(COPPER_WEIGHT));
                Assert.That(counters[ItemID.IRON], Is.EqualTo(IRON_WEIGHT));
                Assert.That(counters[ItemID.GOLD], Is.EqualTo(GOLD_WEIGHT));
            });
        }

        [Test]
        public void Negative_Roll_WeightOutOfRange_Throws()
        {
            _lootRollMock.Setup(library => library.ExclusiveNextInt(0, _weight)).Returns(_weight + 1);
            
            Assert.Throws<InvalidOperationException>(() => _weightedLootTable.Roll());
        }

        [Test]
        public void Negative_ConstructWithEmptyCollection_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => new WeightedLootTable([], _lootRollMock.Object, new CollectionAssertion(new ThrowHandler()), new WeightAssertion(new ThrowHandler())));
        }
        
        [Test]
        public void Negative_ConstructWithNullCollection_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new WeightedLootTable(null!, _lootRollMock.Object, new CollectionAssertion(new ThrowHandler()), new WeightAssertion(new ThrowHandler())));
        }

        [TestCase(0)]
        [TestCase(1)]
        public void Positive_ConstructWithPositiveWeight_NoThrow(int weight)
        {
            Assert.DoesNotThrow(() => new WeightedLootTable([new WeightedEntry { ItemID = ItemID.STONE, Weight = weight}], _lootRollMock.Object, new CollectionAssertion(new ThrowHandler()), new WeightAssertion(new ThrowHandler())));
        }

        [Test]
        public void Negative_ConstructWithNegativeWeight_Throws()
        { 
            Assert.Throws<InvalidWeightException>(() => new WeightedLootTable([new WeightedEntry { ItemID = ItemID.STONE, Weight = -1 }], _lootRollMock.Object, new CollectionAssertion(new ThrowHandler()), new WeightAssertion(new ThrowHandler())));
        }
    }
}
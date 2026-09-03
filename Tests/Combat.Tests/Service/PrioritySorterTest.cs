using IdelPog.Combat.Core.Service;

namespace IdelPog.Combat.Tests.Service
{
    [TestFixture]
    public sealed class PrioritySorterTest
    {
        private PrioritySorter _prioritySorter;

        [OneTimeSetUp]
        public void OneTimeSetup()
        { 
            _prioritySorter = new PrioritySorter();
        }

        private static void AssertCollectionSorted(IReadOnlyList<byte> sortedCollection)
        {
            for (int i = 1; i < sortedCollection.Count; i++)
            {
                Assert.That(sortedCollection[i], Is.GreaterThanOrEqualTo(sortedCollection[i - 1]));
            }
        }

        [TestCase(new byte[] {3, 1, 4, 2, 5, 0})]
        [TestCase(new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9})]
        [TestCase(new byte[] {9, 8, 7, 6, 5})]
        [TestCase(new byte[] {1, 2, 2, 2, 3, 4, 5})]
        [TestCase(new byte[] {9})]
        [TestCase(new byte[] {5, 5, 5, 5, 5})]
        [TestCase(new byte[] {})]
        public void Positive_Sort_SortsCollection(byte[] unsortedBytes)
        {
            IReadOnlyList<byte> sortedCollection = _prioritySorter.Sort(unsortedBytes, number => number);
            Assert.That(sortedCollection, Has.Count.EqualTo(unsortedBytes.Length));

            AssertCollectionSorted(sortedCollection);
        }

        [Test]
        public void Positive_Sort_DoesNotMutateInput()
        {
            byte[] unsortedBytes = [3, 1, 4, 2, 5, 0];
            byte[] expectedBytes = [3, 1, 4, 2, 5, 0];

            _prioritySorter.Sort(unsortedBytes, number => number);

            Assert.That(unsortedBytes, Is.EqualTo(expectedBytes));
        }
        
        [Test]
        public void Positive_Sort_UsesPrioritySelector()
        {
            TestValue[] values =
            [
                new(10, 3),
                new(20, 1),
                new(30, 2)
            ];

            IReadOnlyList<TestValue> sortedCollection = _prioritySorter.Sort(values, value => value.Priority);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(sortedCollection[0].Value, Is.EqualTo(20));
                Assert.That(sortedCollection[1].Value, Is.EqualTo(30));
                Assert.That(sortedCollection[2].Value, Is.EqualTo(10));
            }
        }
        
        [Test]
        public void Positive_Sort_EqualPriorities_KeepsOrder()
        {
            TestValue[] values =
            [
                new(10, 1),
                new(20, 2),
                new(30, 1),
                new(40, 2)
            ];

            IReadOnlyList<TestValue> sortedCollection = _prioritySorter.Sort(values, value => value.Priority);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(sortedCollection[0].Value, Is.EqualTo(10));
                Assert.That(sortedCollection[1].Value, Is.EqualTo(30));
                Assert.That(sortedCollection[2].Value, Is.EqualTo(20));
                Assert.That(sortedCollection[3].Value, Is.EqualTo(40));
            }
        }
    }
    
    internal readonly record struct TestValue(byte Value, byte Priority);
}
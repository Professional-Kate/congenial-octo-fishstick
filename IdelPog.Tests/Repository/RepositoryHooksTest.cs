using IdelPog.Engine.Models;
using IdelPogTests.Utils;

namespace IdelPogTests.Repository
{
    [TestFixture]
    public class RepositoryHooksTest : HookHandler
    {
        private const int KEY = 1;
        private Currency _currency { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _currency = CurrencyFactory.CreateWood();
        }
        
        [Test]
        public void Positive_Add_CallsOAdd()
        {
            TestRepository.Add(KEY, _currency);

            Assert.That(AddEventTriggered, Is.True);
        }

        [Test]
        public void Positive_Remove_CallsOnRemove()
        {            
            TestRepository.Add(KEY, _currency);

            TestRepository.Remove(KEY);

            Assert.That(RemoveEventTriggered, Is.True);
        }
        
        [Test]
        public void Positive_Update_CallsOnUpdate()
        {
            TestRepository.Add(KEY, _currency);

            TestRepository.Update(KEY, _currency);

            Assert.That(UpdateEventTriggered, Is.True);
        }

        [Test]
        public void Positive_Contains_CallsOnContains()
        {
            TestRepository.Contains(KEY);
            
            Assert.That(ContainsEventTriggered, Is.True);
        }

        [Test]
        public void Positive_Get_CallsOnGet()
        {
            TestRepository.Add(KEY, _currency);

            TestRepository.Get(KEY);
            
            Assert.That(GetEventTriggered, Is.True);
        }
    }
}
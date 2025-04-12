using NUnit.Framework;

namespace IdelPog.Tests.Repository
{
    [TestFixture]
    public class RepositoryHooksTest : HookHandler
    {
        private const int KEY = 1;
        private const string VALUE = "VALUE";
       
        [Test]
        public void Positive_Add_CallsOAdd()
        {
            TestRepository.Add(KEY, VALUE);

            Assert.That(AddEventTriggered, Is.True);
        }

        [Test]
        public void Positive_Remove_CallsOnRemove()
        {            
            TestRepository.Add(KEY, VALUE);

            TestRepository.Remove(KEY);

            Assert.That(RemoveEventTriggered, Is.True);
        }
        
        [Test]
        public void Positive_Update_CallsOnUpdate()
        {
            TestRepository.Add(KEY, VALUE);

            TestRepository.Update(KEY, VALUE);

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
            TestRepository.Add(KEY, VALUE);

            TestRepository.Get(KEY);
            
            Assert.That(GetEventTriggered, Is.True);
        }
    }
}
using NUnit.Framework;

namespace Tests.Repository
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

            Assert.IsTrue(AddEventTriggered);
        }

        [Test]
        public void Positive_Remove_CallsOnRemove()
        {            
            TestRepository.Add(KEY, VALUE);

            TestRepository.Remove(KEY);

            Assert.IsTrue(RemoveEventTriggered);
        }
        
        [Test]
        public void Positive_Update_CallsOnUpdate()
        {
            TestRepository.Add(KEY, VALUE);

            TestRepository.Update(KEY, VALUE);

            Assert.IsTrue(UpdateEventTriggered);
        }

        [Test]
        public void Positive_Contains_CallsOnContains()
        {
            TestRepository.Contains(KEY);
            
            Assert.IsTrue(ContainsEventTriggered);
        }

        [Test]
        public void Positive_Get_CallsOnGet()
        {
            TestRepository.Add(KEY, VALUE);

            TestRepository.Get(KEY);
            
            Assert.IsTrue(GetEventTriggered);
        }
    }
}
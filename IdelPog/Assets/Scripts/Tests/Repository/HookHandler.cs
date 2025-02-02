using IdelPog.Repository;
using NUnit.Framework;

namespace Tests.Repository
{
    public class HookHandler
    {
        protected IRepository<int, string> TestRepository;

        protected bool AddEventTriggered;
        protected bool RemoveEventTriggered;
        protected bool GetEventTriggered;
        protected bool UpdateEventTriggered;
        protected bool ContainsEventTriggered;

        [SetUp]
        public void SetUp()
        { 
            TestRepository = new Repository<int, string>();
            
            AddEventTriggered = false;
            RemoveEventTriggered = false;
            GetEventTriggered = false;
            UpdateEventTriggered = false;
            ContainsEventTriggered = false;
            
            TestRepository.OnAdd += OnAdd;
            TestRepository.OnRemove += OnRemove;
            TestRepository.OnGet += OnGet;
            TestRepository.OnUpdate += OnUpdate;
            TestRepository.OnContains += OnContains;
        }

        private void OnAdd(int key, string value)
        {
            AddEventTriggered = true;
        }

        private void OnRemove(int key, string value)
        {
            RemoveEventTriggered = true;
        }

        private void OnGet(int key, string value)
        {
            GetEventTriggered = true;
        }

        private void OnUpdate(string originalValue, string value)
        {
            UpdateEventTriggered = true;
        }

        private void OnContains(int key, bool contains)
        {
            ContainsEventTriggered = true;
        }
    }
}
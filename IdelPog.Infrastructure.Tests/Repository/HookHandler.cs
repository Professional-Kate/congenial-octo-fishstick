using IdelPog.Engine.Models;
using IdelPog.Infrastructure.Repository;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.Infrastructure.Tests.Repository
{
    public class HookHandler
    {
        protected IRepository<int, Currency> TestRepository;

        protected bool AddEventTriggered;
        protected bool RemoveEventTriggered;
        protected bool GetEventTriggered;
        protected bool UpdateEventTriggered;
        protected bool ContainsEventTriggered;

        [SetUp]
        public void SetUp()
        {
            IHandler handler = new ThrowHandler();
            IRepositoryAsserter repositoryAsserter = new RepositoryAsserter(new AssertFound(handler), new AssertNotNull(handler), new AssertNonDuplicate(handler));
            TestRepository = new Repository<int, Currency>(repositoryAsserter);
            
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

        private void OnAdd(int key, Currency value)
        {
            AddEventTriggered = true;
        }

        private void OnRemove(int key, Currency value)
        {
            RemoveEventTriggered = true;
        }

        private void OnGet(int key, Currency value)
        {
            GetEventTriggered = true;
        }

        private void OnUpdate(Currency originalValue, Currency value)
        {
            UpdateEventTriggered = true;
        }

        private void OnContains(int key, bool contains)
        {
            ContainsEventTriggered = true;
        }
    }
}
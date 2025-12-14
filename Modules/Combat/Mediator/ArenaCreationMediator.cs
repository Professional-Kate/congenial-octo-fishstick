using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Factory.Interface;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Progression.Assertion;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Mediator
{
    public sealed class ArenaCreationMediator : IBatchMediator<ArenaCreation>
    {
        private readonly IStateRepository<ArenaType, Arena> _arenaRepository;
        private readonly IArenaFactory _arenaFactory;
        private readonly IDispatchMany<ArenaCreationResponse> _responseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IUniqueAssertion _uniqueAssertion;
        private readonly ILevelAssertion _levelAssertion;

        public ArenaCreationMediator(IStateRepository<ArenaType, Arena> arenaRepository, IArenaFactory arenaFactory, IDispatchMany<ArenaCreationResponse> responseDispatcher, ICollectionAssertion collectionAssertion, IUniqueAssertion uniqueAssertion, ILevelAssertion levelAssertion)
        {
            _arenaRepository = arenaRepository;
            _arenaFactory = arenaFactory;
            _responseDispatcher = responseDispatcher;
            _collectionAssertion = collectionAssertion;
            _uniqueAssertion = uniqueAssertion;
            _levelAssertion = levelAssertion;
        }

        public void HandleMessages(IReadOnlyList<ArenaCreation> messages)
        {
            _collectionAssertion.AssertHasElements(messages);

            ArenaCreationResponse[] responses = new ArenaCreationResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                ArenaCreation creation = messages[i];
                _uniqueAssertion.AssertUnique(creation.ArenaType, _arenaRepository.Contains(creation.ArenaType));

                Arena arena = _arenaFactory.Create(creation);
                _levelAssertion.AssertNotAboveMaxLevel(arena.Levelable);
                
                _arenaRepository.Add(creation.ArenaType, arena);
                
                responses[i] = new ArenaCreationResponse { Information = creation.Information, ReadOnlyLevelable = creation.ReadOnlyLevelable, ArenaType = creation.ArenaType };
            }
            
            _responseDispatcher.Dispatch(responses);
        }
    }
}
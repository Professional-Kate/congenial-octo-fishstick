using Console.Commands.Domains;
using Console.Commands.Resolver;
using Console.Types;
using IdelPog.Common.Repository;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Exceptions;
using Moq;

namespace Console.Tests.Resolver
{
    public class CommandResolverMediatorTest
    {
        private ICommandResolverMediator _commandResolverMediator { get; set; }
        private Mock<IStateRepository<CommandDomain, ICommandDomainResolver>> _stateRepositoryMock { get; set; }
        private Mock<ICommandDomainResolver>  _commandDomainResolverMock { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _stateRepositoryMock = new Mock<IStateRepository<CommandDomain, ICommandDomainResolver>>();
            _commandDomainResolverMock = new Mock<ICommandDomainResolver>();
            _commandResolverMediator = new CommandResolverMediator(_stateRepositoryMock.Object, new AssertNotNull(new ThrowHandler()), new AssertFound(new ThrowHandler()), new AssertCollectionNotEmpty(new ThrowHandler()));
        }

        [SetUp]
        public void SetUp()
        {
            _stateRepositoryMock.Reset();
            _commandDomainResolverMock.Reset();
        }

        [Test]
        public void Positive_ResolveCommand_ResolvesToCorrectType()
        {
            _stateRepositoryMock.Setup(library => library.Contains(CommandDomain.CURRENCY)).Returns(true);
            _stateRepositoryMock.Setup(library => library.Get(CommandDomain.CURRENCY)).Returns(_commandDomainResolverMock.Object);
            
            Assert.DoesNotThrow(() => _commandResolverMediator.ResolveCommand(CommandDomain.CURRENCY, ["ADD", "10"]));
            
            _stateRepositoryMock.Verify(library => library.Contains(CommandDomain.CURRENCY), Times.Once);
            _stateRepositoryMock.Verify(library => library.Get(CommandDomain.CURRENCY), Times.Once);
            _commandDomainResolverMock.Verify(library => library.Resolve("ADD", new[] {"10"}), Times.Once);
        }
        
        [Test]
        public void Negative_ResolveCommand_EmptyArgsArray_Throws()
        {
            Assert.Throws<CollectionEmptyException>(() => _commandResolverMediator.ResolveCommand(CommandDomain.CURRENCY, []));
        }
        
        [Test]
        public void Negative_ResolveCommand_NullArgsArray_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _commandResolverMediator.ResolveCommand(CommandDomain.CURRENCY, null!));
        }

        [Test]
        public void Negative_ResolveCommand_NoResolverFound_Throws()
        {
            Assert.Throws<NotFoundException>(() => _commandResolverMediator.ResolveCommand(CommandDomain.CURRENCY, ["10, 10, 10"]));
        }
    }
}
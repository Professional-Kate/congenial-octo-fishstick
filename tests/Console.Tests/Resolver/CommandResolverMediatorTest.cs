using Console.Assertions;
using Console.Commands;
using Console.Commands.Domains;
using Console.Runtime.Input.Exceptions;
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
        private Mock<IAssetRepository<CommandDomain, ICommandDomainResolver>> _stateRepositoryMock { get; set; }
        private Mock<ICommandDomainResolver>  _commandDomainResolverMock { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _stateRepositoryMock = new Mock<IAssetRepository<CommandDomain, ICommandDomainResolver>>();
            _commandDomainResolverMock = new Mock<ICommandDomainResolver>();
            _commandResolverMediator = new CommandResolverMediator(_stateRepositoryMock.Object, new AssertFound(new ThrowHandler()), new AssertSpanNotEmpty(new ThrowHandler()));
        }

        [SetUp]
        public void SetUp()
        {
            _stateRepositoryMock.Reset();
            _commandDomainResolverMock.Reset();
        }
        
        [Test]
        public void Negative_ResolveCommand_EmptyArgsArray_Throws()
        {
            Assert.Throws<EmptySpanException>(() => _commandResolverMediator.ResolveCommand(CommandDomain.CURRENCY, []));
        }
        
        [Test]
        public void Negative_ResolveCommand_NullArgsArray_Throws()
        {
            Assert.Throws<EmptySpanException>(() => _commandResolverMediator.ResolveCommand(CommandDomain.CURRENCY, null!));
        }

        [Test]
        public void Negative_ResolveCommand_NoResolverFound_Throws()
        {
            Assert.Throws<NotFoundException>(() => _commandResolverMediator.ResolveCommand(CommandDomain.CURRENCY, ["10, 10, 10"]));
        }
    }
}
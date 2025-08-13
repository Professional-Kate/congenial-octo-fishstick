using IdelPog.Console.Assertion;
using IdelPog.Console.Command.Domain;
using IdelPog.Console.Command.Mediator;
using IdelPog.Console.Runtime.Input.Exceptions;
using IdelPog.Console.Runtime.System;
using IdelPog.Console.Types;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using Moq;

namespace IdelPog.Console.Tests.Resolver
{
    public class CommandResolverMediatorTest
    {
        private ICommandResolverMediator _commandResolverMediator { get; set; }
        private Mock<IAssetRepository<Domain, ICommandDomainResolver>> _repositoryMock { get; set; }
        private Mock<ICommandDomainResolver> _commandDomainResolverMock { get; set; }
        private Mock<IDomainPermissionChecker> _domainPermissionCheckerMock { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<IAssetRepository<Domain, ICommandDomainResolver>>();
            _commandDomainResolverMock = new Mock<ICommandDomainResolver>();
            _domainPermissionCheckerMock = new Mock<IDomainPermissionChecker>();
            _commandResolverMediator = new CommandResolverMediator(_repositoryMock.Object, _domainPermissionCheckerMock.Object,
                new FoundAssertion(new ThrowHandler()), new SpanAssertion(new ThrowHandler()), new DomainPermissionAssertion(new ThrowHandler()));
        }

        [SetUp]
        public void SetUp()
        {
            _repositoryMock.Reset();
            _commandDomainResolverMock.Reset();
        }

        [Test]
        public void Negative_ResolveCommand_EmptyArgsArray_Throws()
        {
            Assert.Throws<EmptySpanException>(() => _commandResolverMediator.ResolveCommand(Domain.CURRENCY, []));
        }

        [Test]
        public void Negative_ResolveCommand_NullArgsArray_Throws()
        {
            Assert.Throws<EmptySpanException>(() => _commandResolverMediator.ResolveCommand(Domain.CURRENCY, null!));
        }

        [Test]
        public void Negative_ResolveCommand_NoResolverFound_Throws()
        {
            NotFoundException<Domain> exception =
                Assert.Throws<NotFoundException<Domain>>(() => _commandResolverMediator.ResolveCommand(Domain.CURRENCY, ["10, 10, 10"]));

            Assert.That(exception.Key, Is.EqualTo(Domain.CURRENCY));
        }
    }
}
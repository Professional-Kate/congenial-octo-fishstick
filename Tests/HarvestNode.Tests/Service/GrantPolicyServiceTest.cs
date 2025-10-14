using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.HarvestNode.Contracts;
using IdelPog.HarvestNode.Runtime.Factory.Interface;
using IdelPog.HarvestNode.Runtime.System;
using IdelPog.HarvestNode.Runtime.System.Interface;
using IdelPog.Loot.Policy;
using IdelPog.Loot.Random;
using Moq;

namespace IdelPog.HarvestNode.Tests.Service
{
    [TestFixture]
    public sealed class GrantPolicyServiceTest
    {
        private IGrantPolicyService<ResourceID> _grantPolicyService;
        private Mock<IAssetRepository<ResourceID, IGrantPolicy>> _repositoryMock;
        private Mock<IWeightedPolicyFactory> _factoryMock;
        
        private GrantPolicyEntry _grantPolicyEntry;
        private const ResourceID RESOURCE_ID = ResourceID.LEAF_LITTER;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new  Mock<IAssetRepository<ResourceID, IGrantPolicy>>();
            _factoryMock = new  Mock<IWeightedPolicyFactory>();
            
            _grantPolicyService = new GrantPolicyService<ResourceID>(_repositoryMock.Object, _factoryMock.Object, new UniqueAssertion());

            _grantPolicyEntry = new GrantPolicyEntry { GrantWeight = 1, SkipWeight = 0 };
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock.Reset();
            _factoryMock.Reset();
        }

        private void VerifyRepositoryContains(ResourceID resourceID)
        {
            _repositoryMock.Verify(library => library.Contains(resourceID), Times.Once);
        }

        private void VerifyRepositoryAdd(ResourceID resourceID)
        {
            _repositoryMock.Verify(library => library.Add(resourceID, It.IsAny<IGrantPolicy>()), Times.Once);
        }

        private void VerifyRepositoryNoMoreCalls()
        {
            _repositoryMock.VerifyNoOtherCalls();
        }

        private void VerifyFactoryCalled(GrantPolicyEntry grantPolicyEntry)
        {
            _factoryMock.Verify(library => library.Create(grantPolicyEntry, It.IsAny<ILootRoll>()), Times.Once);
        }

        private void VerifyFactoryNoMoreCalls()
        {
            _factoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_CreateGrantPolicy_ZeroSkipWeight_CreatesAndAddsPolicy_SkipsFactory()
        {
            Assert.DoesNotThrow(() => _grantPolicyService.CreateGrantPolicy(_grantPolicyEntry, RESOURCE_ID));

            VerifyRepositoryContains(RESOURCE_ID);
            VerifyRepositoryAdd(RESOURCE_ID);
            VerifyRepositoryNoMoreCalls();
            VerifyFactoryNoMoreCalls();
        }

        [Test]
        public void Positive_CreateGrantPolicy_ZeroGrantWeight_CreatesAndAddsPolicy_SkipsFactory()
        {
            GrantPolicyEntry entry = new() { GrantWeight = 0, SkipWeight = 10 };
            
            Assert.DoesNotThrow(() => _grantPolicyService.CreateGrantPolicy(entry, RESOURCE_ID));

            VerifyRepositoryContains(RESOURCE_ID);
            VerifyRepositoryAdd(RESOURCE_ID);
            VerifyRepositoryNoMoreCalls();
            VerifyFactoryNoMoreCalls();
        }
        
        [Test]
        public void Positive_CreateGrantPolicy_CreatesAndAddsPolicy()
        {
            GrantPolicyEntry entry = _grantPolicyEntry with { SkipWeight = 10 };
            
            Assert.DoesNotThrow(() => _grantPolicyService.CreateGrantPolicy(entry, RESOURCE_ID));

            VerifyRepositoryContains(RESOURCE_ID);
            VerifyRepositoryAdd(RESOURCE_ID);
            VerifyRepositoryNoMoreCalls();
            VerifyFactoryCalled(entry);
        }

        [Test]
        public void Negative_CreateGrantPolicy_ResourceID_NotUnique_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(RESOURCE_ID)).Returns(true);
            
            Assert.Throws<DuplicateEntityException>(() => _grantPolicyService.CreateGrantPolicy(_grantPolicyEntry, RESOURCE_ID));
            
            VerifyRepositoryContains(RESOURCE_ID);
            VerifyRepositoryNoMoreCalls();
            VerifyFactoryNoMoreCalls();
        }
    }
}
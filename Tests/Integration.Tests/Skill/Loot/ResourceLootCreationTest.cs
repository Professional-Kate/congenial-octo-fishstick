using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.HarvestNode.Contracts;
using IdelPog.HarvestNode.Contracts.Command;
using IdelPog.HarvestNode.Contracts.Error;
using IdelPog.HarvestNode.Contracts.Response;
using IdelPog.Loot.Exceptions;

namespace IdelPog.Integration.Tests.Skill.Loot
{
    public sealed class ResourceLootCreationTest : ManagedTestBuffer
    {
        private ResourceLootCreation _beeHiveLootCreation;

        private ManagedResponseListener<ResourceLootCreationResponse> _responseListener;
        private ManagedErrorListener<ResourceLootCreationError> _errorListener;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _beeHiveLootCreation = new ResourceLootCreation
            {
                ResourceID = ResourceID.BEEHIVE,
                LootTableEntries = [new LootTableEntry { ItemID = ItemID.HONEY, Weight = 10 }, new LootTableEntry { ItemID = ItemID.SMALL_INSECTS, Weight = 1 }],
                GrantPolicyEntry = new GrantPolicyEntry { GrantWeight = 1, SkipWeight = 0 }
            };
        }

        [SetUp]
        public void Setup()
        {
            _responseListener = new ManagedResponseListener<ResourceLootCreationResponse>();
            _errorListener = new ManagedErrorListener<ResourceLootCreationError>();
            
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
        }
        
        private void VerifyResponseListenerCalled(bool wasCalled)
        {
            Assert.That(_responseListener.WasCalled, Is.EqualTo(wasCalled));
        }

        private void VerifyResponseLength(int length)
        {
            Assert.That(_responseListener.Responses, Has.Length.EqualTo(length));
        }

        private static void VerifyResponse(ResourceLootCreationResponse response, ResourceLootCreation creation)
        {
            Assert.Multiple(() =>
            {
                Assert.That(response.ResourceID, Is.EqualTo(creation.ResourceID));
                Assert.That(response.LootTableEntries, Is.EqualTo(creation.LootTableEntries));
            });
        }
        
        private void VerifyErrorListenerCalled(bool wasCalled)
        {
            Assert.That(_errorListener.WasCalled, Is.EqualTo(wasCalled));
        }
        
        private void VerifyErrorLength(int length)
        {
            Assert.That(_errorListener.Error.HarvestNodeLootCreations, Has.Length.EqualTo(length));
        }

        private void VerifyError(Type exception, params ResourceLootCreation[] creations)
        {
            ResourceLootCreationError error = _errorListener.Error;
            Assert.Multiple(() =>
            {
                Assert.That(error.BaseError.Exception, Is.TypeOf<ControllerThrownException>());
                Assert.That(error.BaseError.Exception.InnerException, Is.TypeOf(exception));
                Assert.That(error.HarvestNodeLootCreations, Is.EqualTo(creations));
            });
        }
        
        private void DispatchResourceLootCreation(params ResourceLootCreation[] lootCreations)
        {
            IBuffer<ResourceLootCreation> buffer = BufferManager.RequestBuffer<ResourceLootCreation>(new BufferRequest(lootCreations.Length));
            buffer.Assign(lootCreations);
            buffer.MarkReady();
        }

        [Test]
        public void Positive_SendSingleCommand_CreatesResourceLoot_DispatchesResponse()
        {
            Assert.DoesNotThrow(() => DispatchResourceLootCreation(_beeHiveLootCreation));
            
            VerifyResponseListenerCalled(true);
            VerifyErrorListenerCalled(false);
            VerifyResponseLength(1);
            VerifyResponse(_responseListener.Responses[0], _beeHiveLootCreation);
        }

        [Test]
        public void Positive_SendMultipleCommands_DifferentResourceID_DispatchesResponse()
        {
            ResourceLootCreation leafLitterCreation = new()
            {
                ResourceID = ResourceID.LEAF_LITTER,
                LootTableEntries = [new LootTableEntry { ItemID = ItemID.HERBS, Weight = 10 }],
                GrantPolicyEntry = new GrantPolicyEntry { GrantWeight = 1, SkipWeight = 1 }
            };
            
            Assert.DoesNotThrow(() => DispatchResourceLootCreation(_beeHiveLootCreation, leafLitterCreation));
            
            VerifyResponseListenerCalled(true);
            VerifyErrorListenerCalled(false);
            VerifyResponseLength(2);
            VerifyResponse(_responseListener.Responses[0], _beeHiveLootCreation);
            VerifyResponse(_responseListener.Responses[1], leafLitterCreation);
        }

        [Test]
        public void Negative_SendMultipleCommands_SameResourceID_DispatchesError()
        {
            ResourceLootCreation duplicateCreation = _beeHiveLootCreation with
            {
                LootTableEntries = [new LootTableEntry { ItemID = ItemID.HERBS, Weight = 10 }]
            };
            
            Assert.DoesNotThrow(() => DispatchResourceLootCreation(duplicateCreation, _beeHiveLootCreation));
            
            VerifyResponseListenerCalled(false);
            VerifyErrorListenerCalled(true);
            VerifyErrorLength(2);
            VerifyError(typeof(DuplicateEntityException), duplicateCreation, _beeHiveLootCreation);
        }

        [Test]
        public void Negative_SendSingleCommand_EmptyLootTableEntries_DispatchesError()
        {
            ResourceLootCreation emptyCreation = _beeHiveLootCreation with { LootTableEntries = [] };
            
            Assert.DoesNotThrow(() => DispatchResourceLootCreation(emptyCreation));
            
            VerifyResponseListenerCalled(false);
            VerifyErrorListenerCalled(true);
            VerifyErrorLength(1);
            VerifyError(typeof(EmptyCollectionException), emptyCreation);
        }
        
        [Test]
        public void Negative_SendSingleCommand_GrantPolicy_NegativeGrantWeight_DispatchesError()
        {
            ResourceLootCreation negativeCreation = _beeHiveLootCreation with { GrantPolicyEntry = new GrantPolicyEntry { SkipWeight = 1, GrantWeight = -10 } };
            
            Assert.DoesNotThrow(() => DispatchResourceLootCreation(negativeCreation));

            VerifyResponseListenerCalled(false);
            VerifyErrorListenerCalled(true);
            VerifyErrorLength(1);
            VerifyError(typeof(InvalidWeightException), negativeCreation);
        }
        
        [Test]
        public void Negative_SendSingleCommand_GrantPolicy_NegativeSkipWeight_DispatchesError()
        {
            ResourceLootCreation negativeCreation = _beeHiveLootCreation with { GrantPolicyEntry = new GrantPolicyEntry { SkipWeight = -1, GrantWeight = 1 } };
            
            Assert.DoesNotThrow(() => DispatchResourceLootCreation(negativeCreation));

            VerifyResponseListenerCalled(false);
            VerifyErrorListenerCalled(true);
            VerifyErrorLength(1);
            VerifyError(typeof(InvalidWeightException), negativeCreation);
        }
        
        [Test]
        public void Negative_SendSingleCommand_MultipleLootTableEntry_NegativeWeight_DispatchesResponse()
        {
            ResourceLootCreation negativeCreation = _beeHiveLootCreation with
            {
                LootTableEntries = [ new LootTableEntry { ItemID = ItemID.BIRCH, Weight = -10 }, new LootTableEntry { ItemID = ItemID.OAK,  Weight = 1 } ] 
            };
            
            Assert.DoesNotThrow(() => DispatchResourceLootCreation(negativeCreation));

            VerifyResponseListenerCalled(false);
            VerifyErrorListenerCalled(true);
            VerifyErrorLength(1);
            VerifyError(typeof(InvalidWeightException), negativeCreation);
        }

        [Test]
        public void Positive_SendSingleCommand_SingleLootTableEntry_NegativeWeight_DispatchesResponse()
        {
            // in cases of single entries the weight won't be read
            ResourceLootCreation negativeCreation = _beeHiveLootCreation with { LootTableEntries = [ new LootTableEntry { ItemID = ItemID.HERBS, Weight = -10 }] };
            
            Assert.DoesNotThrow(() => DispatchResourceLootCreation(negativeCreation));
            
            VerifyResponseListenerCalled(true);
            VerifyErrorListenerCalled(false);
            VerifyResponseLength(1);
            VerifyResponse(_responseListener.Responses[0], negativeCreation);
        }
    }
}
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
    [TestFixture]
    public sealed class LocationLootCreationTest : ManagedTestBuffer
    {
        private LocationLootCreation _forestLootCreation;
        
        private ManagedResponseListener<LocationLootCreationResponse> _responseListener;
        private ManagedErrorListener<LocationLootCreationError> _errorListener;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _forestLootCreation = new LocationLootCreation
            {
                ResourceID = ResourceID.ANT_NEST,
                LocationID = LocationID.FOREST,
                LootTableEntries = [ new LootTableEntry { ItemID = ItemID.SMALL_INSECTS, Weight = 2 }, new LootTableEntry { ItemID = ItemID.HERBS,  Weight = 1 } ],
                GrantPolicyEntry = new GrantPolicyEntry { GrantWeight = 1, SkipWeight = 1 }
            };
        }
        
        [SetUp]
        public void Setup()
        {
            _responseListener = new ManagedResponseListener<LocationLootCreationResponse>();
            _errorListener = new ManagedErrorListener<LocationLootCreationError>();
            
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
        }
        
        private void DispatchLocationLootCreation(params LocationLootCreation[] lootCreations)
        {
            IBuffer<LocationLootCreation> buffer = BufferManager.RequestBuffer<LocationLootCreation>(new BufferRequest(lootCreations.Length));
            buffer.Assign(lootCreations);
            buffer.MarkReady();
        }

        private void VerifyResponseListenerCalled(bool wasCalled)
        {
            Assert.That(_responseListener.WasCalled, Is.EqualTo(wasCalled));
        }

        private void VerifyResponseLength(int length)
        {
            Assert.That(_responseListener.Responses, Has.Length.EqualTo(length));
        }

        private static void VerifyResponse(LocationLootCreationResponse response, LocationLootCreation creation)
        {
            Assert.Multiple(() =>
            {
                Assert.That(response.ResourceID, Is.EqualTo(creation.ResourceID));
                Assert.That(response.LocationID, Is.EqualTo(creation.LocationID));
                Assert.That(response.LootTableEntries, Is.EqualTo(creation.LootTableEntries));
            });
        }
        
        private void VerifyErrorListenerCalled(bool wasCalled)
        {
            Assert.That(_errorListener.WasCalled, Is.EqualTo(wasCalled));
        }
        
        private void VerifyErrorLength(int length)
        {
            Assert.That(_errorListener.Error.LocationLootCreations, Has.Length.EqualTo(length));
        }

        private void VerifyError(Type exception, params LocationLootCreation[] creations)
        {
            LocationLootCreationError error = _errorListener.Error;
            Assert.Multiple(() =>
            {
                Assert.That(error.BaseError.Exception, Is.TypeOf<ControllerThrownException>());
                Assert.That(error.BaseError.Exception.InnerException, Is.TypeOf(exception));
                Assert.That(error.LocationLootCreations, Is.EqualTo(creations));
            });
        }

        [Test]
        public void Positive_SendCreationCommand_CreatesLocationLoot_DispatchesResponse()
        {
            Assert.DoesNotThrow(() => DispatchLocationLootCreation(_forestLootCreation));

            VerifyResponseListenerCalled(true);
            VerifyErrorListenerCalled(false);
            VerifyResponseLength(1);
            VerifyResponse(_responseListener.Responses[0], _forestLootCreation);
        }

        [Test]
        public void Positive_SendMultipleCommands_DifferentLocationID_DispatchesResponses()
        {
            LocationLootCreation birchCreation = new()
            {
                ResourceID = ResourceID.STONE,
                LocationID = LocationID.CAVE,
                LootTableEntries = [ new LootTableEntry { ItemID = ItemID.STONE, Weight = 10 }, new LootTableEntry { ItemID = ItemID.DIAMOND, Weight = 1 } ],
                GrantPolicyEntry = new GrantPolicyEntry { GrantWeight = 1, SkipWeight = 1 }
            };
            
            Assert.DoesNotThrow(() => DispatchLocationLootCreation(_forestLootCreation, birchCreation));

            VerifyResponseListenerCalled(true);
            VerifyErrorListenerCalled(false);
            VerifyResponseLength(2);
            VerifyResponse(_responseListener.Responses[0], _forestLootCreation);
            VerifyResponse(_responseListener.Responses[1], birchCreation);
        }

        [Test]
        public void Negative_SendMultipleCommands_SameLocationID_DispatchesError()
        {
            LocationLootCreation duplicateCreation = new()
            {
                ResourceID = ResourceID.LEAF_LITTER,
                LocationID = LocationID.FOREST,
                LootTableEntries = [ new LootTableEntry { ItemID = ItemID.EMERALD, Weight = 1 } ],
                GrantPolicyEntry = new GrantPolicyEntry { GrantWeight = 1, SkipWeight = 100 }
            };
            
            Assert.DoesNotThrow(() => DispatchLocationLootCreation(_forestLootCreation, duplicateCreation));

            VerifyResponseListenerCalled(false);
            VerifyErrorListenerCalled(true);
            VerifyErrorLength(2);
            VerifyError(typeof(DuplicateEntityException), _forestLootCreation, duplicateCreation);
        }

        [Test]
        public void Negative_SendSingleCommand_EmptyLootTableEntries_DispatchesError()
        {
            LocationLootCreation emptyCreation = _forestLootCreation with { LootTableEntries = [] };
            
            Assert.DoesNotThrow(() => DispatchLocationLootCreation(emptyCreation));

            VerifyResponseListenerCalled(false);
            VerifyErrorListenerCalled(true);
            VerifyErrorLength(1);
            VerifyError(typeof(EmptyCollectionException), emptyCreation);
        }

        [Test]
        public void Negative_SendSingleCommand_GrantPolicy_NegativeGrantWeight_DispatchesError()
        {
            LocationLootCreation negativeCreation = _forestLootCreation with { GrantPolicyEntry = new GrantPolicyEntry { SkipWeight = 1, GrantWeight = -10 } };
            
            Assert.DoesNotThrow(() => DispatchLocationLootCreation(negativeCreation));

            VerifyResponseListenerCalled(false);
            VerifyErrorListenerCalled(true);
            VerifyErrorLength(1);
            VerifyError(typeof(InvalidWeightException), negativeCreation);
        }
        
        [Test]
        public void Negative_SendSingleCommand_GrantPolicy_NegativeSkipWeight_DispatchesError()
        {
            LocationLootCreation negativeCreation = _forestLootCreation with { GrantPolicyEntry = new GrantPolicyEntry { SkipWeight = -1, GrantWeight = 1 } };
            
            Assert.DoesNotThrow(() => DispatchLocationLootCreation(negativeCreation));

            VerifyResponseListenerCalled(false);
            VerifyErrorListenerCalled(true);
            VerifyErrorLength(1);
            VerifyError(typeof(InvalidWeightException), negativeCreation);
        }
        
        [Test]
        public void Negative_SendSingleCommand_MultipleLootTableEntry_NegativeWeight_DispatchesResponse()
        {
            LocationLootCreation negativeCreation = _forestLootCreation with
            {
                LootTableEntries = [ new LootTableEntry { ItemID = ItemID.EMERALD, Weight = -10 }, new LootTableEntry { ItemID = ItemID.RUBY,  Weight = 1 } ] 
            };
            
            Assert.DoesNotThrow(() => DispatchLocationLootCreation(negativeCreation));

            VerifyResponseListenerCalled(false);
            VerifyErrorListenerCalled(true);
            VerifyErrorLength(1);
            VerifyError(typeof(InvalidWeightException), negativeCreation);
        }
        
        [Test]
        public void Positive_SendSingleCommand_SingleLootTableEntry_NegativeWeight_DispatchesResponse()
        {
            // in cases of single entries the weight won't be read
            LocationLootCreation negativeCreation = _forestLootCreation with { LootTableEntries = [ new LootTableEntry { ItemID = ItemID.EMERALD, Weight = -10 } ] };
            
            Assert.DoesNotThrow(() => DispatchLocationLootCreation(negativeCreation));

            VerifyResponseListenerCalled(true);
            VerifyErrorListenerCalled(false);
            VerifyResponseLength(1);
            VerifyResponse(_responseListener.Responses[0], negativeCreation);
        }
    }
}
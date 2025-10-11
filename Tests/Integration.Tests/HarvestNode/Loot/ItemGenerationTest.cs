using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Information.Contracts;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Progression;
using IdelPog.HarvestNode.Contracts.Command;

namespace IdelPog.Integration.Tests.HarvestNode.Loot
{
    [TestFixture]
    public sealed class ItemGenerationTest : ManagedTestBuffer
    {
        private HarvestNodeUpdate _ironUpdate;
        private HarvestNodeCreation _ironCreation;
        private ResourceLootCreation _ironLootCreation;
        private LocationLootCreation _locationLootCreation;
        private InventoryUpdateListener _inventoryUpdateListener;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _ironUpdate = new HarvestNodeUpdate
            {
                ResourceID = ResourceID.IRON_CLUSTER,
                SkillID = SkillID.MINING
            };

            _ironCreation = new HarvestNodeCreation
            {
                ReadOnlyHarvestNodes =
                [
                    new ReadOnlyHarvestNode
                    {
                        ResourceID = ResourceID.IRON_CLUSTER,
                        LocationID = LocationID.CAVE,
                        ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 },
                        Information = new Information { Name = "", Description = "" }
                    }
                ],
                LinkedSkill = SkillID.MINING
            };
            
            _ironLootCreation = new ResourceLootCreation
            {
                ResourceID = ResourceID.IRON_CLUSTER,
                LootTableEntries = [ new LootTableEntry { ItemID = ItemID.IRON, Weight = 1 }],
                GrantPolicyEntry = new GrantPolicyEntry { GrantWeight = 0, SkipWeight = 0 }
            };

            _locationLootCreation = new LocationLootCreation
            {
                ResourceID = ResourceID.GEM_VEIN,
                LocationID = LocationID.CAVE,
                LootTableEntries = [ new LootTableEntry { ItemID = ItemID.RUBY, Weight = 1 } ],
                GrantPolicyEntry = new GrantPolicyEntry { GrantWeight = 0, SkipWeight = 0 }
            };
        }
        
        [SetUp]
        public void Setup()
        {
            _inventoryUpdateListener = new InventoryUpdateListener();
            ManagedSubscribe(_inventoryUpdateListener);
        }
        
        private void DispatchNodeCreation(params HarvestNodeCreation[] nodeCreations)
        {
            IBuffer<HarvestNodeCreation> buffer = BufferManager.RequestBuffer<HarvestNodeCreation>(new BufferRequest(nodeCreations.Length));
            buffer.Assign(nodeCreations);
            buffer.MarkReady();
        }

        private void DispatchNodeUpdate(params HarvestNodeUpdate[] nodeUpdates)
        {
            IBuffer<HarvestNodeUpdate> buffer = BufferManager.RequestBuffer<HarvestNodeUpdate>(new BufferRequest(nodeUpdates.Length));
            buffer.Assign(nodeUpdates);
            buffer.MarkReady();
        }

        private void DispatchResourceLootCreation(params ResourceLootCreation[] lootCreations)
        {
            IBuffer<ResourceLootCreation> buffer = BufferManager.RequestBuffer<ResourceLootCreation>(new BufferRequest(lootCreations.Length));
            buffer.Assign(lootCreations);
            buffer.MarkReady();
        }

        private void DispatchLocationLootCreation(params LocationLootCreation[] lootCreations)
        {
            IBuffer<LocationLootCreation> buffer = BufferManager.RequestBuffer<LocationLootCreation>(new BufferRequest(lootCreations.Length));
            buffer.Assign(lootCreations);
            buffer.MarkReady();
        }

        private void AssertListenerCalled(bool wasCalled)
        {
            Assert.That(_inventoryUpdateListener.WasCalled, Is.EqualTo(wasCalled));
        }

        private void AssertResponseLength(int length)
        {
            Assert.That(_inventoryUpdateListener.InventoryUpdates, Has.Length.EqualTo(length));
        }

        private static void AssertInventoryUpdate(InventoryUpdate inventoryUpdate, ItemID itemID)
        {
            Assert.Multiple(() =>
            {
                Assert.That(inventoryUpdate.ItemID, Is.EqualTo(itemID));
                Assert.That(inventoryUpdate.ActionType, Is.EqualTo(ActionType.ADD));
                Assert.That(inventoryUpdate.Amount, Is.GreaterThan(0));
            });
        }
        
        [Test]
        public void Positive_SendHarvestNodeUpdate_ItemGrantPolicyNoDrop_NoUpdatesDispatched()
        {
            DispatchNodeCreation(_ironCreation);
            DispatchResourceLootCreation(_ironLootCreation with { GrantPolicyEntry = new GrantPolicyEntry { GrantWeight = 0, SkipWeight = 1 }});
            
            DispatchNodeUpdate(_ironUpdate);

            AssertListenerCalled(false);
        }
        
        [Test]
        public void Positive_SendHarvestNodeUpdate_LocationGrantPolicyNoDrop_NoUpdatesDispatched()
        {
            DispatchNodeCreation(_ironCreation);
            DispatchLocationLootCreation(_locationLootCreation with { GrantPolicyEntry = new GrantPolicyEntry { GrantWeight = 0, SkipWeight = 1 }});
            
            DispatchNodeUpdate(_ironUpdate);

            AssertListenerCalled(false);
        }

        [Test]
        public void Positive_SendHarvestNodeUpdate_NoDrops_NoUpdatesDispatched()
        {
            DispatchNodeCreation(_ironCreation);
            
            DispatchNodeUpdate(_ironUpdate);

            AssertListenerCalled(false);
        }

        [Test]
        public void Positive_SendHarvestNodeUpdate_HarvestNodeGrantDrop_DispatchesInventoryUpdate()
        {
            DispatchNodeCreation(_ironCreation);
            DispatchResourceLootCreation(_ironLootCreation);
            
            DispatchNodeUpdate(_ironUpdate);

            AssertListenerCalled(true);
            AssertResponseLength(1);
            AssertInventoryUpdate(_inventoryUpdateListener.InventoryUpdates[0], ItemID.IRON);
        }
        
        [Test]
        public void Positive_SendHarvestNodeUpdate_LocationGrantsDrop_DispatchesInventoryUpdate()
        {
            DispatchNodeCreation(_ironCreation);
            DispatchLocationLootCreation(_locationLootCreation);
            
            DispatchNodeUpdate(_ironUpdate);
        
            AssertListenerCalled(true);
            AssertResponseLength(1);
            AssertInventoryUpdate(_inventoryUpdateListener.InventoryUpdates[0], ItemID.RUBY);
        }
        
        [Test]
        public void Positive_SendHarvestNodeUpdate_LocationAndHarvestNodeDrop_DispatchesTwoUpdates()
        {
            DispatchNodeCreation(_ironCreation);
            DispatchResourceLootCreation(_ironLootCreation);
            DispatchLocationLootCreation(_locationLootCreation);
            
            DispatchNodeUpdate(_ironUpdate);
        
            AssertListenerCalled(true);
            AssertResponseLength(2);
            AssertInventoryUpdate(_inventoryUpdateListener.InventoryUpdates[0], ItemID.IRON);
            AssertInventoryUpdate(_inventoryUpdateListener.InventoryUpdates[1], ItemID.RUBY);
        }
    }
}
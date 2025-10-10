using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Information.Contracts;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Progression;

namespace IdelPog.Integration.Tests.HarvestNode.Item
{
    [TestFixture]
    public sealed class ItemGenerationTest : ManagedTestBuffer
    {
        private HarvestNodeUpdate _ironUpdate;
        private HarvestNodeCreation _ironCreation;
        private HarvestNodeLootCreation _ironLootCreation;
        private InventoryUpdateListener _inventoryUpdateListener;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _ironUpdate = new HarvestNodeUpdate
            {
                ItemID = ItemID.IRON,
                SkillID = SkillID.MINING
            };

            _ironCreation = new HarvestNodeCreation
            {
                ReadOnlyHarvestNodes =
                [
                    new ReadOnlyHarvestNode
                    {
                        ResourceID = ResourceID.IRON_CLUSTER,
                        ItemID = ItemID.IRON,
                        LocationID = LocationID.CAVE,
                        ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 },
                        Information = new Information { Name = "", Description = "" }
                    }
                ],
                LinkedSkill = SkillID.MINING
            };
            
            _ironLootCreation = new HarvestNodeLootCreation
            {
                ItemID = ItemID.IRON,
                ResourceID = ResourceID.IRON_CLUSTER,
                LootTableEntries =
                [
                    new LootTableEntry { ItemID = ItemID.IRON, Weight = 1 }
                ],
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

        private void DispatchNodeLootCreation(params HarvestNodeLootCreation[] lootCreations)
        {
            IBuffer<HarvestNodeLootCreation> buffer = BufferManager.RequestBuffer<HarvestNodeLootCreation>(new BufferRequest(lootCreations.Length));
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
        public void Positive_SendHarvestNodeUpdate_GrantPolicyNoDrop_NoUpdatesDispatched()
        {
            DispatchNodeCreation(_ironCreation);
            DispatchNodeLootCreation(_ironLootCreation with { GrantPolicyEntry = new GrantPolicyEntry { GrantWeight = 0, SkipWeight = 1 }});
            
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
        public void Positive_SendHarvestNodeUpdate_ItemGrantDrop_DispatchesInventoryUpdate()
        {
            DispatchNodeCreation(_ironCreation);
            DispatchNodeLootCreation(_ironLootCreation);
            
            DispatchNodeUpdate(_ironUpdate);

            AssertListenerCalled(true);
            AssertResponseLength(1);
            AssertInventoryUpdate(_inventoryUpdateListener.InventoryUpdates[0], _ironUpdate.ItemID);
        }
        
        [Test]
        public void Positive_SendHarvestNodeUpdate_LocationGrantsDrop_DispatchesInventoryUpdate()
        {
            DispatchNodeCreation(_ironCreation);
            
            DispatchNodeUpdate(_ironUpdate);
        
            AssertListenerCalled(true);
            AssertResponseLength(1);
            AssertInventoryUpdate(_inventoryUpdateListener.InventoryUpdates[0], ItemID.STONE);
        }
        
        [Test]
        public void Positive_SendHarvestNodeUpdate_LocationAndItemDrop_DispatchesTwoUpdates()
        {
            DispatchNodeCreation(_ironCreation);
            DispatchNodeLootCreation(_ironLootCreation);
            
            DispatchNodeUpdate(_ironUpdate);
        
            AssertListenerCalled(true);
            AssertResponseLength(2);
            AssertInventoryUpdate(_inventoryUpdateListener.InventoryUpdates[0], _ironUpdate.ItemID);
            AssertInventoryUpdate(_inventoryUpdateListener.InventoryUpdates[0], ItemID.STONE);
        }
    }
}
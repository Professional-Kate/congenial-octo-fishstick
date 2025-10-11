using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Buffer.Manager;
using IdelPog.HarvestNode.Contracts;
using IdelPog.HarvestNode.Contracts.Command;
using IdelPog.HarvestNode.Contracts.Response;

namespace IdelPog.Integration.Tests.HarvestNode.Unlock
{
    internal sealed class HarvestNodeUnlockDispatcher
    {
        private readonly IBufferManager _bufferManager;
        public readonly HarvestNodeRequirementsCreation MiningCreation;
        public readonly HarvestNodeUnlock MiningUnlock;

        public HarvestNodeUnlockDispatcher(IBufferManager bufferManager)
        {
            _bufferManager = bufferManager;

            MiningCreation = new HarvestNodeRequirementsCreation
            {
                SkillID = SkillID.MINING,
                HarvestNodeRequirements =
                [
                    new HarvestNodeRequirement
                    {
                        RequiredLevel = 1,
                        OnUnlockCommand = new HarvestNodeUnlockResponse { ResourceID = ResourceID.STONE, SkillID = SkillID.MINING }
                    },
                    new HarvestNodeRequirement
                    {
                        RequiredLevel = 2,
                        OnUnlockCommand = new HarvestNodeUnlockResponse { ResourceID = ResourceID.IRON_CLUSTER, SkillID = SkillID.MINING }
                    },
                    new HarvestNodeRequirement
                    {
                        RequiredLevel = 3,
                        OnUnlockCommand = new HarvestNodeUnlockResponse { ResourceID = ResourceID.COPPER_CLUSTER, SkillID = SkillID.MINING }
                    },
                    new HarvestNodeRequirement
                    {
                        RequiredLevel = 4,
                        OnUnlockCommand = new HarvestNodeUnlockResponse { ResourceID = ResourceID.GOLD_CLUSTER, SkillID = SkillID.MINING }
                    }
                ]
            };
            
            MiningUnlock = new HarvestNodeUnlock { SkillID = SkillID.MINING, SkillLevel = 1 };
        }

        public void DispatchCreations(params HarvestNodeRequirementsCreation[] creations)
        {
            IBuffer<HarvestNodeRequirementsCreation> buffer = _bufferManager.RequestBuffer<HarvestNodeRequirementsCreation>(new BufferRequest(creations.Length));
            buffer.Assign(creations);
            buffer.MarkReady();
        }
        
        public void DispatchUnlocks(params HarvestNodeUnlock[] unlocks)
        {
            IBuffer<HarvestNodeUnlock> buffer = _bufferManager.RequestBuffer<HarvestNodeUnlock>(new BufferRequest(unlocks.Length));
            buffer.Assign(unlocks);
            buffer.MarkReady();
        }
    }
}
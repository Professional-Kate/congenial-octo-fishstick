using IdelPog.Core.Contracts.Enum;

namespace IdelPog.HarvestNode.Exceptions
{
    public sealed class HarvestNodeLockedException : Exception
    {
        private const string MESSAGE = "{0} does not have the {1} HarvestNode unlocked!!!";

        public readonly SkillID SkillID;
        public readonly ItemID ItemID;

        public HarvestNodeLockedException(SkillID skillID, ItemID itemID) : base (string.Format(MESSAGE, skillID.ToString(), itemID.ToString()))
        {
            SkillID = skillID;
            ItemID = itemID;
        }
    }
}
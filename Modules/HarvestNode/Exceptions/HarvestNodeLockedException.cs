using IdelPog.Core.Contracts.Enum;

namespace IdelPog.HarvestNode.Exceptions
{
    public sealed class HarvestNodeLockedException : Exception
    {
        private const string MESSAGE = "{0} does not have the {1} HarvestNode unlocked!!!";

        public readonly SkillID SkillID;
        public readonly ResourceID ResourceID;

        public HarvestNodeLockedException(SkillID skillID, ResourceID resourceID) : base (string.Format(MESSAGE, skillID.ToString(), resourceID.ToString()))
        {
            SkillID = skillID;
            ResourceID = resourceID;
        }
    }
}
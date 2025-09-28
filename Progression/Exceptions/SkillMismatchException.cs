using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Progression.Exceptions
{
    public sealed class SkillMismatchException : Exception
    {
        private const string MESSAGE = "The passed SkillID {0} does not match the expected SkillID {1}!";

        public readonly SkillID PassedSkillID;
        public readonly SkillID ExpectedSkillID;

        public SkillMismatchException(SkillID passedSkillID, SkillID expectedSkillID) : base(string.Format(MESSAGE, passedSkillID, expectedSkillID))
        {
            PassedSkillID = passedSkillID;
            ExpectedSkillID = expectedSkillID;
        }
    }
}
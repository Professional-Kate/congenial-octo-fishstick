namespace IdelPog.Progression.Exceptions
{
    public sealed class IDMismatchException<TID> : Exception
    {
        private const string MESSAGE = "The passed SkillID {0} does not match the expected SkillID {1}!";

        public readonly TID PassedID;
        public readonly TID ExpectedID;

        public IDMismatchException(TID passedID, TID expectedID) : base(string.Format(MESSAGE, passedID, expectedID))
        {
            PassedID = passedID;
            ExpectedID = expectedID;
        }
    }
}
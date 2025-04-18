namespace ContentHydrator.Assertions
{
    public interface IAssertValidCast
    {
        public void AssertCastable<TExpected>(object objectToAssert);
    }
}
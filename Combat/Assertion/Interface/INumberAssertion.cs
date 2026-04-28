namespace IdelPog.Combat.Assertion.Interface
{
    public interface INumberAssertion
    {
        public void AssertNumberNotZero(uint number, string source);

        public void AssertNumberNotZero(double number, string source);
    }
}
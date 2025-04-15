using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.Validation.Assertions
{
    /// <summary>
    /// Base assertion class. This class contains a method for handling thrown exceptions using a passed <see cref="IHandler"/>
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="Exception"/> to be handled</typeparam>
    /// <seealso cref="Assert"/>
    /// <seealso cref="IHandler"/>
    public abstract class BaseAssertion<T>(IHandler handler)
        where T : Exception
    {
        /// <summary>
        /// Executes the passed action, automatically handling the thrown exception
        /// </summary>
        /// <param name="action">The assertion code. This should contain a throw statement</param>
        protected void Assert(Action action)
        {
            try
            {
                action();
            }
            catch (T exception)
            {
                handler.Handle(exception);
            }
        }
    }
}
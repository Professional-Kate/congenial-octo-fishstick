using System;
using IdelPog.Validation.Handlers.Interfaces;

namespace IdelPog.Validation
{
    /// <summary>
    /// Base assertion class. This class contains a method for handling thrown exceptions using a passed <see cref="IHandler"/>
    /// </summary>
    /// <seealso cref="Assert"/>
    /// <seealso cref="IHandler"/>
    public abstract class BaseAssertion 
    {
        private readonly IHandler _handler;

        protected BaseAssertion(IHandler handler)
        {
            _handler = handler;
        }

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
            catch (Exception exception)
            {
                _handler.Handle(exception);
            }
        }
    }
}
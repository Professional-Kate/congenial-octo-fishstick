using System;
using IdelPog.Validation.Handlers.Interfaces;

namespace IdelPog.Validation
{
    public abstract class AssertionsHandler 
    {
        private readonly IHandler _handler;

        protected AssertionsHandler(IHandler handler)
        {
            _handler = handler;
        }

        protected void Handle(Action action)
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
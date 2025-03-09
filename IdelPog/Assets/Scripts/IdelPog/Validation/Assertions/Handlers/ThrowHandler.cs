using System;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.Validation.Assertions.Handlers
{
    /// <summary>
    /// This handler will throw any passed exception
    /// </summary>
    public class ThrowHandler : IHandler
    {
        public void Handle(Exception exception)
        {
            throw exception;
        }
    }
}
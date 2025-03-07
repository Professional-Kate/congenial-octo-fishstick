using System;
using IdelPog.Validation.Handlers.Interfaces;

namespace IdelPog.Validation.Handlers
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
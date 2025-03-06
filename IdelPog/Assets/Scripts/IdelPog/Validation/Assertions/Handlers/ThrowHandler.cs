using System;
using IdelPog.Validation.Handlers.Interfaces;

namespace IdelPog.Validation.Handlers
{
    public class ThrowHandler : IHandler
    {
        public void Handle(Exception exception)
        {
            throw exception;
        }
    }
}
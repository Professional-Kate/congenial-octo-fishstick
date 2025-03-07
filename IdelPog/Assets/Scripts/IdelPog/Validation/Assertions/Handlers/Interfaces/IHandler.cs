using System;

namespace IdelPog.Validation.Handlers.Interfaces
{
    public interface IHandler
    {
        public void Handle(Exception exception);
    }
}
using System;

namespace IdelPog.Validation.Handlers.Interfaces
{
    public interface IHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        public void Handle(Exception exception);
    }
}
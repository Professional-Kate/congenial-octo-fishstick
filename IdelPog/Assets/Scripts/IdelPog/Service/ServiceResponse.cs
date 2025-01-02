using System;

namespace IdelPog.Service
{
    /// <summary>
    /// Contains two members <see cref="Message"/> and <see cref="IsSuccess"/> that will tell you if the called operation was successful.
    /// </summary>
    /// <seealso cref="Success"/>
    /// <seealso cref="Failure"/>
    public struct ServiceResponse
    {
        /// <summary>
        /// If <see cref="IsSuccess"/> is false then this will contain the reason why the operation failed. Otherwise, it will be null
        /// </summary>
        public readonly string Message;
        
        /// <summary>
        /// Dictates if the operation that returned this object completed all its processing. If false, then <see cref="Message"/> will contain the reason why
        /// </summary>
        public readonly bool IsSuccess;

        private ServiceResponse(bool success, string message = null)
        {
            IsSuccess = success;
            Message = message;
        }

        /// <summary>
        /// Creates a new successful <see cref="ServiceResponse"/> object
        /// </summary>
        /// <returns>A successful <see cref="ServiceResponse"/> object. <see cref="IsSuccess"/> will be true.</returns>
        public static ServiceResponse Success() => new(true);
        
        /// <summary>
        /// Creates a new unsuccessful <see cref="ServiceResponse"/> object, who's <see cref="Message"/> will be populated
        /// </summary>
        /// <param name="message">Why the current operation failed. This is expected to be an <see cref="Exception"/>'s <see cref="Exception.Message"/> where applicable</param>
        /// <returns>An unsuccessful <see cref="ServiceResponse"/> object. <see cref="IsSuccess"/> will be false, and <see cref="Message"/> will contain the reason why</returns>
        public static ServiceResponse Failure(string message) => new(false, message);
    }
}
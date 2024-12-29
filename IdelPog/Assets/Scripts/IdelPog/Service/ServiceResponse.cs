namespace IdelPog.Service
{
    public struct ServiceResponse
    {
        public readonly string Message;
        public readonly bool IsSuccess;

        private ServiceResponse(bool success, string message = null)
        {
            IsSuccess = success;
            Message = message;
        }

        public static ServiceResponse Success() => new(true);
        
        public static ServiceResponse Failure(string message) => new(false, message);
    }
}
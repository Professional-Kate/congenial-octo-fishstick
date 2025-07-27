namespace IdelPog.Messaging.Exceptions
{
    public class ControllerThrownException : Exception
    {
        private const string MESSAGE = "Something blew up! Controller: {0}. Exception: {1}";
        
        public readonly string ControllerName;
        public readonly Exception Exception;

        public ControllerThrownException(string controllerName, Exception exception) : base(string.Format(MESSAGE, controllerName,  exception.Message))
        {
            ControllerName = controllerName;
            Exception = exception;
        }
    }
}
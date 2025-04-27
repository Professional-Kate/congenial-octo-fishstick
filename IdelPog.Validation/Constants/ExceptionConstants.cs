namespace IdelPog.Validation.Constants
{
    public static class ExceptionConstants
    {
        public const string NOT_FOUND_MESSAGE = "Error! The passed ID {0} was not found!";
        public const string MAX_LEVEL_MESSAGE = "Error! The passed Job {0} is at max level! ";
        public const string NEGATIVE_NUMBER_MESSAGE = "Error! The passed number {0} is negative!";
        public const string DUPLICATE_ITEM_MESSAGE = "Error! The passed Item {0} already exists!";
        public const string EMPTY_DIRECTORY_MESSAGE = "Error! The passed Directory {0} is empty!";
        public const string BUFFER_SIZE_MISSMATCH_MESSAGE = "Error! The passed collection is not the correct size! Expected {0}, got {1}!";
        public const string BUFFER_SIZE_INVALID_MESSAGE = "Error! The passed collection size is not valid! {0} is not valid!";
    }
}
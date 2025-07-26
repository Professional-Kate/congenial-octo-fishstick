namespace IdelPog.Validation.Constants
{
    public static class ExceptionConstants
    {
        public const string MAX_LEVEL_MESSAGE = "Error! The passed Skill {0} is at max level! ";
        public const string DUPLICATE_ITEM_MESSAGE = "Error! The passed Item {0} already exists!";
        public const string EMPTY_DIRECTORY_MESSAGE = "Error! The passed Directory {0} is empty!";
        public const string BUFFER_SIZE_MISMATCH_MESSAGE = "Error! The passed collection is not the correct size! Expected {0}, got {1}!";
        public const string BUFFER_SIZE_INVALID_MESSAGE = "Error! The passed collection size is not valid! {0} is not valid!";
        public const string BUFFER_STATE_INVALID_MESSAGE = "Error! The passed BufferState {0} is not valid! Expected {1}";
        public const string NO_LISTENER_FOUND_MESSAGE = "Error! The passed Listener {0} for Type {1} was not found!";
    }
}
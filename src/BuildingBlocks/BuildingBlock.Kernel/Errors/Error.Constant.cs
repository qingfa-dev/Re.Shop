namespace BuildingBlock.Kernel.Errors;

public static class ErrorConstant
{
    #region Constraints

    public static class Constraint
    {
        public static class Code
        {
            public const int MaxLength = 256;
        }

        public static class Message
        {
            public const int MaxLength = 1024;
        }

        public static class Type
        {
            public const int MaxLength = 256;
        }

        public static class Status
        {
            public const int Min = 100;
            public const int Max = 599;
        }
    }

    #endregion

    #region Metadata

    public static class Metadata
    {
        public const string TraceId = nameof(TraceId);
        public const string Timestamp = nameof(Timestamp);
        public const string Resource = nameof(Resource);
        public const string Field = nameof(Field);
        public const string Attempt = nameof(Attempt);
    }

    #endregion

    #region Result

    public static class Result
    {
        #region Failure

        public static class Failure
        {
            #region Status

            public static class Status
            {
                public static class OutOfRange
                {
                    public const string Code =
                        "error.status.out_of_range";

                    public const string Message =
                        "The status code is out of the valid range ({0} - {1}).";
                }
            }

            #endregion

            #region Code

            public static class Code
            {
                public static class NullOrWhitespace
                {
                    public const string Code =
                        "error.code.null_or_whitespace";

                    public const string Message =
                        "The error code cannot be null or whitespace.";
                }

                public static class ExceedsMaxLength
                {
                    public const string Code =
                        "error.code.exceeds_max_length";

                    public const string Message =
                        "The error code exceeds the maximum length of '{0}' characters.";
                }
            }

            #endregion

            #region Message

            public static class Message
            {
                public static class NullOrWhitespace
                {
                    public const string Code =
                        "error.message.null_or_whitespace";

                    public const string Message =
                        "The error message cannot be null or whitespace.";
                }

                public static class ExceedsMaxLength
                {
                    public const string Code =
                        "error.message.exceeds_max_length";

                    public const string Message =
                        "The error message exceeds the maximum length of '{0}' characters.";
                }
            }

            #endregion

            #region Type

            public static class Type
            {
                public static class ExceedsMaxLength
                {
                    public const string Code =
                        "error.type.exceeds_max_length";

                    public const string Message =
                        "The error type exceeds the maximum length of '{0}' characters.";
                }
            }

            #endregion
        }

        #endregion
    }

    #endregion
}

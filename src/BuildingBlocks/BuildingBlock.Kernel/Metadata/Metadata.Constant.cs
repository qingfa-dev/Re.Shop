namespace BuildingBlock.Kernel.Metadata;

public static class MetadataConstant
{
    // ---------------------------------------------------------------
    // Well-known metadata keys
    // ---------------------------------------------------------------
    public static class Keyword
    {
        public const string Timestamp = nameof(Timestamp);
        public const string MemberType = nameof(MemberType);
        public const string CorrelationId = nameof(CorrelationId);
        public const string RequestId = nameof(RequestId);
        public const string CausationId = nameof(CausationId);
        public const string EventId = nameof(EventId);
        public const string AggregateVersion = nameof(AggregateVersion);
    }

    // ---------------------------------------------------------------
    // Failure catalogue
    //   Pattern: Result.Failure.<subject>.<field>.<rule>
    //   Every rule exposes:
    //     Code    -> "<subject>.<field>.<rule>"
    //     Message -> human-readable description
    //     Pattern -> "code:message" (compile-time constant)
    // ---------------------------------------------------------------
    public static class Result
    {
        public static class Failure
        {
            // ----- Subjects -----

            public static class Request
            {
                public static class Argument
                {
                    public static class Null
                    {
                        public const string Code = "request.argument.null";
                        public const string Message = "The request argument must not be null.";
                        public const string Pattern = Code + ":" + Message;
                    }
                }
            }

            public static class Key
            {
                public static class Argument
                {
                    public static class Null
                    {
                        public const string Code = "key.argument.null";
                        public const string Message = "The key argument must not be null.";
                        public const string Pattern = Code + ":" + Message;
                    }
                }

                public static class Metadata
                {
                    public static class NotFound
                    {
                        public const string Code = "key.metadata.not_found";
                        public const string Message = "The specified metadata key was not found.";
                        public const string Pattern = Code + ":" + Message;
                    }
                }
            }

            public static class Value
            {
                public static class Metadata
                {
                    public static class InvalidCast
                    {
                        public const string Code = "value.metadata.invalid_cast";
                        public const string Message = "The metadata value could not be cast to the requested type.";
                        public const string Pattern = Code + ":" + Message;
                    }
                }
            }

            public static class Dictionary
            {
                public static class Argument
                {
                    public static class Null
                    {
                        public const string Code = "dictionary.argument.null";
                        public const string Message = "The source dictionary must not be null.";
                        public const string Pattern = Code + ":" + Message;
                    }
                }

                public static class Mutation
                {
                    public static class NotSupported
                    {
                        public const string Code = "dictionary.mutation.not_supported";
                        public const string Message = "The underlying metadata dictionary does not support mutation.";
                        public const string Pattern = Code + ":" + Message;
                    }
                }
            }
        }
    }
}
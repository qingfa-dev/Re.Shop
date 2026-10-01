namespace SharedKernel.Structures.Meta;

public static class MetadataConstant
{
    public static class MetadataKey
    {
        public const string Code = "code";
        public const string Message = "message";
        public const string Status = "status";
        public const string Type = "type";
        public const string Target = "target";
        public const string Timestamp = nameof(Timestamp);
        public const string MemberType = nameof(MemberType);
        public const string CorrelationId = nameof(CorrelationId);
        public const string RequestId = nameof(RequestId);
        public const string CausationId = nameof(CausationId);
        public const string EventId = nameof(EventId);
        public const string AggregateVersion = nameof(AggregateVersion);
    }

    public static class Result
    {
        public static class Failure
        {
            public static class Key
            {
                public static class NullOrWhitespace
                {
                    public const string Code = "metadata.key.null_or_whitespace";
                    public const string Message = "The metadata key cannot be null or whitespace.";
                }
            }

            public static class Value
            {
                public static class Null
                {
                    public const string Code = "metadata.value.null";
                    public const string Message = "The metadata value cannot be null.";
                }
            }
        }
    }
}

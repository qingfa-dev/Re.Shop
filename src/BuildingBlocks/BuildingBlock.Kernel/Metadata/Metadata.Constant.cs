namespace BuildingBlock.Kernel.Metadata;

/// <summary>Well-known metadata keys and failure rule constants.</summary>
/// <remarks>
/// <para><see cref="Keyword"/> provides stable string constants for common metadata keys.</para>
/// <para><see cref="Result"/> provides a hierarchical catalogue of failure rules, each exposing
/// <c>Code</c>, <c>Message</c>, and <c>Pattern</c> ("code:message").</para>
/// </remarks>
public static class MetadataConstant
{
    #region Keyword

    /// <summary>Well-known metadata key names used across the kernel.</summary>
    /// <remarks>
    /// Each constant mirrors its member name via <c>nameof()</c>, guaranteeing compile-time stability.
    /// These keys are the canonical wire names for correlation, request, and aggregate tracing.
    /// </remarks>
    public static class Keyword
    {
        /// <summary>Metadata key for the event timestamp.</summary>
        public const string Timestamp = nameof(Timestamp);
        /// <summary>Metadata key for the member type identifier.</summary>
        public const string MemberType = nameof(MemberType);
        /// <summary>Metadata key for the correlation ID.</summary>
        public const string CorrelationId = nameof(CorrelationId);
        /// <summary>Metadata key for the request ID.</summary>
        public const string RequestId = nameof(RequestId);
        /// <summary>Metadata key for the causation ID.</summary>
        public const string CausationId = nameof(CausationId);
        /// <summary>Metadata key for the event ID.</summary>
        public const string EventId = nameof(EventId);
        /// <summary>Metadata key for the aggregate version.</summary>
        public const string AggregateVersion = nameof(AggregateVersion);
    }

    #endregion

    #region Failure catalogue

    /// <summary>
    /// Pattern: Result.Failure.&lt;subject&gt;.&lt;field&gt;.&lt;rule&gt;
    /// Every rule exposes:
    ///   Code    -> "&lt;subject&gt;.&lt;field&gt;.&lt;rule&gt;"
    ///   Message -> human-readable description
    ///   Pattern -> "code:message" (compile-time constant)
    /// </summary>
    /// <remarks>
    /// Traverse to the relevant subject (&lt;subject&gt;), then field, then rule to obtain
    /// the <c>Code</c>, <c>Message</c>, and <c>Pattern</c> constants.
    /// </remarks>
    public static class Result
    {
        /// <summary>Failure rules grouped by subject and field.</summary>
        public static class Failure
        {
            #region Request

            /// <summary>Argument validation failures for request.</summary>
            /// <remarks>These rules fire when a request-level argument violates a precondition.</remarks>
            public static class Request
            {
                /// <summary>Argument validation failures.</summary>
                /// <remarks>Applies when the argument itself is <c>null</c>.</remarks>
                public static class Argument
                {
                    /// <summary><c>request.argument.null</c> — request argument must not be null.</summary>
                    /// <remarks>Use the <c>Pattern</c> constant in exceptions to keep code and messages in sync.</remarks>
                    public static class Null
                    {
                        /// <summary>The failure code: <c>"request.argument.null"</c>.</summary>
                        public const string Code = "request.argument.null";
                        /// <summary>The human-readable message.</summary>
                        public const string Message = "The request argument must not be null.";
                        /// <summary>The combined <c>"code:message"</c> pattern.</summary>
                        public const string Pattern = Code + ":" + Message;
                    }
                }
            }

            #endregion

            #region Key

            /// <summary>Key-related failures.</summary>
            /// <remarks>Covers null-key and lookup-failure scenarios for metadata keys.</remarks>
            public static class Key
            {
                /// <summary>Argument validation failures for key.</summary>
                public static class Argument
                {
                    /// <summary><c>key.argument.null</c> — key argument must not be null.</summary>
                    public static class Null
                    {
                        /// <summary>The failure code: <c>"key.argument.null"</c>.</summary>
                        public const string Code = "key.argument.null";
                        /// <summary>The human-readable message.</summary>
                        public const string Message = "The key argument must not be null.";
                        /// <summary>The combined <c>"code:message"</c> pattern.</summary>
                        public const string Pattern = Code + ":" + Message;
                    }
                }

                /// <summary>Key lookup failures.</summary>
                public static class Metadata
                {
                    /// <summary><c>key.metadata.not_found</c> — specified metadata key was not found.</summary>
                    public static class NotFound
                    {
                        /// <summary>The failure code: <c>"key.metadata.not_found"</c>.</summary>
                        public const string Code = "key.metadata.not_found";
                        /// <summary>The human-readable message.</summary>
                        public const string Message = "The specified metadata key was not found.";
                        /// <summary>The combined <c>"code:message"</c> pattern.</summary>
                        public const string Pattern = Code + ":" + Message;
                    }
                }
            }

            #endregion

            #region Value

            /// <summary>Value conversion failures.</summary>
            public static class Value
            {
                /// <summary>Metadata value cast failures.</summary>
                public static class Metadata
                {
                    /// <summary><c>value.metadata.invalid_cast</c> — metadata value could not be cast.</summary>
                    public static class InvalidCast
                    {
                        /// <summary>The failure code: <c>"value.metadata.invalid_cast"</c>.</summary>
                        public const string Code = "value.metadata.invalid_cast";
                        /// <summary>The human-readable message.</summary>
                        public const string Message = "The metadata value could not be cast to the requested type.";
                        /// <summary>The combined <c>"code:message"</c> pattern.</summary>
                        public const string Pattern = Code + ":" + Message;
                    }
                }
            }

            #endregion

            #region Dictionary

            /// <summary>Dictionary operation failures.</summary>
            public static class Dictionary
            {
                /// <summary>Dictionary argument failures.</summary>
                public static class Argument
                {
                    /// <summary><c>dictionary.argument.null</c> — source dictionary must not be null.</summary>
                    public static class Null
                    {
                        /// <summary>The failure code: <c>"dictionary.argument.null"</c>.</summary>
                        public const string Code = "dictionary.argument.null";
                        /// <summary>The human-readable message.</summary>
                        public const string Message = "The source dictionary must not be null.";
                        /// <summary>The combined <c>"code:message"</c> pattern.</summary>
                        public const string Pattern = Code + ":" + Message;
                    }
                }

                /// <summary>Dictionary mutation failures.</summary>
                public static class Mutation
                {
                    /// <summary><c>dictionary.mutation.not_supported</c> — dictionary does not support mutation.</summary>
                    public static class NotSupported
                    {
                        /// <summary>The failure code: <c>"dictionary.mutation.not_supported"</c>.</summary>
                        public const string Code = "dictionary.mutation.not_supported";
                        /// <summary>The human-readable message.</summary>
                        public const string Message = "The underlying metadata dictionary does not support mutation.";
                        /// <summary>The combined <c>"code:message"</c> pattern.</summary>
                        public const string Pattern = Code + ":" + Message;
                    }
                }
            }

            #endregion
        }
    }

    #endregion
}
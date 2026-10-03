namespace BuildingBlock.Kernel.Errors;

/// <summary>Constants for error codes, messages, patterns, constraints, metadata keys, and status codes.</summary>
public static class ErrorConstant
{
    #region Constraints

    /// <summary>Length and range constraints for error members.</summary>
    public static class Constraint
    {
        /// <summary>Code length constraints.</summary>
        public static class Code
        {
            /// <summary>Maximum allowed length for an error code.</summary>
            public const int MaxLength = 256;
        }

        /// <summary>Message length constraints.</summary>
        public static class Message
        {
            /// <summary>Maximum allowed length for an error message.</summary>
            public const int MaxLength = 1024;
        }

        /// <summary>Type length constraints.</summary>
        public static class Type
        {
            /// <summary>Maximum allowed length for an error type.</summary>
            public const int MaxLength = 256;
        }

        /// <summary>Status code range constraints.</summary>
        public static class Status
        {
            /// <summary>Minimum valid HTTP status code.</summary>
            public const int Min = 100;
            /// <summary>Maximum valid HTTP status code.</summary>
            public const int Max = 599;
        }
    }

    #endregion

    #region Metadata

    /// <summary>Well-known metadata keys attached to errors.</summary>
    public static class Metadata
    {
        /// <summary>Metadata key for the trace identifier.</summary>
        public const string TraceId = nameof(TraceId);
        /// <summary>Metadata key for the error timestamp.</summary>
        public const string Timestamp = nameof(Timestamp);
        /// <summary>Metadata key for the affected resource.</summary>
        public const string Resource = nameof(Resource);
        /// <summary>Metadata key for the field that caused the error.</summary>
        public const string Field = nameof(Field);
        /// <summary>Metadata key for the retry attempt count.</summary>
        public const string Attempt = nameof(Attempt);
    }

    #endregion

    #region Status Codes

    /// <summary>HTTP status code constants.</summary>
    public static class StatusCode
    {
        #region Success

        // ── 2xx ─────────────────────────────────────────────────────────
        /// <summary>HTTP 200 OK.</summary>
        public const int Ok = 200;
        /// <summary>HTTP 201 Created.</summary>
        public const int Created = 201;

        #endregion

        #region Failure

        // ── 4xx ─────────────────────────────────────────────────────────
        /// <summary>HTTP 400 BadRequest.</summary>
        public const int BadRequest = 400;
        /// <summary>HTTP 401 Unauthorized.</summary>
        public const int Unauthorized = 401;
        /// <summary>HTTP 403 Forbidden.</summary>
        public const int Forbidden = 403;
        /// <summary>HTTP 404 NotFound.</summary>
        public const int NotFound = 404;
        /// <summary>HTTP 409 Conflict.</summary>
        public const int Conflict = 409;
        /// <summary>HTTP 422 UnprocessableEntity.</summary>
        public const int UnprocessableEntity = 422;

        // ── 5xx ─────────────────────────────────────────────────────────
        /// <summary>HTTP 500 InternalServerError.</summary>
        public const int InternalServerError = 500;
        /// <summary>HTTP 501 NotImplemented.</summary>
        public const int NotImplemented = 501;
        /// <summary>HTTP 502 BadGateway.</summary>
        public const int BadGateway = 502;
        /// <summary>HTTP 503 ServiceUnavailable.</summary>
        public const int ServiceUnavailable = 503;
        /// <summary>HTTP 504 GatewayTimeout.</summary>
        public const int GatewayTimeout = 504;

        #endregion
    }
    #endregion

    #region Result

    /// <summary>Failure rule constants for error validation.</summary>
    public static class Result
    {
        #region Failure

        /// <summary>Failure rules grouped by subject and field.</summary>
        public static class Failure
        {
            #region Status

            /// <summary>Status code out-of-range failure rule.</summary>
            public static class Status
            {
                /// <summary>Status code out-of-range failure rule.</summary>
                public static class OutOfRange
                {
                    /// <summary>The failure code: <c>"error.status.out_of_range"</c>.</summary>
                    public const string Code = "error.status.out_of_range";
                    /// <summary>The human-readable message.</summary>
                    public const string Message = "The status code is out of the valid range ({0} - {1}).";
                }
            }

            #endregion

            #region Code

            /// <summary>Error code validation failure rules.</summary>
            public static class Code
            {
                /// <summary>Null or whitespace code failure rule.</summary>
                public static class NullOrWhitespace
                {
                    /// <summary>The failure code: <c>"error.code.null_or_whitespace"</c>.</summary>
                    public const string Code = "error.code.null_or_whitespace";
                    /// <summary>The human-readable message.</summary>
                    public const string Message = "The error code cannot be null or whitespace.";
                }

                /// <summary>Code exceeds max length failure rule.</summary>
                public static class ExceedsMaxLength
                {
                    /// <summary>The failure code: <c>"error.code.exceeds_max_length"</c>.</summary>
                    public const string Code = "error.code.exceeds_max_length";
                    /// <summary>The human-readable message.</summary>
                    public const string Message = "The error code exceeds the maximum length of '{0}' characters.";
                }
            }

            #endregion

            #region Message

            /// <summary>Error message validation failure rules.</summary>
            public static class Message
            {
                /// <summary>Null or whitespace message failure rule.</summary>
                public static class NullOrWhitespace
                {
                    /// <summary>The failure code: <c>"error.message.null_or_whitespace"</c>.</summary>
                    public const string Code = "error.message.null_or_whitespace";
                    /// <summary>The human-readable message.</summary>
                    public const string Message = "The error message cannot be null or whitespace.";
                }

                /// <summary>Message exceeds max length failure rule.</summary>
                public static class ExceedsMaxLength
                {
                    /// <summary>The failure code: <c>"error.message.exceeds_max_length"</c>.</summary>
                    public const string Code = "error.message.exceeds_max_length";
                    /// <summary>The human-readable message.</summary>
                    public const string Message = "The error message exceeds the maximum length of '{0}' characters.";
                }
            }

            #endregion

            #region Type

            /// <summary>Error type validation failure rules.</summary>
            public static class Type
            {
                /// <summary>Type exceeds max length failure rule.</summary>
                public static class ExceedsMaxLength
                {
                    /// <summary>The failure code: <c>"error.type.exceeds_max_length"</c>.</summary>
                    public const string Code = "error.type.exceeds_max_length";
                    /// <summary>The human-readable message.</summary>
                    public const string Message = "The error type exceeds the maximum length of '{0}' characters.";
                }
            }

            #endregion
        }

        #endregion
    }

    #endregion
}
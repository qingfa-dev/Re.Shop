using BuildingBlock.Kernel.Errors;

namespace BuildingBlock.Kernel.Results;

/// <summary>Defines result constraints, defaults, status codes, and failure constant groups.</summary>
public static class ResultConstant
{
    #region Constraints

    /// <summary>Defines numeric bounds and collection-size limits used by result validation.</summary>
    public static class Constraint
    {
        /// <summary>Error-count limits.</summary>
        public static class Errors
        {
            /// <summary>Maximum number of errors allowed in a result.</summary>
            public const int MaxCount = 50;
        }

        /// <summary>Metadata limits.</summary>
        public static class Metadata
        {
            /// <summary>Maximum number of metadata entries allowed.</summary>
            public const int MaxEntries = 50;
        }

        /// <summary>HTTP status-code bounds for result status validation.</summary>
        public static class Status
        {
            /// <summary>Minimum valid HTTP status code.</summary>
            public const int Min = 100;
            /// <summary>Maximum valid HTTP status code.</summary>
            public const int Max = 599;

            /// <summary>Success (2xx) status-code bounds.</summary>
            public static class Success
            {
                /// <summary>Minimum success status code.</summary>
                public const int Min = 200;
                /// <summary>Maximum success status code.</summary>
                public const int Max = 299;
            }

            /// <summary>Failure (4xx/5xx) status-code bounds.</summary>
            public static class Failure
            {
                /// <summary>Minimum failure status code.</summary>
                public const int Min = 400;
                /// <summary>Maximum failure status code.</summary>
                public const int Max = 599;
            }
        }
    }

    #endregion

    #region Defaults

    /// <summary>Default values used when no explicit value is supplied.</summary>
    public static class Default
    {
        /// <summary>Default status code for unqualified results (500).</summary>
        public const int Status = 500;
        /// <summary>Shared empty error list reused by successful results.</summary>
        public static readonly List<Error> EmptyErrors = new List<Error>();
    }

    #endregion

    #region Status Codes

    /// <summary>HTTP status-code constants for result factory methods.</summary>
    public static class StatusCode
    {
        #region Success

        // ── 2xx ─────────────────────────────────────────────────────────
        /// <summary>200 OK.</summary>
        public const int Ok = 200;
        /// <summary>201 Created.</summary>
        public const int Created = 201;
        /// <summary>202 Accepted.</summary>
        public const int Accepted = 202;
        /// <summary>204 No Content.</summary>
        public const int NoContent = 204;

        // ── 4xx/5xx (forwarded from Error constants) ──────────────────
        /// <summary>400 Bad Request.</summary>
        public const int BadRequest = ErrorConstant.StatusCode.BadRequest;
        /// <summary>401 Unauthorized.</summary>
        public const int Unauthorized = ErrorConstant.StatusCode.Unauthorized;
        /// <summary>403 Forbidden.</summary>
        public const int Forbidden = ErrorConstant.StatusCode.Forbidden;
        /// <summary>404 Not Found.</summary>
        public const int NotFound = ErrorConstant.StatusCode.NotFound;
        /// <summary>409 Conflict.</summary>
        public const int Conflict = ErrorConstant.StatusCode.Conflict;
        /// <summary>422 Unprocessable Entity.</summary>
        public const int UnprocessableEntity = ErrorConstant.StatusCode.UnprocessableEntity;
        /// <summary>500 Internal Server Error.</summary>
        public const int InternalServerError = ErrorConstant.StatusCode.InternalServerError;
        /// <summary>503 Service Unavailable.</summary>
        public const int ServiceUnavailable = ErrorConstant.StatusCode.ServiceUnavailable;

        #endregion
    }

    #endregion

    #region Result Failures

    /// <summary>Failure constant groups that describe result-level error conditions.</summary>
    public static class Failure
    {
        #region Result

    /// <summary>Result-level failure constant groups.</summary>
    public static class Result
    {
        /// <summary>Failure when a result contains errors with mixed status codes.</summary>
        public static class MixedStatusCodes
        {
            /// <summary>Stable error code.</summary>
            public const string Code =
                "result.errors.mixed_status_codes";

            /// <summary>Human-readable error message.</summary>
            public const string Message =
                "All errors in a result must have the same HTTP status code.";
        }

        /// <summary>Failure when a failed result contains no errors.</summary>
        public static class FailureWithoutErrors
        {
            /// <summary>Stable error code.</summary>
            public const string Code =
                "result.failure.without_errors";

            /// <summary>Human-readable error message.</summary>
            public const string Message =
                "A failed result must contain at least one error.";
        }
    }

        #endregion

        #region Errors

        /// <summary>Error-collection failure constant groups.</summary>
    public static class Errors
    {
        /// <summary>Failure when an error collection is empty.</summary>
        public static class Empty
        {
            /// <summary>Stable error code.</summary>
            public const string Code =
                "result.errors.empty";

            /// <summary>Human-readable error message.</summary>
            public const string Message =
                "At least one error is required.";
        }

        /// <summary>Failure when an error collection exceeds the maximum count.</summary>
        public static class ExceedsMaxCount
        {
            /// <summary>Stable error code.</summary>
            public const string Code =
                "result.errors.exceeds_max_count";

            /// <summary>Human-readable error message.</summary>
            public const string Message =
                "A result cannot contain more than {0} errors.";
        }
    }

        #endregion

        #region Metadata

        /// <summary>Metadata failure constant groups.</summary>
    public static class Metadata
    {
        /// <summary>Failure when a metadata key is null, empty, or whitespace.</summary>
        public static class EmptyKey
        {
            /// <summary>Stable error code.</summary>
            public const string Code =
                "result.metadata.empty_key";

            /// <summary>Human-readable error message.</summary>
            public const string Message =
                "Metadata keys cannot be null, empty, or whitespace.";
        }

        /// <summary>Failure when a metadata value is null.</summary>
        public static class NullValue
        {
            /// <summary>Stable error code.</summary>
            public const string Code =
                "result.metadata.null_value";

            /// <summary>Human-readable error message.</summary>
            public const string Message =
                "Metadata value for key '{0}' cannot be null.";
        }

        /// <summary>Failure when metadata exceeds the maximum entry count.</summary>
        public static class ExceedsMaxEntries
        {
            /// <summary>Stable error code.</summary>
            public const string Code =
                "result.metadata.exceeds_max_entries";

            /// <summary>Human-readable error message.</summary>
            public const string Message =
                "Metadata cannot contain more than {0} entries.";
        }
    }

        #endregion

        #region Value

        /// <summary>Value access failure constant groups.</summary>
    public static class Value
    {
        /// <summary>Failure when a successful result has no value.</summary>
        public static class Required
        {
            /// <summary>Stable error code.</summary>
            public const string Code =
                "result.value.required";

            /// <summary>Human-readable error message.</summary>
            public const string Message =
                "A successful result must have a value.";
        }

        /// <summary>Failure when a failed result has a value.</summary>
        public static class NotAllowed
        {
            /// <summary>Stable error code.</summary>
            public const string Code =
                "result.value.not_allowed";

            /// <summary>Human-readable error message.</summary>
            public const string Message =
                "A failed result cannot have a value.";
        }
    }

        #endregion

        #region Status

        /// <summary>Status-code validation failure constant groups.</summary>
    public static class Status
    {
        /// <summary>Failure when a status code is outside the valid range.</summary>
        public static class InvalidStatusCode
        {
            /// <summary>Stable error code.</summary>
            public const string Code =
                "result.status.invalid_status_code";

            /// <summary>Human-readable error message.</summary>
            public const string Message =
                "Status code must be between {0} and {1}, but was {2}.";
        }

        /// <summary>Failure when a success result has an invalid status code.</summary>
        public static class InvalidSuccessStatus
        {
            /// <summary>Stable error code.</summary>
            public const string Code =
                "result.status.invalid_success_status";

            /// <summary>Human-readable error message.</summary>
            public const string Message =
                "A successful result must have a status code between {0} and {1}, but was {2}.";
        }

        /// <summary>Failure when a failure result has an invalid status code.</summary>
        public static class InvalidFailureStatus
        {
            /// <summary>Stable error code.</summary>
            public const string Code =
                "result.status.invalid_failure_status";

            /// <summary>Human-readable error message.</summary>
            public const string Message =
                "A failed result must have a status code between {0} and {1}, but was {2}.";
        }
    }

        #endregion

        #region Flow

        /// <summary>Flow-operation failure constant groups.</summary>
    public static class Flow
    {
        /// <summary>Failure when a required callback is missing.</summary>
        public static class CallbackMissing
        {
            /// <summary>Stable error code.</summary>
            public const string Code = "result.flow.callback_missing";
            /// <summary>Human-readable error message.</summary>
            public const string Message = "The required callback for '{0}' was not provided.";
        }

        /// <summary>Failure when an error callback returns null.</summary>
        public static class ErrorMissing
        {
            /// <summary>Stable error code.</summary>
            public const string Code = "result.flow.error_missing";
            /// <summary>Human-readable error message.</summary>
            public const string Message = "The error callback for '{0}' returned null.";
        }

        /// <summary>Failure when a result callback returns null.</summary>
        public static class ResultMissing
        {
            /// <summary>Stable error code.</summary>
            public const string Code = "result.flow.result_missing";
            /// <summary>Human-readable error message.</summary>
            public const string Message = "The result callback for '{0}' returned null.";
        }
    }

        #endregion
    }

    #endregion
}

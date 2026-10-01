namespace SharedKernel.Errors;

public static class ErrorConstant
{
    // Constraints:
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

    // Defaults:
    public static class Default
    {

        public const string Code = "general.error";
        public const string Message = "An error occurred.";
        public const int Status = 500;
        public static string Type(int status = Status) => $"https://httpstatuses.io/{status}";
    }

    // Results:
    public static class Result
    {
        // Success:
        // Failure:
        public static class Failure
        {
            // Validation:
            // Status:
            public static class Status
            {
                public static class OutOfRange
                {
                    public const string Code = "error.status.out_of_range";
                    public const string Message = "The status code is out of the valid range ({0} - {1}).";
                }
            }

            // Code:
            public static class Code
            {
                public static class NullOrWhitespace
                {
                    public const string Code = "error.code.null_or_whitespace";
                    public const string Message = "The error code cannot be null or whitespace.";
                }

                public static class ExceedsMaxLength
                {
                    public const string Code = "error.code.exceeds_max_length";
                    public const string Message = "The error code exceeds the maximum length of '{0}' characters.";
                }
            }

            // Message:
            public static class Message
            {
                public static class NullOrWhitespace
                {
                    public const string Code = "error.message.null_or_whitespace";
                    public const string Message = "The error message cannot be null or whitespace.";
                }

                public static class ExceedsMaxLength
                {
                    public const string Code = "error.message.exceeds_max_length";
                    public const string Message = "The error message exceeds the maximum length of '{0}' characters.";
                }

            }

            // Type:
            public static class Type
            {
                public static class ExceedsMaxLength
                {
                    public const string Code = "error.type.exceeds_max_length";
                    public const string Message = "The error type exceeds the maximum length of '{0}' characters.";
                }
            }

        }
    }

    #region ErrorTypes
    public static class ErrorTypes
    {

        #region Server Errors (5xx)

        public static class ServerError
        {
            public const int Status = 500;
            public const string Type =
                "https://problems-registry.smartbear.com/server-error";
            public const string Code = Default.Code;
            public const string Message = Default.Message;
        }

        public static class Failure
        {

            public const int Status = 500;

            public const string Type =
                "https://problems-registry.smartbear.com/failure";

            public const string Code = "General.Failure";

            public const string Message = "The operation failed.";
        }

        public static class ServiceUnavailable
        {

            public const int Status = 503;

            public const string Type =
                "https://problems-registry.smartbear.com/service-unavailable";

            public const string Code = "General.ServiceUnavailable";

            public const string Message =
                "The service is temporarily unavailable.";
        }

        public static class LicenseExpired
        {

            public const int Status = 503;

            public const string Type =
                "https://problems-registry.smartbear.com/license-expired";

            public const string Code = "License.Expired";

            public const string Message = "The license has expired.";
        }

        public static class LicenseCancelled
        {

            public const int Status = 503;

            public const string Type =
                "https://problems-registry.smartbear.com/license-cancelled";

            public const string Code = "License.Cancelled";

            public const string Message =
                "The license has been cancelled.";
        }

        #endregion

        #region ErrorTypes — Client Errors (4xx)

        public static class BadRequest
        {

            public const int Status = 400;

            public const string Type =
                "https://problems-registry.smartbear.com/bad-request";

            public const string Code = "Request.BadRequest";

            public const string Message = "The request is invalid.";
        }

        public static class Unauthorized
        {

            public const int Status = 401;

            public const string Type =
                "https://problems-registry.smartbear.com/unauthorized";

            public const string Code = "Request.Unauthorized";

            public const string Message =
                "The request lacks valid authentication credentials.";
        }

        public static class Forbidden
        {

            public const int Status = 403;

            public const string Type =
                "https://problems-registry.smartbear.com/forbidden";

            public const string Code = "Request.Forbidden";

            public const string Message =
                "Access to the requested resource is forbidden.";
        }

        public static class NotFound
        {

            public const int Status = 404;

            public const string Type =
                "https://problems-registry.smartbear.com/not-found";

            public const string Code = "Request.NotFound";

            public const string Message =
                "The requested resource was not found.";
        }

        public static class AlreadyExists
        {

            public const int Status = 409;

            public const string Type =
                "https://problems-registry.smartbear.com/already-exists";

            public const string Code = "Resource.AlreadyExists";

            public const string Message = "The resource already exists.";
        }

        public static class Validation
        {

            public const int Status = 422;

            public const string Type =
                "https://problems-registry.smartbear.com/validation-error";

            public const string Code = "Validation.Failed";

            public const string Message =
                "One or more validation errors occurred.";
        }

        public static class BusinessRuleViolation
        {

            public const int Status = 422;

            public const string Type =
                "https://problems-registry.smartbear.com/business-rule-violation";

            public const string Code = "Validation.BusinessRuleViolation";

            public const string Message =
                "A business rule was violated.";
        }

        #endregion

        #region ErrorTypes — Request Validation Errors

        public static class MissingBodyProperty
        {

            public const int Status = 400;

            public const string Type =
                "https://problems-registry.smartbear.com/missing-body-property";

            public const string Code = "Request.MissingBodyProperty";

            public const string Message =
                "The body property '{0}' is required.";
        }

        public static class MissingRequestHeader
        {

            public const int Status = 400;

            public const string Type =
                "https://problems-registry.smartbear.com/missing-request-header";

            public const string Code = "Request.MissingHeader";

            public const string Message =
                "The request header '{0}' is required.";
        }

        public static class MissingRequestParameter
        {

            public const int Status = 400;

            public const string Type =
                "https://problems-registry.smartbear.com/missing-request-parameter";

            public const string Code = "Request.MissingParameter";

            public const string Message =
                "The request parameter '{0}' is required.";
        }

        public static class InvalidBodyPropertyFormat
        {

            public const int Status = 400;

            public const string Type =
                "https://problems-registry.smartbear.com/invalid-body-property-format";

            public const string Code = "Request.InvalidBodyPropertyFormat";

            public const string Message =
                "The body property '{0}' has an invalid format.";
        }

        public static class InvalidBodyPropertyValue
        {

            public const int Status = 400;

            public const string Type =
                "https://problems-registry.smartbear.com/invalid-body-property-value";

            public const string Code = "Request.InvalidBodyPropertyValue";

            public const string Message =
                "The body property '{0}' has an invalid value.";
        }

        public static class InvalidRequestParameterFormat
        {

            public const int Status = 400;

            public const string Type =
                "https://problems-registry.smartbear.com/invalid-request-parameter-format";

            public const string Code = "Request.InvalidParameterFormat";

            public const string Message =
                "The request parameter '{0}' has an invalid format.";
        }

        public static class InvalidRequestParameterValue
        {

            public const int Status = 400;

            public const string Type =
                "https://problems-registry.smartbear.com/invalid-request-parameter-value";

            public const string Code = "Request.InvalidParameterValue";

            public const string Message =
                "The request parameter '{0}' has an invalid value.";
        }

        public static class InvalidRequestHeaderFormat
        {

            public const int Status = 400;

            public const string Type =
                "https://problems-registry.smartbear.com/invalid-request-header-format";

            public const string Code = "Request.InvalidHeaderFormat";

            public const string Message =
                "The request header '{0}' has an invalid format.";
        }

        public static class InvalidParameters
        {

            public const int Status = 400;

            public const string Type =
                "https://problems-registry.smartbear.com/invalid-parameters";

            public const string Code = "Request.InvalidParameters";

            public const string Message =
                "The request parameters are invalid.";
        }

        #endregion

    }
    #endregion
}

using SharedKernel.Meta;

namespace SharedKernel.Errors;

public interface IError : IMetadata
{
    string Code { get; }
    string Message { get; }
    int Status { get; }
}

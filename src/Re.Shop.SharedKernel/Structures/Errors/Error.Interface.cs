using SharedKernel.Structures.Meta;

namespace SharedKernel.Structures.Errors;

public interface IError : IMetadata
{
    string Code { get; }
    string Message { get; }
    int Status { get; }
}

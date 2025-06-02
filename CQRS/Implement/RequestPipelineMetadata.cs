using Gufel.Dispatcher.Base.Dispatcher;

namespace Gufel.Dispatcher.Implement;

public class RequestPipelineMetadata : IRequestPipelineMetadata
{
    private readonly HashSet<string> _requestsWithPipelines;

    public RequestPipelineMetadata(IEnumerable<ValueTuple<Type, Type?>> requestTypes)
    {
        _requestsWithPipelines = [.. requestTypes.Select(x => GetHashSetKey(x.Item1, x.Item2))];
    }

    private static string GetHashSetKey(Type requestType, Type? responseType) =>
        requestType.FullName + "-" + (responseType?.FullName ?? "None");

    public bool HasPipeline(Type requestType, Type? responseType)
        => _requestsWithPipelines.Contains(GetHashSetKey(requestType, responseType));
}
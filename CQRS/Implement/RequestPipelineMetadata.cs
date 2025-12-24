using Gufel.Dispatcher.Base.Dispatcher;

namespace Gufel.Dispatcher.Implement;

public sealed class RequestPipelineMetadata(IEnumerable<(Type Request, Type? Response)> requestTypes)
    : IRequestPipelineMetadata
{
    private readonly HashSet<(Type Request, Type? Response)> _pipelines = new(requestTypes);

    public bool HasPipeline(Type requestType, Type? responseType)
        => _pipelines.Contains((requestType, responseType));
}

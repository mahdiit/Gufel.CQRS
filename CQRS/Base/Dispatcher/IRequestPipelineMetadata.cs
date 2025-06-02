namespace Gufel.Dispatcher.Base.Dispatcher;

public interface IRequestPipelineMetadata
{
    bool HasPipeline(Type requestType, Type? responseType);
}
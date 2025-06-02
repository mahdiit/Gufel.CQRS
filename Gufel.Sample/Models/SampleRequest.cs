using Gufel.Dispatcher.Base.Dispatcher;

namespace Gufel.Sample.Models
{
    public class SampleRequestNoResponse : IRequest
    {
        public int Id { get; set; }
    }

    public class SampleRequest : IRequest<SampleResponse>
    {
        public int Id { get; set; }
    }

    public class SampleResponse
    {
        public bool Result { get; set; }
    }

    public class SampleHandler : IRequestHandler<SampleRequest, SampleResponse>
    {
        public async Task<SampleResponse> Handle(SampleRequest command, CancellationToken cancellation)
        {
            for (int i = 0; i < 5; i++)
            {
                await Task.Delay(500, cancellation);
                Console.WriteLine($" do item {i}");
            }

            return new SampleResponse() { Result = true };
        }
    }

    public class Sample1Pipeline : IPipelineHandler<SampleRequest, SampleResponse>
    {
        public async Task Handle(SampleRequest command, CancellationToken cancellation)
        {
            Console.WriteLine("pipeline 1 run " + command.Id);
            await Task.Delay(1000, cancellation);
        }
    }

    public class Sample2Pipeline : IPipelineHandler<SampleRequest, SampleResponse>
    {
        public async Task Handle(SampleRequest command, CancellationToken cancellation)
        {
            Console.WriteLine("pipeline 2 run " + command.Id);
            await Task.Delay(1000, cancellation);
        }
    }

    public class SampleNoResponseHandler : IRequestHandler<SampleRequestNoResponse>
    {
        public async Task Handle(SampleRequestNoResponse request, CancellationToken cancellation)
        {
            Console.WriteLine(" no response handler " + request.Id);
            await Task.Delay(1000, cancellation);
        }
    }

    public class SampleNoResponsePipeline : IPipelineHandler<SampleRequestNoResponse>
    {
        public async Task Handle(SampleRequestNoResponse command, CancellationToken cancellation)
        {
            Console.WriteLine("pipeline no response run " + command.Id);
            await Task.Delay(1000, cancellation);
        }
    }
}

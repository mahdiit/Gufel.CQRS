using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gufel.CQRS.Base.Dispatcher;

namespace Gufel.Sample.Models
{
    public class SampleRequest
    {
        public int Id { get; set; }
    }

    public class SampleResponse
    {
        public bool Result { get; set; }
    }

    public class SampleHandler : ICommandHandler<SampleRequest, SampleResponse>
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
}

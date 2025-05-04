using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gufel.CQRS.Base.Dispatcher
{
    public interface IPipelineHandler<TCq, TCqResult>
    {
        Task Handle(TCq command, CancellationToken cancellation);
    }
}

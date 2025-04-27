using Gufel.CQRS.Base.Dispatcher;
using Gufel.CQRS.Dispatcher;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace UnitTest
{
    public class DispatcherTest
    {
        public class TestCommandHandler : ICommandHandler<int, bool>
        {
            public Task<bool> Handle(int command, CancellationToken cancellation)
            {
                return Task.FromResult(command > 0);
            }
        }
        public class TestQueryHandler : IQueryHandler<int, bool>
        {
            public Task<bool> Handle(int query, CancellationToken cancellation)
            {
                return Task.FromResult(query > 0);
            }
        }

        [Fact]
        public async Task Command_Valid_Dispatch()
        {
            var srvList = new ServiceCollection();
            srvList.AddSingleton<ICommandHandler<int, bool>, TestCommandHandler>();
            var serviceApp = srvList.BuildServiceProvider();

            var dispacher = new CommandDispatcher(serviceApp);
            var result = await dispacher.Dispatch<int, bool>(10, CancellationToken.None);

            result.ShouldBe(true);
        }

        [Fact]
        public async Task Query_Valid_Dispatch()
        {
            var srvList = new ServiceCollection();
            srvList.AddSingleton<IQueryHandler<int, bool>, TestQueryHandler>();
            var serviceApp = srvList.BuildServiceProvider();

            var dispacher = new QueryDispatcher(serviceApp);
            var resultTrue = await dispacher.Dispatch<int, bool>(10, CancellationToken.None);
            var resultFalse = await dispacher.Dispatch<int, bool>(0, CancellationToken.None);

            resultTrue.ShouldBe(true);
            resultFalse.ShouldBe(false);
        }
    }
}

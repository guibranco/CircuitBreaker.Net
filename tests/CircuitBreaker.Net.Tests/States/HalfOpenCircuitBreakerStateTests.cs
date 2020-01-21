using CircuitBreaker.Net.Exceptions;
using CircuitBreaker.Net.States;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CircuitBreaker.Net.Tests.States
{
    public class HalfOpenCircuitBreakerStateTests
    {
        private readonly TimeSpan _timeout = TimeSpan.FromMilliseconds(100);
        private readonly ICircuitBreakerInvoker _invoker;
        private readonly HalfOpenCircuitBreakerState _sut;

        public HalfOpenCircuitBreakerStateTests()
        {
            var @switch = Substitute.For<ICircuitBreakerSwitch>();
            _invoker = Substitute.For<ICircuitBreakerInvoker>();
            _sut = new HalfOpenCircuitBreakerState(@switch, _invoker, _timeout);
        }

        public class InvokeTests : HalfOpenCircuitBreakerStateTests
        {
            [Fact]
            public void InvokesOnlyFirstAction()
            {
                Action action = () => { };
                _sut.Invoke(action);
                _invoker.Received().InvokeThrough(Arg.Is(_sut), action, _timeout);
                _invoker.ClearReceivedCalls();

                Assert.Throws<CircuitBreakerOpenException>(() => _sut.Invoke(action));
                _invoker.DidNotReceive().InvokeThrough(Arg.Any<ICircuitBreakerState>(), Arg.Any<Action>(), Arg.Any<TimeSpan>());
            }

            [Fact]
            public void InvokesOnlyFirstFunc()
            {
                Func<object> func = () => new object();
                _sut.Invoke(func);
                _invoker.Received().InvokeThrough(Arg.Is(_sut), func, _timeout);
                _invoker.ClearReceivedCalls();

                Assert.Throws<CircuitBreakerOpenException>(() => _sut.Invoke(func));
                _invoker.DidNotReceive().InvokeThrough(Arg.Any<ICircuitBreakerState>(), Arg.Any<Action>(), Arg.Any<TimeSpan>());
            }
        }

        public class InvokeAsyncTests : HalfOpenCircuitBreakerStateTests
        {
            [Fact]
            public async void InvokeAsyncOnlyFirstAction()
            {
                Func<Task> action = () => Task.FromResult(false);
                await _sut.InvokeAsync(action);
                await _invoker.Received().InvokeThroughAsync(Arg.Is(_sut), action, _timeout);
                _invoker.ClearReceivedCalls();

                await Assert.ThrowsAsync<CircuitBreakerOpenException>(() => _sut.InvokeAsync(action));
                await _invoker.DidNotReceive().InvokeThroughAsync(Arg.Any<ICircuitBreakerState>(), Arg.Any<Func<Task>>(), Arg.Any<TimeSpan>());
            }

            [Fact]
            public async void InvokeAsyncOnlyFirstFunc()
            {
                Func<Task<object>> func = () => Task.FromResult(new object());
                await _sut.InvokeAsync(func);
                await _invoker.Received().InvokeThroughAsync(Arg.Is(_sut), func, _timeout);
                _invoker.ClearReceivedCalls();

                await Assert.ThrowsAsync<CircuitBreakerOpenException>(() => _sut.InvokeAsync(func));
                await _invoker.DidNotReceive().InvokeThroughAsync(Arg.Any<ICircuitBreakerState>(), Arg.Any<Func<Task<object>>>(), Arg.Any<TimeSpan>());
            }
        }
    }
}
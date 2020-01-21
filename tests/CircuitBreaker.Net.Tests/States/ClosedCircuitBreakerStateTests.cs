using CircuitBreaker.Net.States;
using NSubstitute;
using System;
using Xunit;

namespace CircuitBreaker.Net.Tests.States
{
    public class ClosedCircuitBreakerStateTests
    {
        private const int MaxFailures = 3;

        private readonly TimeSpan _timeout = TimeSpan.FromMilliseconds(100);
        private readonly ClosedCircuitBreakerState _sut;
        private readonly ICircuitBreakerSwitch _switch;

        public ClosedCircuitBreakerStateTests()
        {
            _switch = Substitute.For<ICircuitBreakerSwitch>();
            var invoker = Substitute.For<ICircuitBreakerInvoker>();
            _sut = new ClosedCircuitBreakerState(_switch, invoker, MaxFailures, _timeout);
        }

        public class InvocationFailsTests : ClosedCircuitBreakerStateTests
        {
            [Fact]
            public void OpensCircuitAfterMaxFailures()
            {
                _sut.InvocationFails();
                _switch.DidNotReceive().OpenCircuit(Arg.Any<ICircuitBreakerState>());
                _sut.InvocationFails();
                _sut.InvocationFails();
                _switch.Received().OpenCircuit(Arg.Is(_sut));
            }
        }
    }
}
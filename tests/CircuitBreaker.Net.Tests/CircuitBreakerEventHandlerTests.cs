using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CircuitBreaker.Net.Tests
{
    public class CircuitBreakerEventHandlerTests
    {
        private const int MaxFailures = 1;
        private readonly TimeSpan _resetTimeout = TimeSpan.FromMilliseconds(100);
        private readonly TimeSpan _timeout = TimeSpan.FromMilliseconds(100);
        private readonly CircuitBreaker _sut;
        private readonly ICircuitBreakerEventHandler _eventHandler;

        public CircuitBreakerEventHandlerTests()
        {
            _sut = new CircuitBreaker(TaskScheduler.Default, MaxFailures, _timeout, _resetTimeout);
            _eventHandler = Substitute.For<ICircuitBreakerEventHandler>();
            _sut.EventHandler = _eventHandler;
        }

        private readonly Action _anyAction = () => { };
        private readonly Action _throwAction = () => { throw new Exception(); };

        [Fact]
        public void TriggersEvents()
        {
            try
            {
                _sut.Execute(_throwAction);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            { }
            _eventHandler.Received().OnCircuitOpened(_sut);

            Thread.Sleep(_resetTimeout);
            Thread.Sleep(10);

            _sut.Execute(_anyAction);
            _eventHandler.Received().OnCircuitHalfOpened(_sut);

            _sut.Execute(_anyAction);
            _eventHandler.Received().OnCircuitClosed(_sut);
        }
    }
}
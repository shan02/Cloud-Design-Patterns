using System;
using System.Threading;

namespace DesingPattern
{
    /// <summary>
    ///     When continuous failure happens and failure threshold has been reached,
    ///     state machine will be tripped to Open state.
    /// </summary>
    public class CircuitOpenState : ICircuitBreakerState
    {
        private CircuitBreakerStateMachine _stateMachine;
        private static System.Timers.Timer _timeoutTimer;
        private int _timeout;

        public CircuitOpenState(CircuitBreakerStateMachine stateMachine, int timeout = 500 /* Modify as needed. */)
        {
            _timeout = timeout;
            _stateMachine = stateMachine;

            Enter();
        }

        /// <summary>
        ///     Start a timer, when enterting this state.
        /// </summary>
        public void Enter()
        {
            _timeoutTimer = new System.Timers.Timer(_timeout);
            _timeoutTimer.Elapsed += OnTimedEvent;
            _timeoutTimer.AutoReset = true;
            _timeoutTimer.Enabled = true;
        }

        /// <summary>
        ///     Trip back to Close state, when the timer elapses.
        /// </summary>
        /// <param name="newState"></param>
        public void Exit(ICircuitBreakerState newState)
        {
            Interlocked.Exchange(ref _stateMachine.InternalState, newState);
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            Exit(new CircuitHalfOpenState(_stateMachine));
        }
    }
}

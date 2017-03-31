using System;
using System.Threading;

namespace DesingPattern
{
    /// <summary>
    ///     Actions can be executed as there were no errors with the previous N actions.
    /// </summary>
    public class CircuitClosedState : ICircuitBreakerState
    {
        public int FailureCounter = 0;
        public CircuitBreakerStateMachine _stateMachine;
        public int _failureThreshold = 0;

        public CircuitClosedState(CircuitBreakerStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _failureThreshold = stateMachine.FailureThreshold;
        }

        /// <summary>
        ///     Set/Reset the failure counter, when entering closed state.
        /// </summary>
        public void Enter()
        {
            FailureCounter = 0;
        }

        /// <summary>
        ///      Trip to Open State after failure threshold. 
        /// </summary>
        /// <param name="newState"></param>
        public void Exit(ICircuitBreakerState newState)
        {
            Interlocked.Exchange(ref _stateMachine.InternalState, newState);
        }

        public void Execute(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref FailureCounter);
                if (FailureCounter == _failureThreshold)
                {
                    Exit(new CircuitOpenState(_stateMachine));
                }
                
                // Log exceptions                
            }
        }
    }
}

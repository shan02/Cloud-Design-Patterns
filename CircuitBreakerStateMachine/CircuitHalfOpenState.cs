using System;
using System.Threading;

namespace DesingPattern
{
    /// <summary>
    ///     Half open state: Restrict the number of calls that can be made when
    ///     on HalfOpenState. This state helps recovering service without overloading
    ///     the server, giving sometime to come back to the normal stage.
    /// </summary>
    public class CircuitHalfOpenState : ICircuitBreakerState
    {
        private CircuitBreakerStateMachine _stateMachine;
        private int SuccessCounter = 0;
        private readonly object _halfOpenSync = new object();

        public CircuitHalfOpenState(CircuitBreakerStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            SuccessCounter = 0;
        }

        public void Execute(Action action)
        {
            try
            {
                if (Monitor.TryEnter(_halfOpenSync))
                {
                    action.Invoke();

                    // Only if the success threshold has reached trip to closed state and allow all requests. 
                    Interlocked.Increment(ref SuccessCounter);
                    if (SuccessCounter == 5)
                    {
                        Exit(new CircuitClosedState(_stateMachine));
                    }
                }
            }
            catch (Exception ex)
            {
                Exit(new CircuitOpenState(_stateMachine));
                // Log the exception
            }
            finally
            {
                Monitor.Exit(_halfOpenSync);
            }
        }

        public void Exit(ICircuitBreakerState newState)
        {
            Interlocked.Exchange(ref _stateMachine.InternalState, newState);
        }
    }
}

using System;
using DesignPattern.Extensions;

namespace DesingPattern
{
    public class CircuitBreakerStateMachine
    {
        public ICircuitBreakerState InternalState;
        public int FailureThreshold;
        public int SuccessThreshold;
        public State CurrentState => InternalState.GetCurrentState();
               
        public CircuitBreakerStateMachine(int failureThreshold, int successThreshold)
        {
            FailureThreshold = failureThreshold;
            SuccessThreshold = successThreshold;

            InternalState = new CircuitClosedState(this);
        }

        public void Execute(Action action)
        {
            if (InternalState.GetType() == typeof(CircuitClosedState))
            {
                ((CircuitClosedState)InternalState).Execute(action);
            }
            else if(InternalState.GetType() == typeof(CircuitHalfOpenState))
            {
                ((CircuitHalfOpenState)InternalState).Execute(action);
            }
            else if (InternalState.GetType() == typeof(CircuitOpenState))
            {
                // Fail Immediately
                // Log
            }
        }
    }   
}

using DesingPattern;

namespace DesignPattern.Extensions
{
    public static class CircuitBreakerStateMachineExtensions
    {
        public static State GetCurrentState(this ICircuitBreakerState stateMachine)
        {
            if (stateMachine == typeof(CircuitClosedState))
            {
                return State.Closed;
            }
            else if (stateMachine == typeof(CircuitHalfOpenState))
            {
                return State.HalfOpen;
            }
            else
            {
                return State.Open;
            }
        }
    }
}

namespace DesingPattern
{
    public interface ICircuitBreakerState
    {
        void Enter();
        void Exit(ICircuitBreakerState newState);
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;

namespace CircuitBreakerStateMachineTests
{
    [TestClass]
    public class CircuitBreakerStateMachineTests
    {
        private DesingPattern.CircuitBreakerStateMachine stateMachine;
        
        [TestInitialize]
        public void Init()
        {
            stateMachine = new DesingPattern.CircuitBreakerStateMachine(5, 5);
        }
        
        /// <summary>
        ///     State machine in closed state and successful execution of action.
        /// </summary>
        [TestMethod]
        public void Closed_State_Success()
        {
            stateMachine.Execute(() => TestMethod());
            Assert.IsTrue(stateMachine.CurrentState == DesingPattern.State.Closed);
        }

        /// <summary>
        ///     Continuous 5 exceptions should trip the state machine to open state.
        /// </summary>
        [TestMethod]
        public void Open_State_Success()
        {
            stateMachine.Execute(() => ExceptionMethod());
            stateMachine.Execute(() => ExceptionMethod());
            stateMachine.Execute(() => ExceptionMethod());
            stateMachine.Execute(() => ExceptionMethod());
            stateMachine.Execute(() => ExceptionMethod());

            Assert.IsTrue(stateMachine.CurrentState == DesingPattern.State.Open);
        }

        /// <summary>
        ///     Ensure the timer timeout trips back from half open to closed state.
        /// </summary>
        [TestMethod]
        public void HalfOpen_State_Success()
        {
            stateMachine.Execute(() => ExceptionMethod());
            stateMachine.Execute(() => ExceptionMethod());
            stateMachine.Execute(() => ExceptionMethod());
            stateMachine.Execute(() => ExceptionMethod());
            stateMachine.Execute(() => ExceptionMethod());

            Assert.IsTrue(stateMachine.CurrentState == DesingPattern.State.Open);

            Thread.Sleep(600);

            // One more successful operation should put back the state machine to half open.
            stateMachine.Execute(() => TestMethod());
            Assert.IsTrue(stateMachine.CurrentState == DesingPattern.State.HalfOpen);

            // If the state is in half open continuous five calls should succeed should to trip back to closed state.
            stateMachine.Execute(() => TestMethod());
            stateMachine.Execute(() => TestMethod());
            stateMachine.Execute(() => TestMethod());
            stateMachine.Execute(() => TestMethod());
            stateMachine.Execute(() => TestMethod());

            Assert.IsTrue(stateMachine.CurrentState == DesingPattern.State.Closed);
        }
        
        private void TestMethod()
        {
            Console.WriteLine("Execution of test method");
        }

        private void ExceptionMethod()
        {
            throw new Exception("Exception 1");
        }
    }
}

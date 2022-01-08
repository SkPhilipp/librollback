using System;

namespace Rollback.tests
{
    /// <summary>
    /// Abstract fuzzer context allowing for fuzzer-based evaluation of rollback implementations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RollbackFuzzerContext<T> where T : IRollback
    {
        protected enum ExecutionMode
        {
            Dual,
            Single
        }

        private int _rollbackTime;
        protected int Time;
        protected ExecutionMode Mode;
        protected readonly T Main;
        protected readonly T Control;

        protected RollbackFuzzerContext(T main, T control)
        {
            Time = 0;
            _rollbackTime = 0;
            Mode = ExecutionMode.Dual;
            Main = main;
            Control = control;
        }

        /// <summary>
        /// Assigns a rollback time and steps into the next execution mode.
        /// </summary>
        public void StepRollbackPlan()
        {
            Time++;
            _rollbackTime = Time;
            Mode = ExecutionMode.Single;
        }

        /// <summary>
        /// Rolls back the main rollback implementation object and steps into the next execution mode.
        /// </summary>
        public void StepRollbackPerform()
        {
            Main.Rollback(_rollbackTime);
            Time = _rollbackTime;
        }

        /// <summary>
        /// Rolls back the main rollback implementation object and steps into the next execution mode.
        ///
        /// Verifies that the main and control objects are equal, and resets execution mode to DUAL to allow further testing cycles.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void StepComplete()
        {
            StepRollbackPerform();
            Time++;
            Mode = ExecutionMode.Dual;
            if (!Main.Equals(Control))
            {
                throw new Exception($"(Main) {Main} != (Control) {Control}");
            }
        }
    }
}
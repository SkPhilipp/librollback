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
        protected ExecutionMode Mode;
        protected readonly RollbackClock Clock;
        protected readonly T Main;
        protected readonly T Control;

        protected RollbackFuzzerContext(RollbackClock clock, T main, T control)
        {
            Clock = clock;
            _rollbackTime = 0;
            Mode = ExecutionMode.Dual;
            Main = main;
            Control = control;
        }

        /// <summary>
        /// Applies an action to the main and optionally to the control object, depending on the current execution mode.
        /// </summary>
        /// <param name="action"></param>
        public void Apply(Action<T> action)
        {
            action(Main);
            if (Mode == ExecutionMode.Dual)
            {
                action(Control);
            }
        }

        /// <summary>
        /// Assigns a rollback time and steps into the next execution mode.
        /// </summary>
        public void StepRollbackPlan()
        {
            Clock.Tick();
            _rollbackTime = Clock.Time;
            Mode = ExecutionMode.Single;
        }

        /// <summary>
        /// Rolls back the main rollback implementation object and steps into the next execution mode.
        /// </summary>
        public void StepRollbackPerform()
        {
            Clock.MoveTo(_rollbackTime);
            Main.Rollback();
        }

        /// <summary>
        /// Rolls back the main rollback implementation object and steps into the next execution mode.
        ///
        /// Verifies that the main and control objects are equal, and resets execution mode to DUAL to allow further testing cycles.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void StepComplete()
        {
            Clock.Tick();
            StepRollbackPerform();
            Mode = ExecutionMode.Dual;
            if (!Main.Equals(Control))
            {
                throw new Exception($"(Main) {Main} != (Control) {Control}");
            }
        }
    }
}
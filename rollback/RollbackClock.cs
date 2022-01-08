namespace Rollback.structures
{
    public class RollbackClock
    {
        /// <summary>
        /// Previous value of Time
        /// </summary>
        public int LastTime { get; private set; }

        /// <summary>
        /// Current value of Time
        /// </summary>
        public int Time { get; private set; }

        public RollbackClock(int time)
        {
            LastTime = time;
            Time = time;
        }

        /// <summary>
        /// Advances Time by one.
        /// </summary>
        public void Tick()
        {
            LastTime = Time;
            Time++;
        }

        /// <summary>
        /// Advances Time to the specified value.
        /// </summary>
        /// <param name="newTime"></param>
        public void MoveTo(int newTime)
        {
            LastTime = Time;
            Time = newTime;
        }
    }
}
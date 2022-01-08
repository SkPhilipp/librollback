namespace Rollback
{
    internal static class RollbackConfiguration
    {
        public const int Frames = 200;
    }

    /// <summary>
    /// Indicates support for rollback.
    ///
    /// Objects implementing this class keep track of their current point in time, generally through the use of an external clock.
    /// </summary>
    public interface IRollback
    {
        /// <summary>
        /// Rolls this object back to its clock's current point in time.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Removes all currently contained rollback data.
        /// </summary>
        void RollbackClear();
    }
}
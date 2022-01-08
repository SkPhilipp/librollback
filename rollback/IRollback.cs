namespace Rollback
{
    internal static class RollbackConfiguration
    {
        public const int Frames = 200;
    }

    /// <summary>
    /// Indicates support for rollback functionalities.
    /// </summary>
    public interface IRollback
    {
        /// <summary>
        /// Rolls this object back to a point in time.
        /// </summary>
        /// <param name="frameTime"></param>
        void Rollback(int frameTime);

        /// <summary>
        /// Resets rollback data.
        /// </summary>
        void RollbackClear();
    }
}
namespace Rollback
{
    /// <summary>
    /// Indicates support for rollback functionalities where the frame time is embedded in the implementation.
    /// </summary>
    public interface IRollbackEmbedded
    {
        /// <summary>
        /// Rolls this object back to its current point in time.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Resets rollback data.
        /// </summary>
        void RollbackClear();
    }
}
namespace Sprite.Caching
{
    public enum ReasonDeletion
    {
        None,

        /// <summary>
        /// Manually
        /// </summary>
        Removed,

        /// <summary>
        /// Overwritten
        /// </summary>
        Replaced,

        /// <summary>
        /// Timed out
        /// </summary>
        Expired,
    }
}
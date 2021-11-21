using System.Collections.Generic;

namespace Sprite.Sessions
{
    public interface ICurrentSession
    {
        string Id { get; }

        bool IsAvailable { get; }

        string ChangeSessionId { get; }

        IEnumerable<string> Keys { get; }

        /// <summary>
        /// Retrieve the value of the given key, if present.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGetValue(string key, out byte[] value);

        /// <summary>
        /// Set the given key and value in the current session. This will throw if the session
        /// was not established prior to sending the response.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Set(string key, byte[] value);

        /// <summary>
        /// Remove the given key from the session if present.
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);

        /// <summary>
        /// Remove all entries from the current session, if any.
        /// The session cookie is not removed.
        /// </summary>
        void Clear();
    }
}
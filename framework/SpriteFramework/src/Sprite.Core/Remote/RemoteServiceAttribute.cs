using System;

namespace Sprite.Remote
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method)]
    public class RemoteServiceAttribute : Attribute
    {
        public RemoteServiceAttribute(bool isEnabled = true)
        {
            IsEnabled = isEnabled;
        }

        /// <summary>
        /// Default: true.
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}
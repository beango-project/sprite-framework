using System;

namespace Sprite.Data.Persistence
{
    /// <summary>
    /// 持久化提供商/ORM
    /// </summary>
    public interface IVendor : IDisposable
    {
        bool IsDispose { get; }
    }
}
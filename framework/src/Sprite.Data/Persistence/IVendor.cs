using System;
using System.Data.Common;

namespace Sprite.Data.Persistence
{
    /// <summary>
    /// 持久化提供商/ORM
    /// </summary>
    public interface IVendor : IDisposable, IAsyncDisposable
    {
        DbConnection DbConnection { get; }
    }
}
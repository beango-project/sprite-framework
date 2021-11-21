using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
// using Microsoft.AspNetCore.Http;
using Sprite.Sessions;

namespace Sprite.Session.Web
{
    // public class HttpSessionAdapter<TSession> : ISession, ICurrentSession
    //     where TSession : ISession
    // {
    //     protected TSession Session { get; }
    //
    //     public string ChangeSessionId { get; }
    //
    //     public bool IsAvailable => Session.IsAvailable;
    //
    //     public string Id => Session.Id;
    //
    //     public IEnumerable<string> Keys => Session.Keys;
    //     public Task LoadAsync(CancellationToken cancellationToken = default) => LoadAsync(cancellationToken);
    //
    //     public Task CommitAsync(CancellationToken cancellationToken = default) => CommitAsync(cancellationToken);
    //
    //
    //     public bool TryGetValue(string key, out byte[]? value) => TryGetValue(key, out value);
    //
    //     public void Set(string key, byte[] value) => Session.Set(key, value);
    //
    //     public void Remove(string key) => Session.Remove(key);
    //
    //     public void Clear() => Session.Clear();
    // }
}
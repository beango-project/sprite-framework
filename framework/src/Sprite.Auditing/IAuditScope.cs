using System;
using System.Collections.Generic;

namespace Sprite.Auditing
{
    public interface IAuditScope : IDisposable
    {
        AuditConfigOptions Options { get; }
        
        AuditLogEntry Log { get; }

        bool IsDisposed { get; }

        void End();

        Task EndAsync();
    }
}
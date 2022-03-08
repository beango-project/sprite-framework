using System;
using System.Collections.Generic;

namespace Sprite.Auditing
{
    public class AuditScope : IAuditScope
    {
        public AuditScopeContext Context { get; }
        public AuditConfigOptions Options { get; private set; }

        public AuditLogEntry Log { get; }
        public bool IsDisposed => _isDisposed;

        private bool _isDisposed;

        public AuditScope(AuditConfigOptions options, AuditLogEntry log)
        {
            Options = options;
            Log = log;
        }
        
        
        public void End()
        {
            
        }

        public Task EndAsync()
        {
            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            _isDisposed = true;
            Options = null;
        }
    }
}
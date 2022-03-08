using System;
using System.Threading;
using Nito.AsyncEx;

namespace Sprite.Auditing
{
    public class AmbientAuditScope
    {
        private static AsyncLocal<InternalAuditScope> _asyncCell = new AsyncLocal<InternalAuditScope>(ValueChanged);

        /// <summary>
        /// Get current scope
        /// </summary>
        public static AmbientAuditScope Current => _current;

        private static volatile AmbientAuditScope _current;

        /// <summary>
        /// Get Current AuditScope 
        /// </summary>
        public IAuditScope AuditScope => GetScope();

        private IAuditScope GetScope()
        {
            var current = _asyncCell.Value;
            while (current?.AuditScope.IsDisposed == true)
            {
                _asyncCell.Value = current = current?.OuterScope;
            }

            return current.AuditScope;
        }

        /// <summary>
        /// Setting  AuditScope 
        /// </summary>
        /// <param name="auditScope"></param>
        /// <returns></returns>
        public IAuditScope SetAuditScope(IAuditScope auditScope)
        {
            var outerScope = _asyncCell?.Value;
            _asyncCell.Value = new InternalAuditScope(auditScope, outerScope);
            return Current.AuditScope;
        }

        private static void ValueChanged(AsyncLocalValueChangedArgs<InternalAuditScope> args)
        {
            if (args.ThreadContextChanged)
            {
                var previous = args.PreviousValue;
                var current = args.CurrentValue;
                
                if (current == null || previous?.AuditScope.IsDisposed == false)
                {
                    _asyncCell.Value = previous;
                }
            }
        }

        static AmbientAuditScope()
        {
            if (_current == null)
            {
                lock (typeof(AmbientAuditScope))
                {
                    if (_current == null)
                    {
                        _current = new AmbientAuditScope();
                    }
                }
            }
        }

        private class InternalAuditScope
        {
            public Guid Id { get; }

            public IAuditScope AuditScope { get; }


            public InternalAuditScope OuterScope { get; }

            public InternalAuditScope(IAuditScope auditScope, InternalAuditScope outerScope)
            {
                Id = Guid.NewGuid();
                AuditScope = auditScope;
                OuterScope = outerScope;
            }
        }
    }
}
using System;

namespace Sprite.Common
{
    public abstract class Enricher<TContext> : IEnricher<TContext>
    {
        private readonly string _propertyName;
        private readonly Func<TContext, object> _propertyValueFactory;
        private readonly bool _overwrite;
        private readonly Func<TContext, bool>? _propertyPredict;

        /// <summary>
        /// EnrichAction
        /// (context,propertyName,propertyValueFactory,overwrite)=>{}
        /// </summary>
        protected abstract Action<TContext, string, Func<TContext, object>, bool>? EnrichAction { get; }

        protected Enricher(string propertyName, object propertyValue, bool overwrite = false) : this(propertyName, _ => propertyValue, overwrite)
        {
        }

        protected Enricher(string propertyName, Func<TContext, object> propertyValueFactory,
            bool overwrite = false) : this(propertyName, propertyValueFactory, null, overwrite)
        {
        }

        protected Enricher(string propertyName, Func<TContext, object> propertyValueFactory, Func<TContext, bool>? propertyPredict,
            bool overwrite = false)
        {
            _propertyName = propertyName;
            _propertyValueFactory = propertyValueFactory;
            _propertyPredict = propertyPredict;
            _overwrite = overwrite;
        }

        public void Enrich(TContext context)
        {
            if (EnrichAction != null && _propertyPredict?.Invoke(context) != false)
            {
                EnrichAction.Invoke(context, _propertyName, _propertyValueFactory, _overwrite);
            }
        }
    }
}
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sprite.DependencyInjection.Attributes;
using Sprite.ObjectMapping;

namespace Sprite.AspNetCore.Mvc
{
    public abstract class SpriteController : Controller
    {
        [Autowired]
        private Lazy<ILogger> _lazyLogger;

        [Autowired]
        private Lazy<IObjectMapper> _lazyObjectMapper;

        protected IObjectMapper ObjectMapper => _lazyObjectMapper?.Value;

        protected ILogger Logger => _lazyLogger?.Value;
    }
}
using Sprite.AspNetCore.Mvc.Conventions;
using Sprite.AspNetCore.Mvc.Results;

namespace Sprite.AspNetCore.Mvc
{
    public class AspNetCoreMvcOptions
    {
        public ResultHandler ResultHandler { get; private set; } = new DefaultResultHandler();

        public AspNetCoreMvcOptions()
        {
            NormalizerResult = true;
            Controllers = new ConventionalControllerOptions();
        }

        public bool NormalizerResult { get; set; }

        public ConventionalControllerOptions Controllers { get; }

        public void UseResultHandler<TResultHandler>() where TResultHandler : ResultHandler, new()
        {
            ResultHandler = new TResultHandler();
        }
    }
}
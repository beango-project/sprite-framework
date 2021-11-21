using Microsoft.AspNetCore.Mvc.Filters;

namespace Sprite.AspNetCore.Mvc.Results
{
    public abstract class ResultHandler
    {
        public abstract void Handle(ResultExecutingContext context);
    }
}
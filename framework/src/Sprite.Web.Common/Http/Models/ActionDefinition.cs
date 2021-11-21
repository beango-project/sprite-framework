using System.Collections.Generic;

namespace Sprite.Web.Http.Models
{
    public class ActionDefinition
    {
        public string Name { get; set; }

        public string HttpMethod { get; set; }

        public IList<string> Path { get; set; }

        public IList<string> SupportedVersions { get; set; }

        public IList<ParameterDefinition> Parameters { get; set; }
    }
}
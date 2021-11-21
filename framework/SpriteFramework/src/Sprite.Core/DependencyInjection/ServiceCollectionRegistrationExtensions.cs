using Sprite.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionRegistrationExtensions
    {
        public static ExportServiceActivator GetExportServiceActivator(this IServiceCollection services)
        {
            return GetOrCreateExportServiceActivator(services);
        }

        private static ExportServiceActivator GetOrCreateExportServiceActivator(IServiceCollection services)
        {
            var actionList = services.GetSingletonInstance<ExportServiceActivator>();
            if (actionList == null)
            {
                actionList = new ExportServiceActivator();
                services.AddSingleton(typeof(ExportServiceActivator), actionList);
            }

            return actionList;
        }
    }
}
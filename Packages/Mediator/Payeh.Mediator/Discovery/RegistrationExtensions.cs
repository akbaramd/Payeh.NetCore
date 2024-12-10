using System.Reflection;

namespace Payeh.Mediator.Discovery
{
    public static class RegistrationExtensions
    {
        /// <summary>
        /// Scans the provided assemblies for request handlers and event handlers and registers them with the service collection.
        /// </summary>
        /// <param name="services">The IServiceCollection to add handlers to.</param>
        /// <param name="assemblies">The assemblies to scan.</param>
        public static void RegisterMediatorHandlers(this IServiceCollection services, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                services.RegisterHandlers(assembly);
                services.RegisterEventHandlers(assembly);
            }
        }
    }
}
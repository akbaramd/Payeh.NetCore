using System.Reflection;
using Payeh.Mediator.Abstractions.Handlers;

namespace Payeh.Mediator.Discovery
{
    public static class HandlerAssemblyScanner
    {
        /// <summary>
        /// Scans the provided assembly for IRequestHandler implementations and registers them with the service collection.
        /// </summary>
        /// <param name="services">The IServiceCollection to add handlers to.</param>
        /// <param name="assembly">The assembly to scan.</param>
        public static void RegisterHandlers(this IServiceCollection services, Assembly assembly)
        {
            var handlerTypes = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                    .Select(i => new { HandlerType = t, InterfaceType = i }))
                .ToList();

            foreach (var handler in handlerTypes)
            {
                services.AddTransient(handler.InterfaceType, handler.HandlerType);
            }
        }
    }
}
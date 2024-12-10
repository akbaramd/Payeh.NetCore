using System.Reflection;
using Payeh.Mediator.Abstractions.Events;

namespace Payeh.Mediator.Discovery
{
    public static class EventHandlerAssemblyScanner
    {
        /// <summary>
        /// Scans the provided assembly for IEventHandler implementations and registers them with the service collection.
        /// </summary>
        /// <param name="services">The IServiceCollection to add event handlers to.</param>
        /// <param name="assembly">The assembly to scan.</param>
        public static void RegisterEventHandlers(this IServiceCollection services, Assembly assembly)
        {
            var eventHandlerTypes = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                    .Select(i => new { HandlerType = t, InterfaceType = i }))
                .ToList();

            foreach (var handler in eventHandlerTypes)
            {
                services.AddTransient(handler.InterfaceType, handler.HandlerType);
            }
        }
    }
}
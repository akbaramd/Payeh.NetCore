using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Payeh.Mediator.Tests;

public class MediatorTestSetup
{
    public ServiceProvider ServiceProvider { get; private set; }

    public MediatorTestSetup()
    {
        var services = new ServiceCollection();

        // Add mediator with test configurations
        services.AddMediator(config =>
        {
            config.AddAssemblies(Assembly.GetExecutingAssembly());
            config.Memento.IsEnabled = true;
        });

        ServiceProvider = services.BuildServiceProvider();
    }
}
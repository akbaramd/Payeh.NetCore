using Microsoft.Extensions.DependencyInjection;
using Payeh.Mediator;
using Payeh.Mediator.Abstractions;
using Payeh.Mediator.Memento;
using Payeh.Mediator.Pipeline;
using System.Reflection;
using Payeh.Mediator.Abstractions.Memento;

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
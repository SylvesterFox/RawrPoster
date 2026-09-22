using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RawPoster.UI.ViewModels;
using RawPoster.UI.Views;
using RawrPoster.Infrastructure;
using RawrPoster.Infrastructure.Persistence;
using Serilog;

namespace RawPoster.UI;

public partial class App : Application
{
    private ServiceProvider? _services;
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
#if DEBUG
        this.AttachDeveloperTools();
#endif
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RawrPoster", "logs", "rawrposter-.log");
        Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.File(logPath, rollingInterval: RollingInterval.Day).CreateLogger();
        var collection = new ServiceCollection();
        collection.AddInfrastructure();
        collection.AddTransient<MainViewModel>();
        _services = collection.BuildServiceProvider();
        using (var scope = _services.CreateScope())
            scope.ServiceProvider.GetRequiredService<RawrPosterDbContext>().Database.EnsureCreated();
        var viewModel = _services.GetRequiredService<MainViewModel>();
        _ = viewModel.InitializeAsync();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = viewModel
            };
        }
        else if (ApplicationLifetime is IActivityApplicationLifetime singleViewFactoryApplicationLifetime)
        {
            singleViewFactoryApplicationLifetime.MainViewFactory = () => new MainView { DataContext = viewModel };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = viewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}

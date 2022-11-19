using System.Reflection;
using System.Runtime.CompilerServices;
using AiForms.Settings;
using Sample.Views;

namespace Sample;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UsePrism(prism =>
            {
                prism.RegisterTypes(registry =>
                {
                    registry.RegisterForNavigation<NavigationPage>();
                    registry.RegisterForNavigation<MainPage>();
                    registry.RegisterForNavigation<AutoTest>();
                    registry.RegisterForNavigation<IndexPage>();
                    registry.RegisterForNavigation<ManualTest>();
                    registry.RegisterForNavigation<ResultPage>();
                    registry.RegisterForNavigation<SurveyPage>();
                });

                prism.OnAppStart(async navigationService =>
                {
                    await navigationService.NavigateAsync("/NavigationPage/IndexPage");
                });
            })
            .ConfigureMauiHandlers(handlers =>
            {
                handlers.AddSettingsViewHandler();
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        return builder.Build();
    }
}


using System.Reflection;
using System.Runtime.CompilerServices;
using AiForms.Settings;
using Prism.Ioc;
using Sample.ViewModels;
using Sample.ViewModels.Dialogs;
using Sample.Views;
using Sample.Views.Dialogs;

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
                    registry.RegisterForNavigation<VmTest, VmTestViewModel>();
                    registry.RegisterDialog<VmDialog, VmDialogViewModel>();
                });

                prism.OnInitialized(container =>
                {
                    var registry = container.Resolve<IDialogViewRegistry>();
                    AiForms.Dialogs.Configurations.SetIocConfig(
                        type => registry.Registrations.Where(x => x.ViewModel == type).LastOrDefault()?.View,
                        container.Resolve
                    );
                });

                prism.OnAppStart(async (container, navigationService) =>
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


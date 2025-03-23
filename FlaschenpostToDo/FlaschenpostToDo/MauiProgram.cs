using FlaschenpostToDo.Data.Network;
using FlaschenpostToDo.Domain;
using FlaschenpostToDo.Network;
using FlaschenpostToDo.Presentation.View;
using FlaschenpostToDo.Presentation.ViewModel;
using Microsoft.Extensions.Logging;

namespace FlaschenpostToDo;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<TodoPage>();
        builder.Services.AddSingleton<TodoViewModel>();
        builder.Services.AddTransient<DonePage>();
        builder.Services.AddTransient<DoneViewModel>();
        builder.Services.AddTransient<DetailsPage>();
        builder.Services.AddTransient<DetailsViewModel>();
        builder.Services.AddSingleton<ITodoController, TodoItemsController>();
        builder.Services.AddSingleton<INetworkStatusController, NetworkStatusController>();
        builder.Services.AddSingleton<IApiClient>(_ =>
        {
            var socketsHttpHandler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(5)
            };
            var httpclient = new HttpClient(socketsHttpHandler);
            return new ApiClient($"{Settings.Protocol}://{Settings.ApiAddress}:{Settings.Port}", httpclient);
        });


        return builder.Build();
    }
}
﻿using Microsoft.Extensions.Logging;
using MobileApp.Services;
using DotNetEnv;
using Syncfusion.Blazor;

namespace MobileApp
{
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
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSyncfusionBlazor();

            builder.Services.AddSingleton<RealtimeLocation>();
            builder.Services.AddSingleton<BluetoothLowEnergy>();
            builder.Services.AddScoped<EmergencyCall>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            Env.Load();

            return builder.Build();
        }
    }
}

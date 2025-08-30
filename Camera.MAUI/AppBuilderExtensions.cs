namespace Camera.MAUI;

public static class AppBuilderExtensions
{
    public static MauiAppBuilder UseMauiCameraView(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(h =>
        {
            h.AddHandler(typeof(CameraView), typeof(CameraViewHandler));
        });

        builder.Services.AddSingleton<CameraService>();

        return builder;
    }
}

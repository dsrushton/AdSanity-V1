namespace AdSanity_MAUI;

public partial class App
{
    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Resolve MainPage with injected dependencies
        var mainPage = _serviceProvider.GetRequiredService<MainPage>();
        return new Window(mainPage);
    }
}
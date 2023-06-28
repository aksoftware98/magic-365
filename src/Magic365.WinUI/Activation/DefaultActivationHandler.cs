using Magic365.WinUI.Contracts.Services;
using Magic365.WinUI.ViewModels;

using Microsoft.UI.Xaml;

namespace Magic365.WinUI.Activation;

public class DefaultActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
{
    private readonly IWinUINavigationService _navigationService;

    public DefaultActivationHandler(IWinUINavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
    {
        // None of the ActivationHandlers has handled the activation.
        return _navigationService.Frame?.Content == null;
    }

    protected async override Task HandleInternalAsync(LaunchActivatedEventArgs args)
    {
        _navigationService.NavigateTo(typeof(MainViewModel).FullName!, args.Arguments);

        await Task.CompletedTask;
    }
}

using Magic365.Client.ViewModels.Interfaces;
using Magic365.Client.ViewModels;
using Magic365.WinUI.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Magic365.WinUI.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel
    {
        get;
    }

    public SettingsPage()
    {
        ViewModel = App.GetService<SettingsViewModel>();
        InitializeComponent();

        var usagesService = App.GetService<IUsagesClient>();
        usagesService.TrackEventAsync(SessionVariables.User.AccessToken, new()
        {
            UserId = SessionVariables.User.Email,
            EventName = "Open Settings Page",
            SessionId = SessionVariables.SessionId
        });
    }
}


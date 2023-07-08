using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magic365.Client.ViewModels;
using Magic365.Client.ViewModels.Interfaces;
using Magic365.Client.ViewModels.Models;
using Magic365.WinUI.Contracts.Services;
using Magic365.WinUI.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Magic365.WinUI.ViewModels;
public partial class SettingsViewModel : ObservableObject
{

    private readonly IThemeSelectorService _themeSelectorService;
    private readonly ILocalSettingsService _localSettingsService;
    private readonly IAuthenticationProvider _authenticationProvider;
    private readonly IUsagesClient _usagesClient;
    private readonly INavigationService _navigationService;
    public SettingsViewModel(IThemeSelectorService themeSelectorService,
                             ILocalSettingsService localSettingsService,
                             IAuthenticationProvider authenticationProvider,
                             IUsagesClient usagesClient,
                             INavigationService navigationService)
    {
        _themeSelectorService = themeSelectorService;
        _localSettingsService = localSettingsService;
        _authenticationProvider = authenticationProvider;

        _usagesClient = usagesClient;
        _navigationService = navigationService;

        if (_themeSelectorService.Theme == ElementTheme.Dark)
        {
            _theme = Themes[1];
        }
        else if (_themeSelectorService.Theme == ElementTheme.Light)
        {
            _theme = Themes.First();
        }
        else
        {
            _theme = Themes.Last();
        }
    }

    [ObservableProperty]
    private IReadOnlyList<ThemeOption> _themes = new List<ThemeOption>()
        {

           new ThemeOption("Light", ApplicationTheme.Light),
           new ThemeOption("Dark", ApplicationTheme.Dark),
           new ThemeOption("Windows Default", null)
        };

    private ThemeOption _theme;

    public ThemeOption Theme
    {
        get => _theme;
        set
        {

            if (SetProperty(ref _theme, value))
            {
                _ = _usagesClient.TrackEventAsync(SessionVariables.User.AccessToken, new()
                {
                    EventData = new
                    {
                        theme = _theme.Name,
                    },
                    SessionId = SessionVariables.SessionId,
                    EventName = "Change Theme",
                    UserId = SessionVariables.User.Email
                });

                if (_theme.Value == null)
                    _themeSelectorService.SetRequestedThemeAsync();
                else if (_theme.Value == ApplicationTheme.Light)
                    _themeSelectorService.SetThemeAsync(ElementTheme.Light);
                else
                    _themeSelectorService.SetThemeAsync(ElementTheme.Dark);
            }
        }
    }

    public record ThemeOption(string Name, ApplicationTheme? Value);

    public User User => App.User;

    [RelayCommand]
    private async Task LogoutAsync()
    {
        _ = _usagesClient.TrackEventAsync(SessionVariables.User.AccessToken, new()
        {
            EventData = new
            {
                theme = _theme.Name,
            },
            SessionId = SessionVariables.SessionId,
            EventName = "Log Out",
            UserId = SessionVariables.User.Email
        });

        await _authenticationProvider.SignOutAsync();
        App.MainWindow.Content = App.GetService<LoginPage>();
    }

    [RelayCommand]
    private async Task OpenBuyMeCoffeeAsync()
    {
        _ = _usagesClient.TrackEventAsync(SessionVariables.User.AccessToken, new()
        {
            SessionId = SessionVariables.SessionId,
            EventName = "Click on Buy Me Coffee",
            UserId = SessionVariables.User.Email
        });
        await Windows.System.Launcher.LaunchUriAsync(new Uri("https://www.buymeacoffee.com/akacademy99"));
    }

    [RelayCommand]
    private void OpenGitHubRepo()
    {
        _ = _usagesClient.TrackEventAsync(SessionVariables.User.AccessToken, new()
        {
            SessionId = SessionVariables.SessionId,
            EventName = "Click on Open Settings GitHub Repository",
            UserId = SessionVariables.User.Email
        });
    }

    [RelayCommand]
    private void OpenWebsite()
    {
        _ = _usagesClient.TrackEventAsync(SessionVariables.User.AccessToken, new()
        {
            SessionId = SessionVariables.SessionId,
            EventName = "Click on Open Settings Website",
            UserId = SessionVariables.User.Email
        });
    }

}

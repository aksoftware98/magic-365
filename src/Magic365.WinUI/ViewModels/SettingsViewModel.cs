﻿using System;
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
    public SettingsViewModel(IThemeSelectorService themeSelectorService,
                             ILocalSettingsService localSettingsService,
                             IAuthenticationProvider authenticationProvider,
                             IUsagesClient usagesClient)
    {
        _themeSelectorService = themeSelectorService;
        _localSettingsService = localSettingsService;
        _authenticationProvider = authenticationProvider;

        _theme = Themes.Last();
        _usagesClient = usagesClient;
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

}

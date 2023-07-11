// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Magic365.Client.ViewModels;
using Magic365.Client.ViewModels.Interfaces;
using Magic365.Client.ViewModels.Models;
using Magic365.WinUI.Helpers;
using Magic365.WinUI.ViewModels;
using Magic365.WinUI.Views;
using Microsoft.AppCenter.Analytics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.AppService;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Magic365.WinUI.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        private readonly LoginViewModel _viewModel;
        private readonly INavigationService _navigationService;
        private readonly ILocalSettingsService _localSettingsService;
        public LoginPage()
        {
            this.InitializeComponent();
            Loaded += LoginPage_Loaded;
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Magic365"));
            DataContext = _viewModel = App.GetService<LoginViewModel>();
            _navigationService = App.GetService<INavigationService>();
            _localSettingsService = App.GetService<ILocalSettingsService>();
            _viewModel.OnLoginUserSuccessfully += OnLoginUserSuccessfully;
            App.OnUserLogsIn += delegate
            {
                var _shellPage = App.GetService<ShellPage>();
                App.MainWindow.Content = _shellPage;
            };

            App.MainWindow.ExtendsContentIntoTitleBar = true;
            App.MainWindow.SetTitleBar(AppTitleBar);
            App.MainWindow.Activated += MainWindow_Activated;

            Analytics.StartSession();
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            var resource = args.WindowActivationState == WindowActivationState.Deactivated ? "WindowCaptionForegroundDisabled" : "WindowCaptionForeground";

            AppTitleBarText.Foreground = (SolidColorBrush)App.Current.Resources[resource];
            App.AppTitlebar = AppTitleBarText as UIElement;
        }


        private async void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBarHelper.UpdateTitleBar(RequestedTheme);
            var result = await _localSettingsService.ReadSettingAsync<bool>("IsLoggedIn");
            if (result)
            {
                await _viewModel.SignInCommand.ExecuteAsync(null);
            }
        }

        private void OnLoginUserSuccessfully(User user)
        {
            App.LoginUser(user);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Loaded -= LoginPage_Loaded;
            _viewModel.OnLoginUserSuccessfully -= OnLoginUserSuccessfully;
        }
    }
}

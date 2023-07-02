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
    }
}


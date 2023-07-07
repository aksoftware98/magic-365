using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Magic365.Client.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Magic365.WinUI.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class HistoryPage : Page
{

    public HistoryViewModel ViewModel
    {
        get;
    }

    public HistoryPage()
    {
        this.InitializeComponent();
        DataContext = ViewModel = App.GetService<HistoryViewModel>();
        Loaded += HistoryPage_Loaded;
        

    }

    private async void HistoryPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.LoadHistoryCommand.ExecuteAsync(null);
    }
}

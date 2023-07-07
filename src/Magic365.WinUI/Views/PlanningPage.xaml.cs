// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Magic365.Client.ViewModels.Services;
using Magic365.Client.ViewModels;
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
using Windows.Foundation;
using Windows.Foundation.Collections;
using Magic365.WinUI.Services;
using Magic365.Client.ViewModels.Interfaces;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Magic365.WinUI.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PlanningPage : Page
{
    public PlanningViewModel ViewModel
    {
        get; set;
    }
    public PlanningPage()
    {
        this.InitializeComponent();
        DataContext = ViewModel = App.GetService<PlanningViewModel>();
        var usagesService = App.GetService<IUsagesClient>();
        usagesService.TrackEventAsync(SessionVariables.User.AccessToken, new()
        {
            UserId = SessionVariables.User.Email,
            EventName = "Open Planning Page",
            SessionId = SessionVariables.SessionId
        });
    }

    private void ListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        var listView = (ListView)sender;
        var childItem = FindVisualChild<AutoSuggestBox>(listView);
        childItem.Focus(FocusState.Programmatic);
    }

    private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(obj, i);
            if (child != null && child is childItem)
            {
                return (childItem)child;
            }
            else
            {
                childItem childOfChild = FindVisualChild<childItem>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
        }
        return null;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        ViewModel.Note = e.Parameter as string ?? string.Empty;
    }

}

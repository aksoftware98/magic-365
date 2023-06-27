// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Magic365.Client.ViewModels;
using Magic365.Client.ViewModels.Models;
using Magic365.Client.WinUI.Services;
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
		public LoginPage()
		{
			this.InitializeComponent();

            DataContext = _viewModel = App.GetService<LoginViewModel>();
			_viewModel.OnLoginUserSuccessfully += OnLoginUserSuccessfully;
		}

		private void OnLoginUserSuccessfully(User user)
		{
			App.LoginUser(user);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			_viewModel.OnLoginUserSuccessfully -= OnLoginUserSuccessfully;
		}
	}
}

using Magic365.Client.ViewModels.Interfaces;
using Magic365.WinUI.Contracts.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinRT.Interop;

namespace Magic365.WinUI.Services
{
	public class MessageDialogService : IMessageDialogService
	{

        private readonly IWinUINavigationService _activationService;

        public MessageDialogService(IWinUINavigationService activationService)
        {
            _activationService = activationService;
        }

        public async Task ShowOkDialogAsync(string title, string body)
		{
			var contentDialgo = new ContentDialog()
			{
				Title = title,
				Content = body,
				CloseButtonText = "Ok",
				XamlRoot = App.MainWindow.Content.XamlRoot,
				//RequestedTheme = _activationService.Frame?.ActualTheme,
			};

			await contentDialgo.ShowAsync();
		}
	}
}

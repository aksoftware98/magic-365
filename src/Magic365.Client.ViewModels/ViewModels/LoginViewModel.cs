using CommunityToolkit.Mvvm.Input;
using Magic365.Client.ViewModels.Interfaces;
using Magic365.Client.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic365.Client.ViewModels
{
	public partial class LoginViewModel : BaseViewModel
	{

		private readonly IAuthenticationProvider _authProvider;
		private readonly INavigationService _navigation;
		private readonly IMessageDialogService _messageDialogService;

		public LoginViewModel(IAuthenticationProvider authProvider,
							  INavigationService navigation,
							  IMessageDialogService messageDialogService)
		{
			_authProvider = authProvider;
			_navigation = navigation;
			_messageDialogService = messageDialogService;
		}

		public event Action<User> OnLoginUserSuccessfully = delegate { };
        public static User? User
        {
            get; 
            private set; 
        }
		[RelayCommand]
		private async Task SignInAsync()
        {
			IsBusy = true;
			try
            {
                var user = await _authProvider.SignInAsync();
                OnLoginUserSuccessfully.Invoke(user);
                User = user;
            }
			catch (Exception ex)
			{
				// TODO: Log the error 
				await _messageDialogService.ShowOkDialogAsync("Error", "An error occurred while trying to sign in. Please try again later.");
			}
			
			IsBusy = false; 
        }

		[RelayCommand]
		private async Task SignOutAsync()
		{
			IsBusy = true; 
			await _authProvider.SignOutAsync();
			IsBusy = false; 
		}

	}
}

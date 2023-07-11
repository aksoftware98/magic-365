using CommunityToolkit.Mvvm.Input;
using Magic365.Client.ViewModels.Interfaces;
using Magic365.Client.ViewModels.Models;
using Magic365.Shared.DTOs;
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
        private readonly IUsagesClient _usagesClient;
        
        public LoginViewModel(IAuthenticationProvider authProvider,
                              INavigationService navigation,
                              IMessageDialogService messageDialogService,
                              IUsagesClient usagesClient)
        {
            _authProvider = authProvider;
            _navigation = navigation;
            _messageDialogService = messageDialogService;
            _usagesClient = usagesClient;
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
                SessionVariables.User = User = user;
                var sessionId = await _usagesClient.StartUserSessionAsync(User.AccessToken, User);
                SessionVariables.SessionId = sessionId;
                OnLoginUserSuccessfully.Invoke(user);
            }
			catch (Exception ex)
			{
				// TODO: Log the error 
				await _messageDialogService.ShowOkDialogAsync("Error", ex.Message);
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
}

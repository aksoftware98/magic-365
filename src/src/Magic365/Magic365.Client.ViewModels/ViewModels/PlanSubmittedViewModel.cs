using CommunityToolkit.Mvvm.Input;
using Magic365.Client.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic365.Client.ViewModels.ViewModels
{
	public partial class PlanSubmittedViewModel : BaseViewModel
	{

		private readonly INavigationService _navigationService;

		public PlanSubmittedViewModel(INavigationService navigationService)
		{
			_navigationService = navigationService;
		}

		[RelayCommand]
		private void NavigateToPlanningPage()
		{
			_navigationService.NavigateTo("PlanningPage");
		}
	}
}

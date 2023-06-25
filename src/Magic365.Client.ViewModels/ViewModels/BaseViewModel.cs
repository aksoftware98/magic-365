using CommunityToolkit.Mvvm.ComponentModel;

namespace Magic365.Client.ViewModels
{
	public partial class BaseViewModel : ObservableObject
	{
		#region Properties 
		[ObservableProperty]
		private bool _isBusy = false; 
		#endregion 
	}
}

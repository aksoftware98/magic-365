using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magic365.Client.ViewModels.Interfaces;
using Magic365.Shared.DTOs;
using System.Collections.ObjectModel;

namespace Magic365.Client.ViewModels
{
	public partial class PlanViewModel : ObservableObject
	{

        private readonly IPlanningClient _planningClient;

		[ObservableProperty]
		private ObservableCollection<PlanItemViewModel> _items = new();

        public PlanViewModel(PlanDetails planResult, IPlanningClient planningClient)
        {
            Items = new(planResult.Items.Select(p => new PlanItemViewModel(p, RemoveItem, this)));
            _planningClient = planningClient;
        }

        private void RemoveItem(string id)
		{
			var item = Items.SingleOrDefault(i => i.Id == id);
			if (item != null)
				Items.Remove(item);
		}

        public async Task<IEnumerable<MeetingPerson>> SearchContactsAsync(string searchText)
        {
            return await _planningClient.SearchContactAsync(LoginViewModel.User.AccessToken, searchText);
        }
	}
}

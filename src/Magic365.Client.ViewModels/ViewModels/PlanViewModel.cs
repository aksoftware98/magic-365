using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magic365.Client.ViewModels.Interfaces;
using Magic365.Client.ViewModels.Models;
using Magic365.Shared.DTOs;
using System.Collections.ObjectModel;

namespace Magic365.Client.ViewModels
{
	public partial class PlanViewModel : ObservableObject
	{

        private readonly IPlanningClient _planningClient;
        private readonly IUsagesClient _usagesClient;

		[ObservableProperty]
		private ObservableCollection<PlanItemViewModel> _items = new();
        public PlanViewModel(PlanDetails planResult, IPlanningClient planningClient, IUsagesClient usagesClient)
        {
            _usagesClient = usagesClient;
            _planningClient = planningClient;
            Items = new(planResult.Items.Select(p => new PlanItemViewModel(p, RemoveItem, this, usagesClient)));
        }
        private void RemoveItem(string id)
		{
            _ = _usagesClient.TrackEventAsync(SessionVariables.User.AccessToken, new TrackUserEventDto
            {
                EventName = "Remove Plan Item",
                SessionId = SessionVariables.SessionId,
                UserId = SessionVariables.User.Email
            });
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

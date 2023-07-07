using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magic365.Client.ViewModels.Exceptions;
using Magic365.Client.ViewModels.Interfaces;
using Magic365.Client.ViewModels.ViewModels;
using Magic365.Shared.DTOs;

namespace Magic365.Client.ViewModels;
public partial class HistoryViewModel : ObservableObject
{

    private readonly IPlanningClient _planningClient;
    private readonly INavigationService _navigationService;
    private readonly IUsagesClient _usagesClient;
    private readonly IMessageDialogService _dialogsService;

    public HistoryViewModel(IPlanningClient planningClient,
                            INavigationService navigationService,
                            IUsagesClient usagesClient,
                            IMessageDialogService dialogsService)
    {
        _planningClient = planningClient;
        _navigationService = navigationService;
        _usagesClient = usagesClient;
        _dialogsService = dialogsService;
    }

    [ObservableProperty]
    private bool _isBusy = false;

    [ObservableProperty]
    private bool _isHistoryEmpty = false;

    [ObservableProperty]
    private ObservableCollection<PlanHistoryViewModel> _items = new();

    [RelayCommand]
    private async Task LoadHistoryAsync()
    {
        _ = _usagesClient.TrackEventAsync(SessionVariables.User.AccessToken, new()
        {
            EventName = "Open History Page",
            SessionId = SessionVariables.SessionId,
            UserId = SessionVariables.User.Email,
        });

        IsBusy = true;
        try
        {
            var result = await _planningClient.ListHistoryAsync(SessionVariables.User.AccessToken,
                                                                SessionVariables.User.Email);
            Items = new ObservableCollection<PlanHistoryViewModel>(result
                                                                    .Select(x => new PlanHistoryViewModel(_navigationService, _usagesClient, x)));
            IsHistoryEmpty = !Items.Any();
        }
        catch (ApiException ex)
        {
            await _dialogsService.ShowOkDialogAsync("Error", ex.Message);
        }
        catch (Exception)
        {
            // TODO: Log the error 
            await _dialogsService.ShowOkDialogAsync("Error", "An error occurred while loading the history");
        }
        IsBusy = false; 
    }

    [RelayCommand]
    private void GoToPlanningPage()
    {
        _navigationService.NavigateTo(typeof(PlanningViewModel).FullName);
    }

}

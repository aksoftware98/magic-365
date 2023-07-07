using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magic365.Client.ViewModels.Interfaces;
using Magic365.Shared.DTOs;

namespace Magic365.Client.ViewModels.ViewModels;
public partial class PlanHistoryViewModel : ObservableObject
{

    private INavigationService _navigationService;
    private IUsagesClient _usagesClient;
    public PlanHistoryViewModel(INavigationService navigationService,
                                IUsagesClient usagesClient,
                                PlanHistoryDto model)
    {
        _navigationService = navigationService;
        Model = model;
        _usagesClient = usagesClient;
    }

    [ObservableProperty]
    private PlanHistoryDto? _model;


    [RelayCommand]
    private void OpenPlanningPage()
    {
        _ = _usagesClient.TrackEventAsync(SessionVariables.User.AccessToken, new()
        {
            EventData = new
            {
                secondsPassed = (DateTime.UtcNow - Model.Date).TotalSeconds,
                note = Model.Note,
                DateTime = Model.Date,
            },
            EventName = "Use Note Again",
            SessionId = SessionVariables.SessionId,
            UserId = SessionVariables.User.Email,
        });
        _navigationService.NavigateTo(typeof(PlanningViewModel).FullName, Model.Note);
    }

    
}

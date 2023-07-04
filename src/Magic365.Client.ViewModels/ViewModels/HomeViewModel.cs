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
using Magic365.Shared.DTOs;

namespace Magic365.Client.ViewModels;
public partial class HomeViewModel : ObservableObject
{

    private readonly IPlanningClient _planningClient;
    private readonly IMessageDialogService _dialogService;
    public HomeViewModel(IPlanningClient planningClient, 
                         IMessageDialogService dialogService)
    {

        _planningClient = planningClient;
        _dialogService = dialogService;
    }

    [ObservableProperty]
    private bool _isBusy = false;

    [ObservableProperty]
    private ObservableCollection<ToDoItemDto> _toDoItems = new();

    [ObservableProperty]
    private ObservableCollection<CalendarEventDto> _calendarEvents = new();

    [RelayCommand]
    private async Task LoadDashboardAsync()
    {
        IsBusy = true;
        try
        {
            var token = LoginViewModel.User.AccessToken;
            var toDoItemsTask = _planningClient.ListUndoneToDoTasksAsync(token);
            var calendarEventsTask = _planningClient.ListUpcomingCalendarEventsAsync(token);
            await Task.WhenAll(toDoItemsTask, calendarEventsTask);
            ToDoItems = new ObservableCollection<ToDoItemDto>(toDoItemsTask.Result);
            var events = calendarEventsTask.Result;
            foreach (var item in events)
            {
                item.StartDate = item.StartDate.ToLocalTime();
                item.EndDate = item.EndDate.ToLocalTime();
            }
            CalendarEvents = new ObservableCollection<CalendarEventDto>(events);
        }
        catch (ApiException ex)
        {
            // TODO: Log the error 
            await _dialogService.ShowOkDialogAsync("Error", ex.Message);
        }
        catch (Exception ex)
        {
            // TODO: Log the error
            await _dialogService.ShowOkDialogAsync("Error", "Something went wrong, please try again later");
        }
        IsBusy = false;
    }

}

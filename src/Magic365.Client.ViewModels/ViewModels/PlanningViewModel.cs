using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Magic365.Client.ViewModels.Exceptions;
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
    public partial class PlanningViewModel : BaseViewModel
    {

        private readonly IPlanningClient _planningClient;
        private readonly INavigationService _navigation;
        private readonly IMessageDialogService _dialogsService;
        private readonly IUsagesClient _usagesClient;
        private readonly User? _user;
        public PlanningViewModel(IPlanningClient planningClient,
                                 INavigationService navigation,
                                 IMessageDialogService dialogsService,
                                 IUsagesClient usagesClient)
        {
            _planningClient = planningClient;
            _navigation = navigation;
            _user = LoginViewModel.User;
            _dialogsService = dialogsService;
            _usagesClient = usagesClient;
        }

        #region Proeprties 
        [ObservableProperty]
        private bool _isPlanSubmitted = false;

        [ObservableProperty]
        private string _note = string.Empty;

        [ObservableProperty]
        private PlanViewModel? _plan = null;

        #endregion
       
        [RelayCommand]
        private async Task AnalyzeNoteAsync()
        {
            if (string.IsNullOrWhiteSpace(Note))
                return;

            _ = _usagesClient.TrackEventAsync(_user.AccessToken, new TrackUserEventDto
            {
                EventName = "Analyze Note",
                SessionId = SessionVariables.SessionId,
                UserId = _user.Email
            });

            try
            {
                IsBusy = true;
                var plan = await _planningClient.AnalyzeNoteAsync(_user.AccessToken, Note);
                Plan = new(plan, _planningClient, _usagesClient);

                IsPlanSubmitted = true;
            }
            catch (ApiException ex)
            {
                // TODO: Log the error
                await _dialogsService.ShowOkDialogAsync("Error", ex.Message);
            }
            catch (Exception)
            {
                // TODO: Log the error 
                await _dialogsService.ShowOkDialogAsync("Error", "An error occured while submitting the plan. Please try again later.");
            }

            IsBusy = false;
        }

        [RelayCommand]
        private async Task SubmitPlanAsync()
        {
            // TODO: Validate the plan client-side 
            if (Plan == null)
                return;

            _ = _usagesClient.TrackEventAsync(_user.AccessToken, new TrackUserEventDto
            {
                EventName = "Submit Plan",
                SessionId = SessionVariables.SessionId,
                UserId = _user.Email
            });

            IsBusy = true;

            var planRequest = BuildRequestObject();
            try
            {
                await _planningClient.SubmitPlanAsync(_user.AccessToken, planRequest);

                _navigation.NavigateTo("PlanSubmittedPage");
            }
            catch (ApiException ex)
            {
                // TODO: Log the error
                await _dialogsService.ShowOkDialogAsync("Error", ex.Message);
            }
            catch (Exception)
            {
                // TODO: Log the error 
                await _dialogsService.ShowOkDialogAsync("Error", "An error occured while submitting the plan. Please try again later.");
            }
            IsBusy = false;

        }

        public async Task<IEnumerable<MeetingPerson>> SearchContactAsync(string query)
        {
            try
            {
                _ = _usagesClient.TrackEventAsync(_user.AccessToken, new TrackUserEventDto
                {
                    EventName = "Search Contact",
                    SessionId = SessionVariables.SessionId,
                    UserId = _user.Email
                });
                return await _planningClient.SearchContactAsync(_user.AccessToken, query);
            }
            catch (ApiException ex)
            {
                // TODO: Log the error
                await _dialogsService.ShowOkDialogAsync("Error", ex.Message);
                return Enumerable.Empty<MeetingPerson>();
            }
            catch (Exception)
            {
                // TODO: Log the error 
                await _dialogsService.ShowOkDialogAsync("Error", "An error occurred while submitting the plan. Please try again later.");
                return Enumerable.Empty<MeetingPerson>();
            }
        }

        private PlanDetails BuildRequestObject()
        {
            return new()
            {
                Note = Note,
                UserId = SessionVariables.User.Email,
                Items = Plan.Items.Select(p => new PlanItem
                {
                    Type = p.Type,
                    EndTime = p.EndDateTime,
                    StartTime = p.StartDateTime,
                    Title = p.Title,
                    People = p.Contacts?.Select(c => new MeetingPerson()
                    {
                        Email = c.Email,
                        Name = c.DisplayName,
                        AddContact = c.AddContact,
                        AddEmailToContact = c.UpdateEmail,
                        Id = c.Id
                    }) ?? Enumerable.Empty<MeetingPerson>()
                })
            };
        }
    }
}

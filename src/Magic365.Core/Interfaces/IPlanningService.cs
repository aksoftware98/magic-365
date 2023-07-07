using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magic365.Shared;
using Magic365.Shared.DTOs;
namespace Magic365.Core.Interfaces
{
	public interface IPlanningService
	{

		/// <summary>
		/// Understand a note using AI and build a productivity plan out of it for user to review and fill the remaining data
		/// </summary>
		/// <param name="note">Note query request</param>
		/// <returns></returns>
		[Obsolete("The method is obsolete. Use AnalyzeNoteAsync instead within the interface INoteAnalyzeService.")]
		Task<PlanDetails> AnalyzeNoteAsync(SubmitNoteRequest note);

		/// <summary>
		/// Submit the final plan to be added to create the events and the meetings in the calendar, and the to-do items
		/// </summary>
		/// <param name="plan"></param>
		/// <returns></returns>
		Task SubmitPlanAsync(PlanDetails plan);

        /// <summary>
        /// Search contacts of the users
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IEnumerable<MeetingPerson>> FetchContactsAsync(string query);
	}
}

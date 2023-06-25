using Magic365.Shared.DTOs;

namespace Magic365.Core.Interfaces
{
	/// <summary>
	/// Interface that wraps the implementation that sends a sentence to OpenAI GPT-3 and returns the result.
	/// </summary>
	public interface INoteAnalyzingService
	{
		Task<PlanDetails> AnalyzePlanAsync(string query);
	}
}

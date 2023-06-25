using Magic365.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic365.Core.Interfaces
{
	/// <summary>
	/// Interface that wraps the implementation that sends a sentence to Azure Conversational Language Service (Cognitive Service) and returns the result.
	/// </summary>
	[Obsolete("This interface is not used anymore. The logic has been migrated from using Azure Cognitive Services to OpenAI GPT-3.")]
	public interface ILanguageUnderstandingService
	{

		Task<AITextAnalyzeResponse> AnalyzeTextAsync(string query);

		
	}
}

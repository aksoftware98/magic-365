﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Magic365.Core.Models
{
	public class AnalyzerResultItem
	{

        public AnalyzerResultItem()
        {
			Action = "";
			Type = "";
        }

        [JsonPropertyName("action")]
		public string Action { get; set; }
		[JsonPropertyName("type")]
		public string Type { get; set; }
		[JsonPropertyName("startTime")]
		public string? StartTime { get; set; }
		[JsonPropertyName("endTime")]
		public string? EndTime { get; set; }
		[JsonPropertyName("startDate")]
		public string? StartDate { get; set; }
		[JsonPropertyName("endDate")]
		public string? EndDate { get; set; }
		[JsonPropertyName("people")]
		public string[]? People { get; set; }
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic365.Client.ViewModels.Interfaces
{
	public interface INavigationService
	{

		void NavigateTo(string pageKey, object? args = null);
		
		

	}
}
